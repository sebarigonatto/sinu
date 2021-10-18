using SINU.Authorize;
using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SINU.ViewModels;
using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using System.Collections;
using static SINU.Models.AjaxDataTableModel;

namespace SINU.Controllers
{
    [Authorize]
    [AuthorizacionPermiso("RUDInscripcion")]
    public class DelegacionController : Controller
    {
        SINUEntities db = new SINUEntities();
        OficinasYDelegaciones UsuarioDelegacion;
        // GET: Delegacion
        #region Index - aqui se realiza un filtro y que me muestren los postulantes que estan en las convocatorias activas
        public ActionResult Index()
        {
            try
            {
                var control = typeof(DelegacionController);
                //busco la delegacion que pertenece al usuario con perfil de delegacion            
                UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                ViewBag.Delegacion = UsuarioDelegacion.Nombre;
                // tomara los datos de incripciones correspondiente a la Delegacion /cuenta usario Asociado
                //cargo todos los registros que hayan validado la cuenta, y esten en la carga de los datos basicos, pero además que pertenezcan a la delegacion del usuario actual.
                List<Column> Columnas = new List<Column>
                {
                        ColumnaDTAjax("IdPersona",noPrint:true),
                        ColumnaDTAjax("IdInscripcionEtapaEstado",noPrint:true),
                        ColumnaDTAjax("IdDelegacionOficinaIngresoInscribio",noPrint:true),
                        ColumnaDTAjax("IdSecuencia",noPrint:true),
                        ColumnaDTAjax("ACTIVA",noPrint:true),                        
                        ColumnaDTAjax("Nombres",true,true),
                        ColumnaDTAjax("Apellido",true,true),                    
                        ColumnaDTAjax("Email",true,true,className:"truncate"),
                        ColumnaDTAjax("Fecha2",visible:true,nombreDisplay:"Fecha"),
                        ColumnaDTAjax("Modalidad_Siglas",true,true,nombreDisplay:"Modalidad"),
                        ColumnaDTAjax("Etapa",true,true),
                        ColumnaDTAjax("Estado",true,true)
                };

                List<filtroExtra> filtros = new List<filtroExtra>{
                    new filtroExtra{Columna="ACTIVA",Valor="true"},
                    new filtroExtra{Columna="IdDelegacionOficinaIngresoInscribio",Valor=UsuarioDelegacion.IdOficinasYDelegaciones.ToString(), }
                };

                var listaTablas = new List<DataTableVM> {
                    new DataTableVM
                    {
                        TablaVista="vConsultaInscripciones",
                        Columnas= Columnas,
                        filtrosExtras= filtros.Append(new filtroExtra{Columna="IdSecuencia",Condicion="!=",Valor="4"}).ToList(),
                        IdTabla="InscriptosTODOS"
                    },
                      new DataTableVM
                    {
                        TablaVista="vConsultaInscripciones",
                        Columnas= Columnas,
                        filtrosExtras= filtros.Append(new filtroExtra{Columna="Etapa",Valor="DATOS BASICOS"}).ToList(),                      
                        IdTabla="InscriptosDATOSBASICOS"
                    },
                        new DataTableVM
                    {
                        TablaVista="vConsultaInscripciones",
                        Columnas= Columnas,
                        filtrosExtras=filtros.Append(new filtroExtra{Columna="Etapa",Valor="ENTREVISTA"}).ToList(),
                        IdTabla="InscriptosENTREVISTA"
                    },
                          new DataTableVM
                    {
                        TablaVista="vConsultaInscripciones",
                        Columnas= Columnas,
                        filtrosExtras=filtros.Append(new filtroExtra{Columna="Etapa",Valor="DOCUMENTACION"}).ToList(),
                        IdTabla="InscriptosDOCUMENTACION"
                    },
                            new DataTableVM
                    {
                        TablaVista="vConsultaInscripciones",
                        Columnas= Columnas,
                        filtrosExtras=filtros.Append(new filtroExtra{Columna="Etapa",Valor="PRESENTACION"}).ToList(),
                        IdTabla="InscriptosPRESENTACION"
                    },
                }; 
               
                return View("Index", listaTablas);

            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Index"));
            }
        }

        #endregion

        #region Restaurar Postulante(Reutilizo Details) - reutilizo detalles para crear una pantalla para que un postulante pueda ser restaurado al proceso de inscripcion
        /// <summary>
        /// Reutilizo este datails(Modifico la Vista para poder Restaurar un Postulante que en el caso se haya interrumpido su proceso)
        /// </summary>
        /// <param name="id">recibe como parametro el IDPostulante o IdPersona que son iguales</param>
        /// <returns></returns>
        public ActionResult Details(int? id)//Recibe el IdPersona
        {
            try
            {
                RestaurarPostulanteVM datos = new RestaurarPostulanteVM()
                {
                    vInscripcionDetallesVM = db.vInscripcionDetalle.Where(m => m.IdPersona == id).ToList(),
                    vDataProblemaEncontradoVM = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == id).ToList()
                };
                return View(datos);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Details"));
            }

        }
        #endregion

        #region Asignar Fecha de Entrevista(GET) - accion que le asigna una fecha de entrevista a un solo postulante
        /// <summary>
        /// Accion de tipo GET esta accion tiene que devolver los datos segun el parametro que lo pasaron
        /// recibe como parametro el IdPersona
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Devuelve los datos de la persona a la cual recibe el Id</returns>
        public ActionResult EntrevistaAsignaFecha(int id)///Recibe el IdPersona
        {
            try
            {
                vEntrevistaLugarFechaUltInscripc Dato = db.vEntrevistaLugarFechaUltInscripc.FirstOrDefault(m => m.IdPersona == id);///en la variable "Dato" guardo la informacion de la persona con idPersona igual que hay en la tabla

                return View(Dato);///devuelvo a la vista el modelo "Dato" es donde contiene toda la informacion de la persona
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        #endregion

        #region Asignar Fecha de Entrevista(POST) - accion que le asigna una fecha de entrevista a un solo postulante
        /// <summary>
        /// Accion de tipo POST: lo que va a realizar esta accion es guardar el tabla inscripcion la fecha que se le fue otorgada por una delegacion
        /// recibe como parametros "datos"(que es una collecion de input enviados por)
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EntrevistaAsignaFecha(vEntrevistaLugarFecha datos)
        {
            List<vInscripcionDetalle> InscripcionElegida;
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;
            try
            {
                if (ModelState.IsValid)
                {
                    var da = db.Inscripcion.Find(datos.IdInscripcion);
                    da.FechaEntrevista = datos.FechaEntrevista;
                    db.SaveChanges();
                    
                    if (db.vInscripcionDetalleUltInsc.FirstOrDefault(m=>m.IdInscripcion==da.IdInscripcion).IdSecuencia!=10)
                    {
                        db.spProximaSecuenciaEtapaEstado(0, datos.IdInscripcion, false, 0, "ENTREVISTA", "Asignada");

                    }
                        

                    MailConfirmacionEntrevista Modelo = new MailConfirmacionEntrevista
                    {
                        Apellido = datos.Apellido, ///cambiar por nombre!!
                        FechaEntrevista = datos.FechaEntrevista
                    };
                    //verificar el llamado de una funcion asyncronica desde un metodo sincronico
                    var Result = Func.EnvioDeMail(Modelo, "MailConfirmacionEntrevista", null, datos.IdPersona, "MailAsunto4", null, null);

                    InscripcionElegida = db.vInscripcionDetalle.Where(m => m.IdInscripcion == datos.IdInscripcion).ToList();
                    vInscripcionEtapas = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == datos.IdInscripcion);

                    if (vInscripcionEtapas.Estado == "Asignada")
                    {
                        db.spProximaSecuenciaEtapaEstado(0, datos.IdInscripcion, false, 0, "ENTREVISTA", "Pendiente");
                    }
                    return RedirectToAction("Index",new { id = "Entrevista" });

                }
                return View(datos);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        #endregion
        #region Ninguno de estas acciones funciona son los ACtionResult  por defecto
        [HttpPost]
        // GET: Delegacion/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                vInscripcionDetalle elegido = db.vInscripcionDetalle.First(m => m.IdPersona == id);
                if (elegido == null)
                {
                    return View("Error", Func.ConstruyeError("Ese usuario no se encontro", "Delegacion", "Edit"));
                }

                return View(elegido);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Edit"));
            }
        }

        // POST: Delegacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Edit"));
            }
        }

        // GET: Delegacion/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
        }

        // POST: Delegacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
        }

        #endregion

        #region Postular(GET) - muestra los datos del postulante al que van a Postular/No Postular
        [HttpGet]
        public ActionResult Postular(int? id)
        {
            vInscripcionDetalle InscripcionElegida;
            vInscripcionEtapaEstadoUltimoEstado vInscripcion;
            try
            {
                UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                ViewBag.Delegacion = UsuarioDelegacion.Nombre;
                if (id == null)
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);               
                    return View("Error", Func.ConstruyeError("Falta el Nro de ID que desea buscar en la tabla de INSCRIPTOS", "Delegacion", "Details"));
                }

                InscripcionElegida = db.vInscripcionDetalle.FirstOrDefault(m => m.IdInscripcion == id);

                if (InscripcionElegida == null)
                {
                    //return HttpNotFound("ese numero de ID no se encontro ");
                    return View("Error", Func.ConstruyeError("Incorrecta la llamada a la vista detalle con el id " + id.ToString() + " ==> NO EXISTE o no le corresponde verlo", "Delegacion", "Details"));
                }
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Details"));
            }
            vInscripcion = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == id);
            ViewBag.Estado = vInscripcion.Estado;
            return View(InscripcionElegida);
        }
        #endregion

        #region Postular(POST) - accion que Postula/No Postula a un postulante y puede seguir avanzando en el proceso de inscripcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        /// <summary>
        /// Accion de metodo POST en donde se va a poder postular/no postular o restaurar a un postulante esta accion recibe 2 parametros de la vista Postular
        /// </summary>
        /// <param name="botonPostular">Valor que envia el boton segun lo que presiono el usuario delegacion</param>
        /// <param name="id">Es un IdPersona o IdPostulante </param>
        /// <returns></returns>
        public async Task<ActionResult> Postular(string botonPostular, int id)
        {
            List<vInscripcionDetalle> InscripcionElegida;///Creo una List para poder traer los datos del postulante
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;///Creo esta 2° list para que me traiga la ultima Etapa en la que se encuentra el postulante
            Configuracion configuracion;///traigo datos de la tabla configuracion para poder armar el mail de notificacion para los postulantes
            bool x = false;///variable que luego se enviara al postulante para que pueda ver el boton que lo direccione al sistema
                           ///si esta en false eso quiere decir que no fue postulado
            string cuerpo = "";
            try
            {
                switch (botonPostular)
                {
                    case "Postular":
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "ENTREVISTA", "Postulado");
                        x = true;
                        configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4Postulado");
                        cuerpo = configuracion.ValorDato.ToString();
                        break;
                    case "No Postular":
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "ENTREVISTA", "No Postulado");
                        //el texto de respuesta aqui depende de la preferencia que selecciono al momento de llenar datos basicos
                        configuracion = (db.Inscripcion.FirstOrDefault(m => m.IdInscripcion == id).IdPreferencia == 6) ? db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado2") : db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado1");
                        cuerpo = configuracion.ValorDato.ToString();
                        break;
                    case "Volver":
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "ENTREVISTA", "Pendiente");
                        break;
                }
                InscripcionElegida = db.vInscripcionDetalle.Where(m => m.IdInscripcion == id).ToList();
                vInscripcionEtapas = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == id);

                var callbackUrl = Url.Action("Index", "Postulante", null, protocol: Request.Url.Scheme);///se crea este link, que luego sera enviado en el mail del postulante para poder acceder al sistema 

                var modeloPlanti = new ViewModels.MailPostular///plantilla que es enviada al cuerpo del mail
                {
                    Apellido = "",/// cambiar por nombre !!!
                    MailCuerpo = cuerpo.Replace("$Nombre", vInscripcionEtapas.Nombres),/// se remplaza con la variable creada en la base de datos con un nombre del postulante
                    LinkConfirmacion = callbackUrl,
                    Postulado = x
                };

                await Task.Delay(1000);

                switch ((vInscripcionEtapas.Estado).ToString())
                {
                    case "Postulado":
                        Func.EnvioDeMail(modeloPlanti, "MailPostulado", null, vInscripcionEtapas.IdPersona, "MailAsunto2", null, null);
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 13, "", "");
                        break;
                    case "No Postulado":
                        Func.EnvioDeMail(modeloPlanti, "MailPostulado", null, vInscripcionEtapas.IdPersona, "MailAsunto2", null, null);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Index"));
            }
            return RedirectToAction("Index",new { id= "Entrevista"});
        }
        #endregion

        #region Documentacion(GET) - accion que le devuleve los datos cargados por el postulante(pantallas reutilizada de postulante)

        /// <summary>
        /// Recibe el IdPersona del postulante para poder traer toda informacion del postulante
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Devuelve todos los datos del Postulante</returns>
        [HttpGet]
        public ActionResult Documentacion(int id)
        {
            var vdetalle = db.vInscripcionDetalleUltInsc.FirstOrDefault(m => m.IdPersona == id);
            var NyA = vdetalle.Nombres + " " + vdetalle.Apellido;
            ViewBag.NInscripcion = vdetalle.IdInscripcion;
            ViewBag.Modalidad = vdetalle.Modalidad;

            IDPersonaVM personaVM = new IDPersonaVM();
            personaVM.ID_PER = id;
            personaVM.NomyApe = NyA;

            var PantallasEstadoProblemas = new List<Array>();
            db.spTildarPantallaParaPostulate(personaVM.ID_PER).ForEach(m => PantallasEstadoProblemas.Add(new object[] { m.Pantalla, m.Abierta, m.CantComentarios }));
            personaVM.ListProblemaCantPantalla = PantallasEstadoProblemas;
            ViewBag.PantallasEstadoProblemas2 = JsonConvert.SerializeObject(PantallasEstadoProblemas);

            //////////////////Verifico si se encuentra cargado el documento//////////////
            bool cert;
            bool anex;
            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
            string Ubicacionfile = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
            string[] anexo = Directory.GetFiles(Ubicacionfile, id + "&" + "Anexo2" + "*");
            string[] Certificado = Directory.GetFiles(Ubicacionfile, id + "&" + "Certificado" + "*");

            if (anexo.Count() == 1)
            {
                anex = true;
                personaVM.anexo2 = anex;
            }
            if (Certificado.Count() == 1)
            {
                cert = true;
                personaVM.certificado = cert;
            }
            /////////////////////////fin del cofigo////////////////////////////////////////

            ///////////////////////Codigo para generar la lista de problemas al final de la documentacion////////////////////////
            personaVM.vDataProblemaEncontradosVmDocu = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == id).ToList();
            /////////////////////////////////fin del cofigo///////////////////////////////////////////////////////////////


            /////////////////////////////////Boton notificar enabled/disabled/////////////////////////////////////////
            //var ExistProblema = db.DataProblemaEncontrado.Where(m => m.IdPostulantePersona == vdetalle.IdPersona && m.IdDataVerificacion == 26).Any();
            //ViewBag.Problem = ExistProblema;

            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            return View(personaVM);
        }
        #endregion

        #region Actualizar Iconos de Documentacion (GET) - esto le devuleve una array con los datos de pantallas abirtas y devuleve un result json para que se actualize sin hacer refresh
        [HttpGet]
        public JsonResult ActualizaIcon(int id_persona)
        {
            var PantallasEstadoProblemas = new List<Array>();
            db.spTildarPantallaParaPostulate(id_persona).ForEach(m => PantallasEstadoProblemas.Add(new object[] { m.Pantalla, m.Abierta, m.CantComentarios }));
            //personaVM.ListProblemaCantPantalla = PantallasEstadoProblemas;
            var PantallasEstadoProblemas2 = JsonConvert.SerializeObject(PantallasEstadoProblemas);

            return Json(new { estado = PantallasEstadoProblemas2 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Confirmar - Esta accion le perimte al usuario(Delegacion) poder avanzar al postulante,lo hace avanzar a la etapa Presentacion, y tambien le envia un mail de notificacion donde se comunicara que la documentacion esta validada

        /// <summary>
        /// En esta accion el usuario delegacion puede enviar un postulantes a la ETAPA de "Presentacion"
        /// </summary>
        /// <param name="id">IdInscripcion</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Documentacion(int? id)///IdInscripcion
        {
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapaEstado;
            Configuracion configuracion;
            List<vDataProblemaEncontrado> data;
            string cuerpo = "";
            try
            {
                vInscripcionEtapaEstado = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == id);
                configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpoDocumentacionValidado");///se trae de la tabla configuracion los datos del mail
                cuerpo = configuracion.ValorDato.ToString();///de la varialble "configuracion"  solo nos interesa el cuerpo del mail que sera enviada por una viewmodel al cuerpo del mail
                data = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == id).ToList();///listado de los problemas encontrado en el postulante
                var PantallaCerradas = db.spTildarPantallaParaPostulate(id).Where(m => m.Abierta == true).ToList();/// verifica si hay pantallas abiertas con problemas o no validadas
                if (PantallaCerradas.Count == 0)///si en pantallascerrad no encuentra problemas se puede validar el postulante para la proxima ETAPA
                {
                    var modeloPlantilla = new ViewModels.MailDocumentacion
                    {
                        Estado = "Validado",
                        MailCuerpo = cuerpo,
                        Apellido = vInscripcionEtapaEstado.Apellido,
                        Errores = data
                    };
                    var Result = Func.EnvioDeMail(modeloPlantilla, "MailDocumentacion", null, id, "MailAsunto9", null, null);
                    db.spProximaSecuenciaEtapaEstado(id, 0, false, 0, "", "");
                    return Json(new { View = "Index" });
                };
                return Json(new { success = true, msg = "Hay Pantallas no validadas" });
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }

        }

        #endregion

        #region Volver Etapa Anterior - Esta accion le perimte al usuario(delegacion) poder volver a la etapa anterior para que pueda corregir sus datos tambien se el envia un mail de notificacion para que modifique sus datos

        /// <summary>
        /// En esta accion se puede volver un postulante a la ETAPA "Documentacion - Incio de carga" donde el postulante podra corregir sus falencias
        /// </summary>
        /// <param name="ID_persona"></param>
        /// <returns></returns>
        public async Task<ActionResult> VolverEtapa(int? ID_persona)
        {
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapaEstado;
            Configuracion configuracion;
            List<vDataProblemaEncontrado> data;
            string cuerpo = "";
            try
            {
                vInscripcionEtapaEstado = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == ID_persona);
                configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpoDocumentacionConErrores");
                cuerpo = configuracion.ValorDato.ToString();
                data = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona).ToList();
                var PantallaCerradas = db.spTildarPantallaParaPostulate(ID_persona).Where(m => m.Abierta == true).ToList();
                if (PantallaCerradas.Count != 0)
                {
                    var modeloPlantilla = new ViewModels.MailDocumentacion
                    {
                        Estado = "Volver Etapa",
                        MailCuerpo = cuerpo,
                        Apellido = vInscripcionEtapaEstado.Apellido,/// cambiar por nombre
                        Errores = data
                    };
                    Func.EnvioDeMail(modeloPlantilla, "MailDocumentacion", null, ID_persona, "MailAsunto9", null, null);

                    db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "DOCUMENTACION", "Inicio De Carga");
                    return Json(new { View = "Index" });

                }
                return Json(new { success = true, msg = "Si el postulante no contiene problemas presione el boton confirmar" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
        }

        #endregion

        #region Interrumpir Proceso - Esta accion le permite al usuario(delegacion) interrumpir el proceso de inscripcion del postulante tambien se le envia un mail de notificacion al postulante

        [HttpPost]
        public async Task<ActionResult> InterrumpirProceso(int? ID_persona)
        {
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapaEstado;
            Configuracion configuracion;
            List<vDataProblemaEncontrado> data;
            string cuerpo = "";
            try
            {
                vInscripcionEtapaEstado = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == ID_persona);
                configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpoDocumentacionNoValidado");
                cuerpo = configuracion.ValorDato.ToString();
                data = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona).ToList();
                var PantallaCerradas = db.spTildarPantallaParaPostulate(ID_persona).Where(m => m.Abierta == true).ToList();
                if (PantallaCerradas.Count != 0)
                {
                    var modeloPlantilla = new ViewModels.MailDocumentacion
                    {
                        Estado = "No Validado",
                        MailCuerpo = cuerpo.Replace("$Nombre", vInscripcionEtapaEstado.Nombres),
                        Apellido = "",///Cambiar a Nnombre
                        Errores = data
                    };
                    Func.EnvioDeMail(modeloPlantilla, "MailDocumentacion", null, ID_persona, "MailAsunto9", null, null);
                    db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "DOCUMENTACION", "No Validado");
                    return Json(new { View = "Index" });

                }
                return Json(new { success = true, msg = "Si el postulante no contiene problemas presione el boton confirmar" });
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
        }

        #endregion

        #region La accion permite restaurar un postulante al proceso de inscripcion (GET)
        /// <summary>
        /// se recibe de la vista Detail ID_Persona para que el usuario delegacion pueda volver un postulante a la etapa incion de carga de Documentacion
        /// </summary>
        /// <param name="ID_persona"></param>
        /// <returns></returns>
        public ActionResult RestaurarPostulante(int? ID_persona)
        {
            try
            {
                    db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "DOCUMENTACION", "Inicio De Carga");
                    return RedirectToAction("Index",new { id= "Documentacion" });
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
        }
        #endregion

        #region accion que devuelve la Documentacion penal y el certificado de Anexo2
        public ActionResult DocPenal(int id, string docu)
        {
            try
            {
                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string Ubicacionfile = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string[] archivos = Directory.GetFiles(Ubicacionfile, id + "&" + docu + "*");
                if (archivos.Count() == 0)
                {
                    return View("Error");
                }
                byte[] FileBytes = System.IO.File.ReadAllBytes(archivos[0]);
                string app = "";
                switch (archivos[0].ToString().Substring(archivos[0].ToString().LastIndexOf('.') + 1))
                {
                    case "jpg":
                        app = "image/jpeg";
                        break;
                    case "pdf":
                        app = "application/pdf";
                        break;
                    default:
                        break;
                };
                return File(FileBytes, app);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "_Docupenal"));
            }
        }
        #endregion

        #region Fecha de Presentacion(Get) - a un solo postulantes(la accion solo le asigna una fecha de presentacion a un solo postulante)
        public ActionResult PresentacionAsignaFecha(int id)
        {
            try
            {
                UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                var IdDeleg = UsuarioDelegacion.IdOficinasYDelegaciones;
                //var establecDele = db.EstablecimientoRindeExamenDeOficina(UsuarioDelegacion.IdOficinasYDelegaciones).ToList();
                PresentaciondelPostulante presentaciondel = new PresentaciondelPostulante();
                presentaciondel.LugarPresentacion = new SelectList(db.EstablecimientoRindeExamenDeOficina(UsuarioDelegacion.IdOficinasYDelegaciones).ToList(), "IdEstablecimientoRindeExamen", "Direccion");

                presentaciondel.DetalleInscripcion = db.vInscripcionDetalleUltInsc.FirstOrDefault(m => m.IdPersona == id);
                presentaciondel.DetalleInscripcion.Rinde_En = db.Inscripcion.Find(presentaciondel.DetalleInscripcion.IdInscripcion).IdEstablecimientoRindeExamen.ToString();
                var DatosdelLugar = new List<Array>();
                db.EstablecimientoRindeExamen.ToList().ForEach(m => DatosdelLugar.Add(new object[] { m.IdEstablecimientoRindeExamen,
                                                                                               m.Jurisdiccion + ", " + m.Localidad + ", " + m.Departamento,
                                                                                               m.Nombre,
                                                                                               m.Direccion+", "+m.Comentario}));
                presentaciondel.DatosLugar = JsonConvert.SerializeObject(DatosdelLugar);
                return View(presentaciondel);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        #endregion

        #region Fecha de Presentacion (POST) - asigna una fecha de presentacion a un solo postulante
        [HttpPost]
        public ActionResult PresentacionAsignaFecha(int Id, DateTime Fecha, int LugarPresentacion)
        {
            vOficDeleg_EstablecimientoRindExamen establecExamens;
            vInscripcionDetalle Inscripto;
            Configuracion configuracion;
            try
            {
                
                db.spExamenParaEsteInscripto(Id, Fecha, LugarPresentacion);
                Inscripto = db.vInscripcionDetalle.FirstOrDefault(m => m.IdInscripcion == Id);
                int idest = (int)db.Inscripcion.FirstOrDefault(m => m.IdInscripcion == Id).IdEstablecimientoRindeExamen;
                establecExamens = db.vOficDeleg_EstablecimientoRindExamen.FirstOrDefault(m => m.IdEstablecimientoRindeExamen == idest);
                configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo10");
                var callbackUrl = Url.Action("Index", "Postulante", null, protocol: Request.Url.Scheme);
                var modelPlanti = new ViewModels.MailPresentacion
                {

                    Apellido = Inscripto.Apellido,///Cambiar a Nombre
                    establecimiento = establecExamens,
                    Link = callbackUrl,
                    MailCuerpo = configuracion.ValorDato,
                    fecha =  Fecha
                };
                var Result = Func.EnvioDeMail(modelPlanti, "MailFechaPresentacion", null, Inscripto.IdPersona, "MailAsunto10", null, null);
                if (db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m=>m.IdInscripcionEtapaEstado==Inscripto.IdInscripcion).IdSecuencia==16)
                {
                        db.spProximaSecuenciaEtapaEstado(null, Id, false, 0, "", "");
                }
               
                return RedirectToAction("Index",new { id="Presentacion"});
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public ActionResult ListaProblema(int ID_persona, int IdPanatlla)
        {
            List<vDataProblemaEncontrado> problema = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona).Where(m => m.IdPantalla == IdPanatlla).ToList();
            return View(problema);

        }
        [HttpGet]
        public ActionResult DataProblema(int? ID, int ID_persona)
        {
            try
            {
                ProblemaEcontradoVM problema = new ProblemaEcontradoVM
                {
                    ListDataVerificacionVM = new SelectList(db.DataVerificacion.Where(m => m.IdPantalla == 10).ToList(), "IdDataVerificacion", "Descripcion")
                };
                var postu = db.Persona.FirstOrDefault(m => m.IdPersona == ID_persona);
                if (ID == null)
                {
                    problema.vListDataProblemasVM = new vDataProblemaEncontrado()
                    {
                        IdPostulantePersona = ID_persona,
                        Apellido_Y_Nombres = postu.Apellido + ", " + postu.Nombres,
                        DNI = postu.DNI
                    };
                }
                else
                {
                    problema.vListDataProblemasVM = db.vDataProblemaEncontrado.FirstOrDefault(m => m.IdDataProblemaEncontrado == ID);
                }
                return PartialView(problema);
            }
            catch (System.Exception ex)
            {
                return PartialView("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Details"));
            }
        }
        [HttpPost]
        public ActionResult DataProblema(ProblemaEcontradoVM dato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (dato.vListDataProblemasVM.IdDataProblemaEncontrado == 0)
                    {
                        DataProblemaEncontrado dataProblemaEncontrado = new DataProblemaEncontrado
                        {
                            Comentario = dato.vListDataProblemasVM.Comentario,
                            IdDataVerificacion = dato.vListDataProblemasVM.IdDataVerificacion,
                            IdPostulantePersona = dato.vListDataProblemasVM.IdPostulantePersona
                        };
                        db.DataProblemaEncontrado.Add(dataProblemaEncontrado);
                        db.SaveChanges();
                        return Json(new { success = true, msg = "Se Inserto correctamente el Problema" });

                    }
                    else
                    {
                        DataProblemaEncontrado dataProblemaEncontrado = new DataProblemaEncontrado
                        {
                            Comentario = dato.vListDataProblemasVM.Comentario,
                            IdDataVerificacion = dato.vListDataProblemasVM.IdDataVerificacion,
                            IdPostulantePersona = dato.vListDataProblemasVM.IdPostulantePersona,
                            IdDataProblemaEncontrado = dato.vListDataProblemasVM.IdDataProblemaEncontrado
                        };
                        db.Entry(dataProblemaEncontrado).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, msg = "Se Inserto correctamente el Problema" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message });
            }

            return Json(new { success = false, msg = "Error en el Modelo Recibido" });
        }
        [HttpPost]
        public ActionResult DelDataProblema(int? ID)
        {
            DataProblemaEncontrado dataProblemaEncontrado = db.DataProblemaEncontrado.Find(ID);
            db.DataProblemaEncontrado.Remove(dataProblemaEncontrado);
            db.SaveChanges();
            return Json(new { success = true, msg = "Se Borro correctamente el Problema", form = "Elimina", url_Tabla = "ListaProblema", url_Controller = "Delegacion" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ProblemaPantalla(int IdPostulante, int IdPantalla)
        {
            try
            {
                ProblemaPantallaVM datos = new ProblemaPantallaVM()
                {
                    ListvDataProblemaEncontradoVM = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == IdPostulante).Where(m => m.IdPantalla == IdPantalla).ToList(),
                    DataProblemaEncontradoVM = new DataProblemaEncontrado()
                    {
                        IdPostulantePersona = IdPostulante

                    },
                    DataVerificacionVM = db.DataVerificacion.Where(m => m.IdPantalla == IdPantalla).ToList()
                };

                return PartialView(datos);
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpPost]
        public JsonResult ProblemaPantalla(ProblemaPantallaVM datos)
        {
            var ProblemaExiste = db.vDataProblemaEncontrado.FirstOrDefault(m => m.IdDataVerificacion == datos.DataProblemaEncontradoVM.IdDataVerificacion && m.IdPostulantePersona == datos.DataProblemaEncontradoVM.IdPostulantePersona && m.DataVerificacion != "Otros: Aclare");
            var Comentario = datos.DataProblemaEncontradoVM.Comentario;///guarda en la variable comentario si trae un comentario o no
            var IdDataVerificacion = datos.DataProblemaEncontradoVM.IdDataVerificacion;
            var Idpantalla = db.DataVerificacion.Find(IdDataVerificacion).IdPantalla;
            var abierto = db.spTildarPantallaParaPostulate(datos.DataProblemaEncontradoVM.IdPostulantePersona).FirstOrDefault(m => m.IdPantalla == Idpantalla);
            try
            {
                if (abierto.Abierta == false)
                {
                    return Json(new { success = true, msg = "La pantalla se encuentra validada y no se puede agregar problemas" });
                }

                if (Comentario != null)///se utiliza la variable para verificar si agregaron un comentario(Si agregaron un comentarios lo deja avanzar/ en caso contrario le pedira al usuario agregar un comentario)
                {
                    if (ProblemaExiste == null)///en este IF se utiliza la variable ProblemaExiste para probar si ya existe un problema con el mismo id en la misma pantalla(si es el valor es verdadero lo deja agregar/caso contrario avisa al usuario que hay un problema ya existente en la panatlla)
                    {
                        var data = datos.DataProblemaEncontradoVM;
                        db.DataProblemaEncontrado.Add(data);
                        db.SaveChanges();
                        return Json(new { success = true, form = "Elimina", msg = "Problema agregado", url_Tabla = "ProblemaPantalla", url_Controller = "Delegacion", IdPantalla = Idpantalla }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { success = true, msg = "El problema que desea agregar ya existe" });
                }
                return Json(new { success = true, msg = "Es importante agregar un comentario" });
            }
            catch (Exception x)
            {

                return Json(new { msg = x.InnerException.Message });
            }

        }
        [HttpPost]
        public JsonResult EliminaProblemaPantalla(int? IDdataproblema)
        {
            try
            {
                var idPantalla = db.vDataProblemaEncontrado.FirstOrDefault(m => m.IdDataProblemaEncontrado == IDdataproblema).IdPantalla;
                var reg = db.DataProblemaEncontrado.Find(IDdataproblema);
                db.DataProblemaEncontrado.Remove(reg);
                db.SaveChanges();
                return Json(new { success = true, form = "Elimina", msg = "Problema eliminado", url_Tabla = "ProblemaPantalla", url_Controller = "Delegacion", IdPantalla = idPantalla });
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Acccion que le permite cerrar la pantalla de un postulante(se cierra la pantalla por que no hubo problemas en dicha documentacion)
        /// <summary>
        /// Action en donde permite cerrar la pantalla de un postulante, se cierra cuando un postulante no tiene ningun tipo de problemas en el cargado de datos o se abre cuando se necesita cargar problemas a un postulante
        /// </summary>
        /// <param name="id">idPostulante o Idpersona</param>
        /// <param name="IdPanatlla">El Numero de Id de la pantalla de la cual se necesita cerrar</param>
        /// <param name="AoC">AoC Abierto o Cerrar, se necesita enviar un booleano para saber que accion necesita realizar el controlador si abrir o cerra una pantalla</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CerrarPantalla(int id, int IdPanatlla, int AoC)
        {
            var inscrip = db.vInscripcionDetalleUltInsc.FirstOrDefault(m => m.IdPersona == id);/// 06/04/2021 debido a que se envia los botones de validar y abrir a la vista DocumentosNecesarios.cshtml se 
                                                                                                                 ///cambia aqui para que reciba parametro idPersona o IdInscripcion
            var TieneProblema = db.spTieneProblemasEnPantallaEstePostulate(inscrip.IdPersona, IdPanatlla).ToList();
            var Abierto = db.spTildarPantallaParaPostulate(inscrip.IdPersona).FirstOrDefault(m => m.IdPantalla == IdPanatlla);
            var DocuNecesarios = db.DocumentosNecesariosDelInscripto(inscrip.IdInscripcion).ToList();
            var EntregTodo = DocuNecesarios.FirstOrDefault(m => m.Presentado == false && m.Obligatorio == true);/// esta linea de condigo verifica si hay un documento obligatorio no presentado
                                                                                                                ///si es que hay uno te trae que documento Obligatorio falta
            
            try
            {
                if (ModelState.IsValid)
                {
                    if (IdPanatlla == 10 && EntregTodo != null && AoC == 1)/// el if pregunta si la pantalla es 10 el entregatodo no tiene documentacion obligatoria y el AoC=1(Cerrar o validar)
                                                                           ///esta tratando de validar la pantalla 
                    {
                        return Json(new { success = false, msg = "Para validar esta pantalla debe estar toda la documentacion obligatoria presentada" });
                    }
                    if (inscrip.CarreraRelacionada == null && IdPanatlla == 1 && AoC == 1)/// ver la parte de las condiciones debido a que si intenta abrir la pantalla que ya esta abierta y no tiene una carrera relacionada va a devolver el return mostrando un mensaje incorrecto
                    {
                        return Json(new { success = false, msg = "Para validar esta pantalla el postulante debe tener una carrera relacionada" });

                    }
                    if (Abierto.Abierta == true)
                    {
                        if (TieneProblema.ToList().First() == true && AoC == 1)//cerrar la pantalla con problemas cargados
                        {
                            return Json(new { success = false, msg = "No se puede cerrar la ventana por que tiene problemas cargados" });
                        }
                        if (TieneProblema.ToList().First() == false && AoC == 0)//abrir la pantalla sin problemas cargadoos ya estando abierta
                        {
                            return Json(new { success = false, msg = "Esta pantalla ya se encuentra abierta para agregar problemas" });
                        }
                        if (TieneProblema.ToList().First() == true && AoC == 0)///Abrir la pantalla con problemas cargados ya estando abierta
                        {
                            return Json(new { success = false, msg = "Esta pantalla ya se encuentra abierta para agregar problemas" });
                        }
                        if (AoC == 1)// si AoC(Abierto o Cerrado) es True=1 se valida la pantalla(Quiere decir que la pantalla se cierra) y el Usuario(Delegacion) no va a poder agregar Problemas a un Postulante
                        {
                            db.spCierraPantallaDePostulante(IdPanatlla, inscrip.IdPersona, Convert.ToBoolean(AoC));
                            return Json(new { success = true, msg = "Se valido correctamente los datos" });
                        }
                    }
                    else
                    {
                        if (AoC == 0)// si AoC(Abierto o Cerrado) es false=0 se abre la pantalla para que el usuario(Delegacion) pueda serguir agregando problemas a un Postulante 
                        {
                            db.spCierraPantallaDePostulante(IdPanatlla, inscrip.IdPersona, Convert.ToBoolean(AoC));
                            return Json(new { success = true, msg = "Se abrio la pantalla para agregar problemas" });
                        }
                        return Json(new { success = true, msg = "La pantalla ya se encuentra validada, siga validando las siguientes" });
                    }

                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message });
            }

            return Json(new { success = false, msg = "Error en el Modelo Recibido" });
        }

        #endregion

        #region Documentacion Necesaria para el postulante (Get) - se genera una lista con toda la documentacion a presentar por el postulante
        public ActionResult DocumentosNecesarios(int ID_persona)
        {
            try
            {
                //var ExistProblema = db.DataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona && m.IdDataVerificacion == 26).Any();
                //ViewBag.Problem = ExistProblema;
                var inscrip = db.vInscripcionDetalleUltInsc.FirstOrDefault(m => m.IdPersona == ID_persona);

                var DocuNecesarios = db.DocumentosNecesariosDelInscripto(inscrip.IdInscripcion).OrderByDescending(m => m.Obligatorio).ToList();
                ViewBag.Idinscripto = inscrip.IdInscripcion;
                ViewBag.IDPOST = inscrip.IdPersona;
                DocuNecesaria datos = new DocuNecesaria()
                {
                    DocumentosNecesarios = DocuNecesarios
                };
                var listDocu = DocuNecesarios.ToList();
                return PartialView(datos);
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region Documentacion Necesaria para el postulante (POST) boton (Confirmar) Pestaña (Documentacion Presentada)- guarda los registros de que documentacion presento Modificacion 18/02/2021
        [HttpPost]
        public JsonResult DocuNecesarios(string[] select, int? IdInscripto)
        {
            try
            {
                var Inscrip = db.vInscripcionDetalle.FirstOrDefault(m => m.IdInscripcion == IdInscripto);
                var ExistProblema = db.DataProblemaEncontrado.FirstOrDefault(m => m.IdPostulantePersona == Inscrip.IdPersona && m.IdDataVerificacion == 26);


                if (select == null || IdInscripto == null)
                {
                    return Json(new { success = false, msg = "No se a seleccionado ninguna documentacion" });
                }
                else
                {
                    foreach (var item in select)
                    {
                        int x = Convert.ToInt32(item);
                        db.spDocumentoInscripto(Convert.ToBoolean(1), IdInscripto, x, null);
                    }

                    var DocuNecesarios = db.DocumentosNecesariosDelInscripto(IdInscripto).OrderBy(m => m.IdTipoDocPresentado).ToList();
                    var DocuNoPresnt = DocuNecesarios.FirstOrDefault(m => m.Presentado == false & m.Obligatorio == true);///utilizo el filtro para que me traiga la documentacion obligatoria y poder ver si adueda
                                                                                                                         ///verificar que cuando se entrega toda la documentacion entra aca y pincha debido a que trata de borrar un problema inexistente
                    if (DocuNoPresnt==null && ExistProblema==null)
                    {
                        return Json(new { succes = true, msg = "Se agrego correctamente toda la documentacion", form = "ActualizaDocuNec", url_Tabla = "DocumentosNecesarios", url_Controller = "Delegacion" });
                    }
                    if (DocuNoPresnt == null)///utilizo el filtro anterior para verificar si adeuda documentacion obligatoria si - true solamente agrega la documentacion -false cuando toda la documentacion  
                    {
                        db.sp_DataProblemaEncontradoIUD(Inscrip.IdPersona, ExistProblema.IdDataVerificacion, null, ExistProblema.IdDataProblemaEncontrado, true);
                        return Json(new { succes = true, msg = " Toda la doucumentacion ha sido entregada", form = "ActualizaDocuNec", url_Tabla = "DocumentosNecesarios", url_Controller = "Delegacion" });
                    }
                    else
                    {
                        return Json(new { succes = true, msg = "Se agrego correctamente la documentacion", form = "ActualizaDocuNec", url_Tabla = "DocumentosNecesarios", url_Controller = "Delegacion" });
                    }
                }




                //return Json(new { success = true, msg = "Operacon exitosa", form = "ActualizaDocuNec", url_Tabla = "DocumentosNecesarios", url_Controller = "Delegacion" });
                // TODO: Add insert logic here

            }


            catch (System.Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Elimina registros de la documentacion necesaria del postulante
        public JsonResult DelDocuNecesaria(int Idinscripto, int idtipodoc, Boolean esInser)
        {
            try
            {
                db.spDocumentoInscripto(esInser, Idinscripto, idtipodoc, null);
                return Json(new { success = true, msg = "Se elimino la documentacion presentada", form = "ActualizaDocuNec", url_Tabla = "DocumentosNecesarios", url_Controller = "Delegacion" });
            }
            catch (Exception)
            {

                throw;
            }
            //return View("Index");
        }
        #endregion

        #region (GET)Asignar a varios fecha de presentacion - en esta get lleva datos de lugar de establecimiento y los postulantes que estan para asignarle fecha
        [HttpGet]
        public ActionResult AsignarFechaVarios()
        {
            try
            {
                UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                ListadoPostulanteAsignarFecha listadoPostulanteAsignarFecha = new ListadoPostulanteAsignarFecha
                {
                    AsignarFechaVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "Presentacion" 
                                                                                    && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones
                                                                                    && (bool)m.Activa).ToList(),
                    LugarPresentacion = new SelectList(db.EstablecimientoRindeExamenDeOficina(UsuarioDelegacion.IdOficinasYDelegaciones).ToList(), "IdEstablecimientoRindeExamen", "Direccion"),
                    FechaPresentacion = DateTime.Now,
                    listado = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "Presentacion" && m.Estado == "A Asignar" && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList().Count
                };
                var DatosdelLugar = new List<Array>();
                db.EstablecimientoRindeExamen.ToList().ForEach(m => DatosdelLugar.Add(new object[] { m.IdEstablecimientoRindeExamen,
                                                                                               m.Jurisdiccion + ", " + m.Localidad + ", " + m.Departamento,
                                                                                               m.Nombre,
                                                                                               m.Direccion+", "+m.Comentario}));
                listadoPostulanteAsignarFecha.DatosLugar = JsonConvert.SerializeObject(DatosdelLugar);
                return View("AsignarFechaVarios", listadoPostulanteAsignarFecha);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region (POST) Asignar a varios fecha de presentacion -  se seleccion a varios postulantes y se le asignan fechas de presentacion
        [HttpPost]
        public JsonResult AsignarFechaVarios(string[] select, DateTime Fecha, int LugarPresentacion)
        {
            vOficDeleg_EstablecimientoRindExamen establecExamens;
            vInscripcionDetalle Inscripto;
            Configuracion configuracion;
            if (select == null)
            {
                return Json(new { success = false, msg = "Por favor seleccione a uno o varios postulantes" });
            }
            try
            {
                establecExamens = db.vOficDeleg_EstablecimientoRindExamen.FirstOrDefault(m => m.IdEstablecimientoRindeExamen == LugarPresentacion);
                configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo10");
                foreach (var item in select)
                {
                    int x = Convert.ToInt32(item);
                    Inscripto = db.vInscripcionDetalle.FirstOrDefault(m => m.IdInscripcion == x);
                    var callbackUrl = Url.Action("Index", "Postulante", null, protocol: Request.Url.Scheme);
                    var modelPlanti = new ViewModels.MailPresentacion
                    {
                        Apellido = Inscripto.Apellido,/// cambiar a nombre
                        establecimiento = establecExamens,
                        Link = callbackUrl,
                        MailCuerpo = configuracion.ValorDato,
                        fecha = Fecha
                    };
                    db.spProximaSecuenciaEtapaEstado(null, Convert.ToInt32(item), null, null, "PRESENTACION", "Asignada");
                    db.spExamenParaEsteInscripto(Convert.ToInt32(item), Fecha, LugarPresentacion);
                    var Result = Func.EnvioDeMail(modelPlanti, "MailFechaPresentacion", null, Inscripto.IdPersona, "MailAsunto10", null, null);
                }
                return Json(new { success = true, msg = "Se Asigno Correctamente la fecha y lugar de examen" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message });
            }

        }
        #endregion

        #region Asignar fecha de ENTREVISTA a varios Postulante(GET) ENTREVISTA - en esta accion se genera la lista de postulante a la cual se le puede asignar una fecha de entrevista
        [HttpGet]
        public ActionResult AsignarFechaVariosEntrevista()
        {
            UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
            try
            {
                List<vEntrevistaLugarFechaUltInscripc> dato = db.vEntrevistaLugarFechaUltInscripc.Where(m => m.Etapa == "ENTREVISTA" && m.Estado == "A Asignar" && m.Nombre == UsuarioDelegacion.Nombre && (bool)m.Activa ).ToList();
                //ViewBag.Listado = db.vEntrevistaLugarFechaUltInscripc.Where(m => m.Etapa == "ENTREVISTA" && m.Estado == "A Asignar").ToList().Count();/// utilizo un count para saber si me trae un listado con postulante para utlizarlo en la vista
                ViewBag.Listado = dato.Count();
                return View(dato);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Asignar fecha de ENTREVISTA a varios postulantes (POST) - en esta accion se le asigna la fecha a un listado seleccionado de postulante y envia el mail a los postulantes 
        [HttpPost]
        public JsonResult AsignarFechaVariosEntrevista(DateTime Fecha, string[] select)
        {
            try
            {
                if (select == null)
                {
                    return Json(new { success = false, msg = "Por favor seleccione a uno o varios postulantes" });
                }

                vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;
                MailConfirmacionEntrevista Modelo = new MailConfirmacionEntrevista
                {
                    Apellido = "",/// Cambiar por nombre
                    FechaEntrevista = Fecha

                };

                List<string> correos = new List<string>();
                foreach (var item in select)
                {
                    int x = Convert.ToInt32(item);

                    var inscrip = db.Inscripcion.Find(x);
                    inscrip.FechaEntrevista = Fecha;
                    db.SaveChanges();
                    db.spProximaSecuenciaEtapaEstado(0, x, false, 0, "ENTREVISTA", "Asignada");


                    vInscripcionEtapas = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == x);

                    if (vInscripcionEtapas.Estado == "Asignada")
                    {
                        db.spProximaSecuenciaEtapaEstado(0, x, false, 0, "ENTREVISTA", "Pendiente");
                    }
                    //lista de mail de los postulantes
                    correos.Add(inscrip.Postulante.Persona.Email);
                }

                _ = Func.EnvioDeMail(Modelo, "MailConfirmacionEntrevista", null, null, "MailAsunto4", null, correos);
                return Json(new { success = true, msg = "Se Asigno Correctamente la fecha y lugar de examen" });

                //List<vEntrevistaLugarFecha> dato = db.vEntrevistaLugarFecha.Where(m => m.Etapa == "ENTREVISTA" && m.Estado == "A Asignar").ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Accion que permitira mostrar la cantidad de documentacion no presentado o en condiciones para ser aceptadas
        /// <summary>
        /// Sumamente importantes ver como cargar los problemas en dataproblemaencontrado ya que el iddatverificacion tendria que ser otro y seguido de un comentario que diga "Documentacion no presentada"
        ///
        /// FALTA MAS CODIGO/ IDEA ES INSERTAR A LA TABLA DIRECTAMENTE PARA MOSTRAR 1 SOLO ERRORE EN LA PANTALLA Y NOTIFICARLO AL POSTULANTE
        /// </summary>
        /// <param name="IdInscripto"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult NotiDocumentacion(int IdInscripto, int IdPantalla)
        {
            try
            {
                var Postu = db.vInscripcionDetalle.FirstOrDefault(m => m.IdInscripcion == IdInscripto);///traigo los datos de un postulante
                var probleExistente = db.DataProblemaEncontrado.FirstOrDefault(m => m.IdPostulantePersona == Postu.IdPersona && m.IdDataVerificacion == 26);
                var Dataverificacion = db.DataVerificacion.FirstOrDefault(m => m.IdPantalla == IdPantalla && m.Descripcion == "Otros: Aclare");
                var DocuNecesarios = db.DocumentosNecesariosDelInscripto(IdInscripto).OrderBy(m => m.IdTipoDocPresentado).ToList();
                var DocuNoPresnt = DocuNecesarios.FirstOrDefault(m => m.Presentado == false && m.Obligatorio == true);

                if (DocuNoPresnt != null && probleExistente == null)
                {
                    db.sp_DataProblemaEncontradoIUD(Postu.IdPersona, Dataverificacion.IdDataVerificacion, "Documentacion no presentada/Incorrecta", 0, false);
                    return Json(new { succes = true, msg = "Se notificara al postulante que adeuda Documentacion Obligatoria" });
                }
                if (probleExistente != null)
                {
                    return Json(new { succes = true, msg = "Ya se realizo la notificacion al postulante si adeuda documentacion" });
                }
                else
                {
                    return Json(new { succes = true, msg = "Toda la documentacion Obligatoria a sido presentada" });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Accion que actualiza la tabla general de Problemas encontrados en un postulante - esta tabla se encuentra al final del validar datos
        [HttpGet]
        public JsonResult ActuTabla(int IdPersona)
        {
            var Problemas = new List<Array>();
            db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == IdPersona).ToList().ForEach(m => Problemas.Add(new[] { m.DataVerificacion, m.Comentario }));
            var ProblemasJSON = JsonConvert.SerializeObject(Problemas);

            return Json(new { estado = ProblemasJSON }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
