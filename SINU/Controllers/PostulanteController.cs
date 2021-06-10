﻿using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using QRCoder;
using SINU.Authorize;
using SINU.Models;
using SINU.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Web;

namespace SINU.Controllers
{

    [Authorize]
    public class PostulanteController : Controller
    {
        SINUEntities db = new SINUEntities();

        //----------------------------------PAGINA PRINCIPAL----------------------------------------------------------------------//
        //ver este atributo de autorizacion si corresponde o no
        //[Authorize(Roles = "Postulante")]
        public ActionResult Index(int? ID_Postulante)
        {
            try
            {

                IDPersonaVM pers = new IDPersonaVM
                {
                    //cargo id del postulante - el que se recibe, en caso de que sea una consulta, o el ID que se recupera del Postulante logueado
                    ID_PER = ID_Postulante ?? db.Persona.FirstOrDefault(m => m.Email == HttpContext.User.Identity.Name.ToString()).IdPersona,
                };

                pers.OfiDele = db.OficinasYDelegaciones.FirstOrDefault(mbox => mbox.IdOficinasYDelegaciones == db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdDelegacionOficinaIngresoInscribio);
                //verifico el ROL al que pertenece el Usuario alctualmente logueado 
                Session["DeleConsul"] = !HttpContext.User.IsInRole("Postulante");
                //controlo la situacion en la que un postulante podria ver los datos de otro
                if (!(bool)Session["DeleConsul"] && pers.ID_PER != db.Persona.FirstOrDefault(m => m.Email == HttpContext.User.Identity.Name.ToString()).IdPersona)
                {
                    return RedirectToAction("AccionNoAutorizada", "Error");
                }
                //texto declaracion jurada
                ViewBag.TextDeclaracionJurada = db.Configuracion.First(m => m.NombreDato == "TextDeclaracionJurada").ValorDato;

                //recurpero la ultima inscripcion de postulante
                var UltimaInscripcion = db.Inscripcion.Where(m => m.IdPostulantePersona == pers.ID_PER).OrderBy(m => m.FechaInscripcion).ToList().Last();
                
                //cargo los ID de las etapas por las que paso el postulante
                pers.EtapaTabs = db.vPostulanteEtapaEstado.Where(id => id.IdInscripcion == UltimaInscripcion.IdInscripcion).OrderBy(m => m.IdEtapa).DistinctBy(id => id.IdEtapa).Select(id => id.IdEtapa).ToList();

                //cargo esto ID etapas en un string
                pers.EtapaTabs.ForEach(m => pers.IDETAPA += m + ",");

                           
                //control del error al no existir el postulante
                if (UltimaInscripcion == null)
                {
                    Response.StatusCode = 404;
                    return RedirectToAction("NotFound", "Error");
                }

                //cargo nombre de la delegacion correspondiente al postulante
                ViewBag.DelePost = UltimaInscripcion.OficinasYDelegaciones.Nombre;

                //creo array con las secuecias por las que el Postulante transito
                List<int> Secuencias = db.InscripcionEtapaEstado.OrderByDescending(m => m.Fecha).Where(m => m.IdInscripcionEtapaEstado == UltimaInscripcion.IdInscripcion).Select(m => m.IdSecuencia).ToList();
                ViewBag.ULTISECU = Secuencias[0];

                //verifico si se lo postulo o no en la entrevista
                pers.NoPostulado = (Secuencias[0] == 12);

                //ver como mostrar esta pantalla de si su documentacion fue rechazada
                pers.ProcesoInterrumpido = (Secuencias[0] == 24);

                //verifico si la validacion esta en curso o no para el bloqueo de la Pantalla de Documentacion
                ViewBag.ValidacionEnCurso = (Secuencias[0] == 14) /*|| (Secuencias[0] == 24)*/;

                //Boolenao de si paso por validacion
                Session["ValidoUnaVez"] = (Secuencias.IndexOf(14) != -1) && (Secuencias[0] == 13 || Secuencias[0] == 24);

                //Cargo listado con las solapas de documentacion "abiertas o cerradas"
                pers.ListProblemaCantPantalla = new List<Array>();
                
                db.spTildarPantallaParaPostulate(pers.ID_PER).ForEach(m => pers.ListProblemaCantPantalla.Add(new object[] { m.Pantalla, m.Abierta, m.CantComentarios }));
                //pers.ListProblemaCantPantalla = PantallasEstadoProblemas;
                ViewBag.PantallasEstadoProblemas2 = JsonConvert.SerializeObject(pers.ListProblemaCantPantalla);
                                               
                //var FechaFinConvo = db.vConvocatoriaDetalles.FirstOrDefault(m => m.Fecha_Inicio_Proceso <= idInscri.FechaInscripcion && m.Fecha_Fin_Proceso >= idInscri.FechaInscripcion && m.IdInstitucion == idInscri.IdPreferencia && m.IdModalidad == idInscri.IdModalidad).Fecha_Fin_Proceso;
                //var FechaFinConvo = db.vInscriptosYConvocatorias.FirstOrDefault(m => m.IdInscripcion == idInscri.IdInscripcion).Fecha_Fin_Proceso;
                //ViewBag.VenceComvocatoria = DateTime.Now > FechaFinConvo;
                ViewBag.VenceComvocatoria = !db.sp_InvestigaDNI(UltimaInscripcion.Postulante.Persona.DNI).First().Convocatoria_Activa;
                if (!ViewBag.ValidacionEnCurso)
                {
                    ViewBag.ValidacionEnCurso = ViewBag.VenceComvocatoria;
                }

                ViewBag.MOD_CAR = new[] { UltimaInscripcion.IdModalidad, UltimaInscripcion.IdCarreraOficio !=null? db.CarreraOficio.FirstOrDefault(m=>m.IdCarreraOficio== UltimaInscripcion.IdCarreraOficio).CarreraUoficio:null, UltimaInscripcion.IdInscripcion.ToString() };

                
                pers.NomyApe = UltimaInscripcion.Postulante.Persona.Apellido + ", " + UltimaInscripcion.Postulante.Persona.Nombres;

                if (pers.NoPostulado)
                {
                    ViewBag.TextNoAsignado = (db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdPreferencia == 6) ? db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado2").ValorDato : db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado1")
                                            .ValorDato.Replace("$Nombre", pers.NomyApe.ToUpper());
                   
                }
              
                return View(pers);
            }
            catch (Exception ex)
            {

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Postulante", "Index"));
            }

        }
        //----------------------------------DATOS BASICOS----------------------------------------------------------------------//

                   
        [AuthorizacionPermiso("ListarRP")]
        public ActionResult DatosBasicos(int ID_persona)
        {
            try
            {
                //se carga los datos basicos del usuario actual y los utilizados para los dropboxlist
                DatosBasicosVM datosba = new DatosBasicosVM()
                {
                    vPersona_DatosBasicosVM = db.vPersona_DatosBasicos.FirstOrDefault(b => b.IdPersona == ID_persona),
                    SexoVM = db.Sexo.OrderBy(m => m.Descripcion).Where(m => m.Descripcion != "Seleccione Sexo").ToList(),
                    OficinasYDelegacionesVM = db.OficinasYDelegaciones.ToList(),
                    ComoSeEnteroVM = db.ComoSeEntero.Where(n => n.IdComoSeEntero != 1).ToList()
                };

                datosba.Preferenacia = db.Institucion.FirstOrDefault(m => m.IdInstitucion == datosba.vPersona_DatosBasicosVM.IdPreferencia).NombreInst;
                //datosba.vPersona_DatosBasicosVM.Edad = 0;
                //datosba.vPersona_DatosBasicosVM.IdSexo = 0;
                //agrego la opcion de "Necesito Orientacion" en el combo Inctitucion 
                //datosba.vPeriodosInscripsVM.Add(new vPeriodosInscrip() { IdInstitucion= 1,NombreInst="Necesito Orientacion"});
                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Postulante", "DatosBasicos"));
            }

        }


        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS BASICOS"
        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public ActionResult DatosBasicos(DatosBasicosVM Datos)
        {
            if (Datos.vPersona_DatosBasicosVM.ComoSeEntero == null)
            {
                ModelState["vPersona_DatosBasicosVM.ComoSeEntero"].Errors.Clear();
            };

            if (ModelState.IsValid)
            {
                try
                {
                    //se guarda los datos de las persona devueltos
                    vPersona_DatosBasicos p = Datos.vPersona_DatosBasicosVM;

                    int result = db.spDatosBasicosUpdate(p.Apellido, p.Nombres, p.IdSexo, p.DNI, p.Telefono, p.Celular, p.FechaNacimiento, p.Email, p.IdDelegacionOficinaIngresoInscribio, p.ComoSeEntero, p.IdComoSeEntero, p.IdPreferencia, p.IdPersona, p.IdPostulante);

                    //envio nombre de la delegacion en caso de que se haya cambiado, para actualizar los detalles en la parte superior de la vista del "Index/Postulante"
                    string nombreDelegacion = db.OficinasYDelegaciones.Find(p.IdDelegacionOficinaIngresoInscribio).Nombre;
                    return Json(new { success = true, msg = "Datos Guardados.", form = "datosbasicos", delegacion= nombreDelegacion });
                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    //string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" });
                }
            };
            return Json(new { success = false, msg = "Datos no validos, revise los mismos." });
        }

        

        /*--------------------------------------------------------------SOLICITUD DE ENTREVISTA------------------------------------------------------------------------------*/
        [HttpPost]
        [AuthorizacionPermiso("ModificarSecuenciaP")]
        public async Task<JsonResult> SolicitudEntrevistaAsync(int ID_persona)
        {
            try
            {
                var p = db.vPersona_DatosBasicos.First(m => m.IdPersona == ID_persona);
                //ver esto al momento de poner datos basico valido o no valido, creo que deberia ser segun edad y si solo tinee la opcion "necesito orientacion"

                //recurpero la ultima inscripcion
                int idInscrip = db.vPostPersonaEtapaEstadoUltimoEstado.FirstOrDefault(m=>m.IdPersona== ID_persona).IdInscripcionEtapaEstado;

               
                db.spProximaSecuenciaEtapaEstado(p.IdPersona, idInscrip, false, 0, "DATOS BASICOS", (p.Edad <= 35 || p.Edad > 16) ? "Validado": "No Validado");

                //coloco el delay para que las secuencias sean insertadas en distintos tiempos
                await Task.Delay(1000);

                db.spProximaSecuenciaEtapaEstado(ID_persona, idInscrip, false, 0, "ENTREVISTA", "A Asignar");

                //Envio de Mail para notificar a la delegacion correpondiente
                int ID_Delegacion = (int)db.Inscripcion.FirstOrDefault(m => m.IdInscripcion == idInscrip).IdDelegacionOficinaIngresoInscribio;

                SolicitudEntreCorreoPostulante datosMail = new SolicitudEntreCorreoPostulante()
                {
                    Apellido = "",
                    Apellido_P = p.Apellido,
                    Dni_P = p.DNI,
                    IdInscripcion_P = idInscrip,
                    Nombre_P = p.Nombres,
                    url = Url.Action("Index", "Postulante", new { ID_Postulante = p.IdPostulante }, protocol: Request.Url.Scheme),
                    Delegacion = db.OficinasYDelegaciones.Find(ID_Delegacion).Nombre
                };
                var grupoDelegacion = db.vUsuariosAdministrativos.Where(m => m.IdOficinasYDelegaciones == ID_Delegacion).ToList();


                Func.EnvioDeMail(datosMail, "PlantillaMailSolicitudEntrevista", null, null, "MailAsunto8", ID_Delegacion, null);


                return Json(new { success = true, msg = "La Solicitud de Entrevista fue exitosa.", form = "solicitudentrevista" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg=  "Error en la operacion, intentelo nuevamente" }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------------------ENTREVISTA----------------------------------------------------------------------//

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Entrevista(int ID_persona)
        {

            try
            {
                vEntrevistaLugarFechaUltInscripc entrevistafh = new vEntrevistaLugarFechaUltInscripc();
                entrevistafh = db.vEntrevistaLugarFechaUltInscripc.FirstOrDefault(m => m.IdPersona == ID_persona);
                //var idinscrip = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdInscripcion;
                var idinscrip = db.vPostPersonaEtapaEstadoUltimoEstado.First(m => m.IdPersona == ID_persona).IdInscripcionEtapaEstado;
                var IDsecu = db.InscripcionEtapaEstado.Where(m=>m.IdInscripcionEtapaEstado== idinscrip && m.Secuencia_EtapaEstado.IdEtapa == 3).OrderBy(m=>m.Fecha).ToList().Last().IdSecuencia;
                //coloco el estado de la entrevista
                ViewBag.EstadoEntre = (IDsecu == 11) ? "Concretada" : db.vSecuencia_EtapaEstado.FirstOrDefault(m => m.IdSecuencia == IDsecu).Estado;
                ViewBag.FechaAsisgnada = (entrevistafh.FechaEntrevista != null);
                //se carga los texto parametrizados desde la tabla configuracion
                string[] consideraciones = {
                    db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTitulo").ValorDato.ToString(),
                    db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTexto").ValorDato.ToString()
                };
                ViewBag.Considere = consideraciones;

                return PartialView(entrevistafh);
            }
            catch (Exception ex)
            {

                return PartialView(ex);
            }

        }

        //----------------------------------DATOS PERSONALES----------------------------------------------------------------------//


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult DatosPersonales(int ID_persona)
        {
            try
            {

                DatosPersonalesVM datosba = new DatosPersonalesVM()
                {
                    vPersona_DatosPerVM = db.vPersona_DatosPer_UltInscripc.FirstOrDefault(m => m.IdPersona == ID_persona),
                    TipoNacionalidadVM = db.TipoNacionalidad.Where(m => m.IdTipoNacionalidad != 4).ToList(),
                    vEstCivilVM = db.vEstCivil.ToList(),
                    vRELIGIONVM = db.vRELIGION.ToList(),
                    CarreraOficioVm = new List<spCarrerasParaEsteInscripto_Result2>(),
                    ModalidadVm = new List<ComboModalidad>()

                };

                JsonResult result = this.EdadModalidad(ID_persona, datosba.vPersona_DatosPerVM.FechaNacimiento.ToString()) as JsonResult;
                dynamic ResultData = result.Data;

                datosba.ModalidadVm = ResultData.comboModalidad;
                datosba.CarreraOficioVm = ResultData.comboCarrera;

                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }

      

        [AuthorizacionPermiso("ListarRP")]
        public JsonResult EdadModalidad(int? ID_persona, string? FechaNacimiento)
        {
            try
            {
                //DateTime fechaNAC = DateTime.Parse(Fecha);
                //var institutos = db.spRestriccionesParaEstePostulante(IdPOS, fechaNAC, null).DistinctBy(m => m.IdInstitucion).Select(m => new SelectListItem { Value = m.IdInstitucion.ToString(), Text = m.NombreInst }).ToList();
                //institutos.Add(new SelectListItem() { Value = "1", Text = "Necesito Orientacion" });
                //return Json(new { institucion = institutos }, JsonRequestBehavior.AllowGet);


                List<ComboModalidad> ModalidadVm = new List<ComboModalidad>();
                List<spCarrerasParaEsteInscripto_Result2> CarreraOficioVm = new List<spCarrerasParaEsteInscripto_Result2>();

                var Convocatorias = db.spRestriccionesParaEstePostulante(ID_persona,Convert.ToDateTime(FechaNacimiento), null).ToList();
                foreach (var convo in Convocatorias)
                {
                  
                    //cargo modalidades
                    var modalidad = db.vConvocatoriaDetalles.FirstOrDefault(m => m.IdConvocatoria == convo.IdConvocatoria);
                    if (ModalidadVm.FirstOrDefault(m => m.IdModalidad == modalidad.IdModalidad) == null)
                    {
                        ModalidadVm.Add(new ComboModalidad() { IdModalidad = modalidad.IdModalidad, Modalidad = modalidad.Modalidad, EstCivil = convo.IdEstadoCivil });

                    };
                    //cargo carreras
                    var carreras = db.spCarrerasDelGrupo(modalidad.IdGrupoCarrOficio, "").ToList();
                    foreach (var carrera in carreras)
                    {
                        CarreraOficioVm.Add(new spCarrerasParaEsteInscripto_Result2 { IdCarreraOficio = carrera.IdCarreraOficio, CarreraUoficio = carrera.CarreraUoficio, IdModalidad = modalidad.IdModalidad });
                    };
                   
                }
                return Json(new { comboModalidad = ModalidadVm, comboCarrera = CarreraOficioVm }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }

        }

        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS PERSONALES"
        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public ActionResult DatosPersonales(DatosPersonalesVM Datos)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var p = Datos.vPersona_DatosPerVM;
                    var inscrip = db.vPostPersonaEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == p.IdPersona);

                    //verifico si cambio la modalidad par quitar los problemas en 'DataProblemaEncontrado' que tiene, ya que se debe verificar nuevamente
                    if (inscrip.IdModalidad != null && inscrip.IdModalidad != p.IdModalidad)
                    {
                        db.DataProblemaEncontrado.RemoveRange(db.DataProblemaEncontrado.Where(m => m.IdPostulantePersona == p.IdPersona).ToList());
                        db.VerificacionPantallasCerradas.RemoveRange(db.VerificacionPantallasCerradas.Where(m => m.IdPostulantePersona == p.IdPersona).ToList());
                        //como cambio de modalidad y los documentos que se requiere para cada un a es distinta borro los documentos entregados por el postulante VER ESTO
                        db.DocPresentado.RemoveRange(db.DocPresentado.Where(m => m.IdInscripcion == inscrip.IdInscripcionEtapaEstado).ToList());
                        db.SaveChanges();
                    }
                    //Si el id religion en NULL le envio "" corresponde a la religion NINGUNA
                    p.IdReligion ??= "";
                    //busco el nuevo id preferencia para la modalidad seleccionada
                    int IDpreNuevo = db.vInstitucionModalidad.FirstOrDefault(m => m.IdModalidad == p.IdModalidad).IdInstitucion;
                    var msg = new System.Data.Entity.Core.Objects.ObjectParameter("msg","");
                    var result = db.spDatosPersonalesUpdate(p.IdPersona, p.IdInscripcion, p.CUIL, p.FechaNacimiento, p.IdEstadoCivil, p.IdReligion, p.idTipoNacionalidad, p.IdModalidad, p.IdCarreraOficio, IDpreNuevo,p.Nombres,p.Apellido, msg);

                    return Json(new { success = true, msg = "Datos Guardados.", form = "CambiaMOD" });
                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    //string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" });
                }
            }
            return Json(new { success = false, msg = "Datos no validos, revise los mismos." });
        }
        //----------------------------------Antecedentes Penales----------------------------------------------------------------------//


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult DocuPenal(int ID_persona)
        {
            try
            {

                DocuPenalVM d = new DocuPenalVM()
                {
                    IdPersona = ID_persona

                };

                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string carpetaLink = "../Documentacion/ArchivosDocuPenal/";
                string archivo = ID_persona + "*";

                //obtengo los archivos (certificado de antecedentes penales y autorizacion para investigacion), correspondiente al postulante
                string[] archivos = Directory.GetFiles(CarpetaDeGuardado, archivo);
                foreach (var item in archivos)
                {
                    if (item.IndexOf("Anexo2") > 0) d.PathFormularioAanexo2 = carpetaLink + item.Substring(item.LastIndexOf("\\") + 1);
                    if (item.IndexOf("Certificado") > 0) d.PathConstanciaAntcPenales = carpetaLink + item.Substring(item.LastIndexOf("\\") + 1);
                }
                
                //ultimo id inscripcion del poos
                var idInscrip = db.vPostPersonaEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == ID_persona).IdInscripcionEtapaEstado;

                //registro de la declaracion jurada, segun ultima inscripcion
                d.PenalDeclaJurada = db.DeclaracionJurada.FirstOrDefault(m => m.IdInscripcion == idInscrip) ?? new DeclaracionJurada { IdInscripcion= idInscrip };
                d.PenalDeclaJurada.IdPersona = ID_persona;
                return PartialView(d);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public JsonResult DocuPenal(DocuPenalVM data)
        {
            try
            {
                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string anexo = "", cert = "";
                string NombreArchivo, ExtencioArchivo, guarda;
                bool btanexo = false, btcert = false;
                int id = data.IdPersona;
                string[] archivos = Directory.GetFiles(CarpetaDeGuardado, id + "*");
                foreach (var item in archivos)
                {
                    if (item.IndexOf("Anexo2") > 0) anexo = item;
                    if (item.IndexOf("Certificado") > 0) cert = item;
                }

                if (data.FormularioAanexo2 != null)
                {
                    if (anexo != "") System.IO.File.Delete(anexo);
                    NombreArchivo = id + "&Anexo2";
                    ExtencioArchivo = Path.GetExtension(data.FormularioAanexo2.FileName);
                    guarda = CarpetaDeGuardado + NombreArchivo + "&" + data.FormularioAanexo2.FileName;
                    data.FormularioAanexo2.SaveAs(guarda);
                    btanexo = true;
                }
                if (data.ConstanciaAntcPenales != null)
                {
                    if (cert != "") System.IO.File.Delete(cert);
                    NombreArchivo = id + "&Certificado";
                    ExtencioArchivo = Path.GetExtension(data.ConstanciaAntcPenales.FileName);
                    guarda = CarpetaDeGuardado + NombreArchivo + "&" + data.ConstanciaAntcPenales.FileName;
                    data.ConstanciaAntcPenales.SaveAs(guarda);
                    btcert = true;
                }
                //Declaracion Jurada
                DeclaracionJurada DeclaJura = data.PenalDeclaJurada;
                if (DeclaJura.IdDeclaracionJurada == 0)
                {
                    
                    db.DeclaracionJurada.Add(DeclaJura);
                }
                else
                {
                    var declaju = new DeclaracionJurada() { IdDeclaracionJurada = DeclaJura.IdDeclaracionJurada };

                    if (TryUpdateModel(DeclaJura, "",
                       new string[] { "PoseeAntecedentes", "Antecedentes_Detalles", "EsAdicto", "Comentario", "IdInscripcion" }))
                    {

                        db.Entry(DeclaJura).State = EntityState.Modified;
                        
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, form = "DocuPenal", msg = "Datos Guardados.", anexo = btanexo, cert = btcert }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Datos no validos, revise los mismos." }, JsonRequestBehavior.AllowGet);
            }

        }


        //public bool ModificaNombre(DocuPenalVM data)
        //{
        //    string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
        //    string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
        //    string anexo = "" , cert = "";
        //    string anexoNew = "", certNew = "";

        //    string NombreArchivo, ExtencioArchivo, guarda, idInscripcion, idPostulante;
        //    string[] archivos;

        //    //levanto los registros de la ultima inscripcion de los postulantes
        //    var ListaInscriptos = db.vInscripcionDetalleUltInsc.Select(m => new  { idInscripcion = m.IdInscripcion.ToString(), idPostulante = m.IdPersona.ToString() });

        //    foreach (var inscripcion in ListaInscriptos)
        //    {
        //        //busco si el postulante posee registros
        //        archivos = Directory.GetFiles(CarpetaDeGuardado, inscripcion.idPostulante + "*");
        //        foreach (var item in archivos)
        //        {
        //            if (item.IndexOf("Anexo2") > 0) {
        //                anexo = item;
        //                anexoNew = item.Replace(inscripcion.idPostulante, inscripcion.idInscripcion);
        //                System.IO.File.Move(anexo, anexoNew);

        //            }
        //            if (item.IndexOf("Certificado") > 0) {
        //                cert = item;
        //                certNew = item.Replace(inscripcion.idPostulante, inscripcion.idInscripcion);
        //                System.IO.File.Move(cert, certNew);

        //            }
        //        }




        //    }

          
        //    return true;
        //}

        [AuthorizacionPermiso("ListarRP")]
        public FileContentResult GetFile(int? ID_persona, string? docu)
        {
            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;

            if (docu == null)
            {
                string UbicacionPDF = $"{ubicacion}Documentacion\\ANEXO 2 A LA SOLICITUD DE INGRESO.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(UbicacionPDF);
                //el tercer para obligar la descarga del archivo
                return File(FileBytes, "application/pdf", "Formulario-AUTORIZACIÓN PARA REQUERIR ANTECEDENTES PENALES.pdf");
            }
            else
            {

                string Ubicacionfile = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string[] archivos = Directory.GetFiles(Ubicacionfile, ID_persona + "&" + docu + "*");
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
                };
                return File(FileBytes, app);
            }
        }

        //----------------------------------Domicilio----------------------------------------------------------------------//


        public ActionResult Domicio_API(int ID_persona)
        {

            Domiciolio_API domi = new Domiciolio_API()
            {
                vPersona_Domicilio_API = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == ID_persona),
                Pais_API = db.sp_vPaises("").OrderBy(m => m.DESCRIPCION).Select(m => new SelectListItem { Text = m.DESCRIPCION, Value = m.CODIGO }).ToList(),
                Provincia_API = new List<SelectListItem>(),
                Localidad_API = new List<SelectListItem>()

            };
            //cargo liatado con los viajes del postulante
            var viajes = db.PostulanteViaje.Where(m => m.IdPostulantePersona == ID_persona).ToList();
            domi.PostulanteViajeListaVM = new List<SelectListItem>();
            foreach (var viaje in viajes)
            {
                SelectListItem item = new SelectListItem { Text = db.sp_vPaises(viaje.codigovPais).First().DESCRIPCION + ", " + viaje.FechaInicioviaje.Value.ToShortDateString(), Value = viaje.IdPostulanteViaje.ToString() };
                domi.PostulanteViajeListaVM.Add(item);
            }

            ViewBag.Paises1 = new SelectList(db.sp_vPaises(""), "CODIGO", "DESCRIPCION");
            return View(domi);
        }


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Domicilio(int ID_persona)
        {
            try
            {
                DomicilioVM datosdomilio = new DomicilioVM()
                {
                    vPersona_DomicilioVM = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == ID_persona),
                    sp_vPaises_ResultVM = db.sp_vPaises("").OrderBy(m => m.DESCRIPCION).ToList(),
                    provincias = db.vProvincia_Depto_Localidad.OrderBy(m => m.Provincia).Select(m => new SelectListItem { Value = m.Provincia, Text = m.Provincia }).DistinctBy(m => m.Text).ToList(),
                    PostulanteViajeListaVM = new List<SelectListItem>()

                };
                //cargo liatado con los viajes del postulante
                var viajes = db.PostulanteViaje.Where(m => m.IdPostulantePersona == ID_persona).ToList();

                foreach (var viaje in viajes)
                {
                    SelectListItem item = new SelectListItem { Text = db.sp_vPaises(viaje.codigovPais).First().DESCRIPCION + ", " + viaje.FechaInicioviaje.Value.ToShortDateString(), Value = viaje.IdPostulanteViaje.ToString() };
                    datosdomilio.PostulanteViajeListaVM.Add(item);
                }

                ViewBag.Paises1 = new SelectList(db.sp_vPaises(""), "CODIGO", "DESCRIPCION");


                //datosdomilio.vPersona_DomicilioVM.IdpersonaPostu=()

                if (datosdomilio.vPersona_DomicilioVM.IdPais != "AR")
                {
                    string[] domiextR = datosdomilio.vPersona_DomicilioVM.Prov_Loc_CP.Split('-');
                    datosdomilio.vPersona_DomicilioVM.TBoxProvincia = domiextR[0];
                    datosdomilio.vPersona_DomicilioVM.Localidad = domiextR[1];
                    datosdomilio.vPersona_DomicilioVM.CODIGO_POSTAL = domiextR[2];
                }
                else if (datosdomilio.vPersona_DomicilioVM.IdLocalidad == 20819)
                {
                    datosdomilio.vPersona_DomicilioVM.CODIGO_POSTAL = datosdomilio.vPersona_DomicilioVM.Prov_Loc_CP;
                };

                if (datosdomilio.vPersona_DomicilioVM.EventualIdPais != "AR")
                {
                    string[] domiextE = datosdomilio.vPersona_DomicilioVM.EventualProv_Loc.Split('-');
                    datosdomilio.vPersona_DomicilioVM.TBoxEventualProvincia = domiextE[0];
                    datosdomilio.vPersona_DomicilioVM.EventualLocalidad = domiextE[1];
                    datosdomilio.vPersona_DomicilioVM.EventualCodigo_Postal = domiextE[2];
                }
                else if (datosdomilio.vPersona_DomicilioVM.EventualIdLocalidad == 20819)
                {
                    datosdomilio.vPersona_DomicilioVM.EventualCodigo_Postal = datosdomilio.vPersona_DomicilioVM.EventualProv_Loc;
                };

                datosdomilio.vProvincia_Depto_LocalidadREALVM = (datosdomilio.vPersona_DomicilioVM.IdLocalidad != null) ?
                    db.vProvincia_Depto_Localidad.Where(m => m.Provincia == datosdomilio.vPersona_DomicilioVM.Provincia).ToList()
                  : new List<vProvincia_Depto_Localidad>();

                datosdomilio.vProvincia_Depto_LocalidadEVENTUALVM = (datosdomilio.vPersona_DomicilioVM.EventualIdLocalidad != null) ?
                    db.vProvincia_Depto_Localidad.Where(m => m.Provincia == datosdomilio.vPersona_DomicilioVM.EventualProvincia).ToList()
                  : new List<vProvincia_Depto_Localidad>();

                return PartialView(datosdomilio);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }


        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS PERSONALES"
        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public JsonResult Domicilio(DomicilioVM Datos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var p = Datos.vPersona_DomicilioVM;
                    if (p.IdPais != "AR")
                    {
                        p.IdLocalidad = null;
                        p.Prov_Loc_CP = p.TBoxProvincia + "-" + p.Localidad + "-" + p.CODIGO_POSTAL;
                    }
                    else
                    {
                        if (p.IdLocalidad == 20819)
                        {
                            p.Prov_Loc_CP = p.CODIGO_POSTAL;
                        }
                        else
                        {
                            p.Prov_Loc_CP = null;
                        };

                    };

                    if (p.EventualIdPais != "AR")
                    {
                        p.EventualIdLocalidad = null;
                        p.EventualProv_Loc = p.TBoxEventualProvincia + "-" + p.EventualLocalidad + "-" + p.EventualCodigo_Postal;
                    }
                    else
                    {
                        if (p.EventualIdLocalidad == 20819)
                        {
                            p.EventualProv_Loc = p.EventualCodigo_Postal;
                        }
                        else
                        {
                            p.EventualProv_Loc = null;
                        };
                    };

                    db.spDomiciliosU(
                        p.IdDomicilioDNI, p.Calle, p.Numero, p.Piso, p.Unidad, p.IdLocalidad, p.Prov_Loc_CP, p.IdPais,
                        p.IdDomicilioActual, p.EventualCalle, p.EventualNumero, p.EventualPiso, p.EventualUnidad, p.EventualIdLocalidad, p.EventualProv_Loc, p.EventualIdPais
                    );

                    return Json(new { success = true, msg = "Datos Guardados" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    //string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = false, msg = "Datos no validos, revise los mismos." }, JsonRequestBehavior.AllowGet);
        }

        //Cargo el DropBoxList de localidad,segun Provincia Seleccionado o Cp segun Localidad Seleccionada
        public JsonResult DropEnCascadaDomicilio(string? Provincia, int? Localidad)
        {
            if (Provincia != null)
            {
                var localidades = db.vProvincia_Depto_Localidad
                                .Where(m => m.Provincia == Provincia)
                                .Select(m => new SelectListItem
                                {
                                    Value = m.IdLocalidad.ToString(),
                                    Text = m.Localidad
                                })
                                .OrderBy(m => m.Text)
                                .ToList();
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            else if (Localidad != null)
            {
                string Value = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).IdLocalidad.ToString();
                string Text = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).CODIGO_POSTAL;

                return Json(new { Value, Text }, JsonRequestBehavior.AllowGet);
            };
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        //----------------------------------Estudios----------------------------------------------------------------------//

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Estudios(int ID_persona)
        {
            try
            {
                EstudiosVM estudio = new EstudiosVM
                {
                    vPersona_EstudioListVM = db.VPersona_Estudio.Where(m => m.IdPersona == ID_persona).ToList()
                };
                foreach (var item in estudio.vPersona_EstudioListVM)
                {
                    if (item.IdInstitutos == 0)
                    {
                        string[] paisyinst = item.NombreYPaisInstituto.Split('-');
                        item.Jurisdiccion = paisyinst[0];
                        item.Nombre = paisyinst[1];
                    }
                }
                return PartialView(estudio);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult EstudiosCUD(int ID_persona, int? ID)
        {
            try
            {
                EstudiosVM estudio = new EstudiosVM()
                {
                    NivelEstudioVM = db.NiveldEstudio.ToList(),
                    //cargo los datos para el combobox de provincia
                    Provincia = db.Institutos
                        .DistinctBy(m => m.Jurisdiccion)
                        .OrderBy(m => m.Jurisdiccion)
                        .Select(m => new SelectListItem { Text = m.Jurisdiccion, Value = m.Jurisdiccion })
                        .ToList()
                };
                //verifico si envio un ID 
                //SI reciobio ID cargo los datos correspondiente
                //caso contrario envio un nuevo registro de Estudio
                if (ID != null)

                {
                    estudio.vPersona_Estudioidvm = db.VPersona_Estudio.FirstOrDefault(m => m.IdEstudio == ID);

                    //Si IdInstituto existe se carga los datos relacionados con el, provincia localidad y nombre de institutos
                    //de NO existir se carga datos del campo "NombreYPaisInstituto"

                    if (estudio.vPersona_Estudioidvm.IdInstitutos != 0)
                    {
                        estudio.Localidad = db.Institutos
                            .Where(m => m.Jurisdiccion == estudio.vPersona_Estudioidvm.Jurisdiccion)
                            .DistinctBy(m => m.Localidad)
                            .OrderBy(m => m.Localidad)
                            .Select(m => new SelectListItem { Value = m.Localidad, Text = m.Localidad })
                            .ToList();
                        estudio.InstitutoVM = db.Institutos
                            .Where(m => m.Localidad == estudio.vPersona_Estudioidvm.Localidad)
                            .OrderBy(m => m.Nombre)
                            .Select(m => new SelectListItem
                            {
                                Value = m.Id.ToString(),
                                Text = m.Nombre
                            })
                            .ToList();
                        estudio.InstitutoVM.Add(new SelectListItem { Value = "0", Text = "Otro" });
                        estudio.InstitutoVM = estudio.InstitutoVM.OrderBy(m => m.Value).ToList();
                        estudio.vPersona_Estudioidvm.INST_EXT = false;
                        estudio.vPersona_Estudioidvm.Nombre = "";
                    }
                    else
                    {

                        if (estudio.vPersona_Estudioidvm.NombreYPaisInstituto[0] == 'O')
                        {
                            var proloc = estudio.vPersona_Estudioidvm.NombreYPaisInstituto.Split('-');
                            estudio.vPersona_Estudioidvm.Jurisdiccion = proloc[2];
                            estudio.vPersona_Estudioidvm.Localidad = proloc[3];
                            //cargo las localidades que corresponde a jujuy
                            estudio.Localidad = db.Institutos
                                .Where(m => m.Jurisdiccion == estudio.vPersona_Estudioidvm.Jurisdiccion)
                                .DistinctBy(m => m.Localidad)
                                .OrderBy(m => m.Localidad)
                                .Select(m => new SelectListItem { Value = m.Localidad, Text = m.Localidad })
                                .ToList();
                            estudio.vPersona_Estudioidvm.otro_inst = proloc[1];
                            //cargo loso institutos correspondiente de la localidad

                            estudio.vPersona_Estudioidvm.IdInstitutos = 0;
                            estudio.InstitutoVM = db.Institutos
                               .Where(m => m.Localidad == estudio.vPersona_Estudioidvm.Localidad)
                               .OrderBy(m => m.Nombre)
                               .Select(m => new SelectListItem
                               {
                                   Value = m.Id.ToString(),
                                   Text = m.Nombre
                               })
                               .ToList();
                            estudio.InstitutoVM.Add(new SelectListItem { Text = "Otro", Value = "0" });
                            estudio.InstitutoVM = estudio.InstitutoVM.OrderBy(m => m.Value).ToList();
                            estudio.vPersona_Estudioidvm.INST_EXT = false;
                            estudio.vPersona_Estudioidvm.Nombre = "";

                        }
                        else
                        {

                            string[] paisinst = estudio.vPersona_Estudioidvm.NombreYPaisInstituto.Split('-');
                            estudio.vPersona_Estudioidvm.Jurisdiccion = paisinst[0];
                            estudio.vPersona_Estudioidvm.Nombre = paisinst[1];
                            estudio.Localidad = new List<SelectListItem>();
                            estudio.InstitutoVM = new List<SelectListItem>();
                            estudio.vPersona_Estudioidvm.INST_EXT = true;
                        }

                    }
                }
                else
                {
                    VPersona_Estudio nuevoestu = new VPersona_Estudio()
                    {
                        IdPersona = ID_persona,
                        NombreYPaisInstituto = "-",
                        INST_EXT = false,
                        Completo = true,
                        CursandoUltimoAnio = true,
                        Localidad = "",
                        Jurisdiccion = ""

                    };

                    estudio.vPersona_Estudioidvm = nuevoestu;
                    estudio.Localidad = new List<SelectListItem>();
                    estudio.InstitutoVM = new List<SelectListItem>();
                };
                return PartialView(estudio);

            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }

        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public ActionResult EstudiosCUD(EstudiosVM Datos)
        {

            ModelState["vPersona_Estudioidvm.Jurisdiccion"].Errors.Clear();
            ModelState["vPersona_Estudioidvm.otro_inst"].Errors.Clear();
            ModelState["vPersona_Estudioidvm.Localidad"].Errors.Clear();
            ModelState["vPersona_Estudioidvm.CantidadMateriaAdeudadas"].Errors.Clear();
            ModelState["vPersona_Estudioidvm.ultimoAnioCursado"].Errors.Clear();
            if (ModelState.Keys.Contains("vPersona_Estudioidvm.IdInstitutos"))
            {
                ModelState["vPersona_Estudioidvm.IdInstitutos"].Errors.Clear();

            }

            if (ModelState.IsValid)
            {
                try
                {
                    var e = Datos.vPersona_Estudioidvm;
                    if (e.otro_inst != null)
                    {
                        e.NombreYPaisInstituto = "O-" + e.otro_inst + "-" + e.prov_localidad;
                        e.IdInstitutos = 0;

                    }
                    else if (e.IdInstitutos != 0)
                    {
                        e.prov_localidad = null;
                        e.NombreYPaisInstituto = null;

                    }
                    else
                    {
                        //e.IdInstitutos = 0;
                        e.NombreYPaisInstituto = e.Jurisdiccion + "-" + e.Nombre;
                    }
                    if (!e.Completo)
                    {
                        e.Promedio = null;
                        //e.ultimoAnioCursado = null;
                    }
                    db.spEstudiosIU(e.IdEstudio, e.IdPersona, e.Titulo, e.Completo, e.IdNiveldEstudio, e.IdInstitutos, e.Promedio, e.CantidadMateriaAdeudadas, e.ultimoAnioCursado, e.NombreYPaisInstituto, e.CursandoUltimoAnio);

                    return Json(new { success = true, msg = "Estudio agregado correctamente." });
                }
                catch (Exception ex)
                {
                    //revisar como mostrar error en la vista
                    return Json(new { success = false, msg="Error en la operacion, intentelo nuevamente" });
                }
            }
            return Json(new { success = false, msg = "Datos no validos, revise los mismos." });

        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaEST(int IdPersona, int IDEstudio)
        {
            try
            {
                var estu = db.Estudio.Find(IDEstudio);
                if (estu != null)
                {
                    db.spEstudiosEliminar(IDEstudio);
                    //success: es true cundo la operacion es exitosa
                    //msg:mensjae que figurara en el modal
                    //form: se ejecutar un accion en un switch del script de la vista index
                    //url_Tabla: es el nombre de la accion y colocandole el subfijo NAV es el contenedor de la tabla actual
                    return Json(new { success = true, msg = "Eliminacion Exitosa.", form = "Elimina", url_Tabla = "Estudios", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente", JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente", JsonRequestBehavior.AllowGet });
            }
        }


        public JsonResult DropCascadaEST(int opc, string val)
        {
            if (opc == 0)
            {
                var result = db.Institutos
                                 .Where(m => m.Jurisdiccion == val)
                                 .DistinctBy(m => m.Localidad)
                                 .Select(m => new SelectListItem
                                 {
                                     Value = m.Localidad,
                                     Text = m.Localidad
                                 })
                                 .OrderBy(m => m.Text)
                                 .ToList();


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //recibo 2 parametroe en val uno pára privincia y otro es localidad
                string[] valpro = val.Split('-');
                string juri = valpro[0];
                string loca = valpro[1];
                var result = db.Institutos
                                 .Where(m => m.Jurisdiccion == juri)
                                 .Where(m => m.Localidad == loca)
                                 .Select(m => new SelectListItem
                                 {
                                     Value = m.Id.ToString(),
                                     Text = m.Nombre.Substring(0, 45)//ver para truncar esto aqui o en el cliente
                                 })
                                 .OrderBy(m => m.Text)
                                 .ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        /*--------------------------------------------------------------IDIOMAS-------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Idiomas(int ID_persona)
        {
            try
            {
                List<vPersona_Idioma> idioma;
                idioma = db.vPersona_Idioma.Where(m => m.IdPersona == ID_persona).ToList();
                //ver si enviar por viewbag o generar un view model
                ViewBag.Id = ID_persona;
                return PartialView(idioma);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult IdiomaCUD(int? ID_persona, int? ID)
        {
            try
            {
                IdiomasVM idioma = new IdiomasVM()
                {
                    NivelIdiomaVM = db.NivelIdioma.ToList(),
                    Sp_VIdiomas_VM = db.sp_vIdiomas("").Where(m=>m.CODIGO!= "S/IDIOMA").ToList()
                };

                if (ID != null)
                {
                    idioma.VPersona_IdiomaIdVM = db.vPersona_Idioma.FirstOrDefault(m => m.IdPersonaIdioma == ID);
                }
                else
                {
                    idioma.VPersona_IdiomaIdVM = new vPersona_Idioma()
                    {
                        IdPersonaIdioma = 0,
                        IdPersona = ID_persona
                    };
                }

                return PartialView(idioma);

            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public JsonResult IdiomaCUD(IdiomasVM datos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    vPersona_Idioma i = datos.VPersona_IdiomaIdVM;
                    db.spIdiomasIU(i.IdPersonaIdioma, i.IdPersona, i.CodIdioma, i.Habla, i.Lee, i.Escribe);
                    return Json(new { success = true, msg = "Idioma guardado correctamente." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    return Json(new { success = false, msg = "ERROR, refresque la pagina e intentelo de nuevo." }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, msg = "Datos no validos, revise los mismos." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult SinIdioma(int idper)
        {
         
            try
            {
                //llamo este sp para indicar el que no maneja otro idioma y le envio el codigo correspondiente al "SIN IDIOMA"
                db.spIdiomasIU(0, idper, "S/IDIOMA" , 1,1, 1);
                return Json(new { success = true, msg = "Sin idioma extranjero seleccionado.", form = "Elimina", url_Tabla = "Idiomas", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "ERROR, refresque la pagina e intentelo de nuevo." }, JsonRequestBehavior.AllowGet);
            }
            
        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaIDIO(int IdPersona, int IDIdio)
        {
            try
            {
                var regidioma = db.PersonaIdioma.FirstOrDefault(m => m.IdPersonaIdioma == IDIdio);
                db.PersonaIdioma.Remove(regidioma);
               
                db.SaveChanges();
                return Json(new { success = true, msg = "Eliminacion Exitosa.", form = "Elimina", url_Tabla = "Idiomas", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" });
            }
        }

        /*--------------------------------------------------------------ACTIVIDAD MILITAR-------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult ActMilitar(int ID_persona)
        {
            try
            {
                ViewBag.Id = ID_persona;
                List<vPersona_ActividadMilitar> LISTactividad;
                LISTactividad = db.vPersona_ActividadMilitar.Where(m => m.IdPersona == ID_persona).ToList();

                return PartialView(LISTactividad);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult ActMilitarCUD(int ID_persona, int? ID)
        {
            try
            {
                ActividadMIlitarVM actividad = new ActividadMIlitarVM()
                {
                    FuerzasVM = db.Fuerza.Where(m => m.IdFuerza != 14).ToList(),
                    IDPErsona = ID_persona,
                    BajaVM = db.Baja.ToList(),
                    SituacionRevistaVM = db.SituacionRevista.ToList()
                };
                if (db.Postulante.FirstOrDefault(m => m.IdPersona == ID_persona) != null)
                {
                    actividad.SituacionRevistaVM.Remove(db.SituacionRevista.First(m => m.SituacionRevista1 == "Retirado"));
                };
                if (ID != null)
                {
                    actividad.ACTMilitarIDVM = db.ActividadMilitar.FirstOrDefault(m => m.IdActividadMilitar == ID);
                }
                else
                {
                    actividad.ACTMilitarIDVM = new ActividadMilitar()
                    {
                        IdActividadMilitar = 0,
                    };
                }
                return PartialView(actividad);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public JsonResult ActMilitarCUD(ActividadMIlitarVM datos)
        {

            try
            {
                var situreserva = new[] { 2, 1 };
                if (situreserva.Contains(datos.ACTMilitarIDVM.IdSituacionRevista) || !(bool)datos.ACTMilitarIDVM.Ingreso)
                {
                    ModelState["ACTMilitarIDVM.FechaBaja"].Errors.Clear();
                    ModelState["ACTMilitarIDVM.MotivoBaja"].Errors.Clear();
                    ModelState["ACTMilitarIDVM.IdBaja"].Errors.Clear();
                    ModelState["ACTMilitarIDVM.IdSituacionRevista"].Errors.Clear();
                }



                if (ModelState.IsValid)
                {
                    var a = datos.ACTMilitarIDVM;
                    if (a.Ingreso == true)
                    {
                        a.CausaMotivoNoingreso = null;
                        if (a.IdSituacionRevista == 2)
                        {
                            a.FechaBaja = null;
                            a.IdBaja = 0;
                            a.MotivoBaja = "";
                        }

                    }
                    else
                    {
                        a.FechaBaja = null;
                        a.FechaIngreso = null;
                        a.MotivoBaja = null;
                        a.Jerarquia = null;
                        a.Cargo = null;
                        a.Destino = null;
                        a.IdSituacionRevista = 0;
                        a.IdBaja = 0;
                    }

                    db.spActividadMilitarIU(a.IdActividadMilitar, datos.IDPErsona, a.Ingreso, a.FechaIngreso, a.FechaBaja, a.CausaMotivoNoingreso, a.MotivoBaja, a.Jerarquia, a.Cargo, a.Destino, a.IdSituacionRevista, a.IdFuerza, a.IdBaja);
                    return Json(new { success = true, msg = "Actividad Militar agregada correctamente." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, msg = "Datos no validos, revise los mismos." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "ERROR, refresque la pagina e intentelo nuevamente." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult SinActMilitar(int idper)
        {

            try
            {
                //llamo este sp para indicar el que no maneja otro idioma y le envio el codigo correspondiente al "SIN IDIOMA"
                db.spActividadMilitarIU(0, idper, null, null, null, "", "", "", "", "", 0, 14, null);
                return Json(new { success = false, msg = "Sin actividad militar seleccionado.", form = "Elimina", url_Tabla = "ActMilitar", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = true, msg = "Error en la operacion, intentelo nuevamente" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaACT(int IdPersona, int IDActMil)
        {
            try
            {
                db.spActividadMilitarEliminar(IDActMil);
                return Json(new { success = false, msg = "Eliminacion Exitosa.", form = "Elimina", url_Tabla = "ActMilitar", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = true, msg = "Error en la operacion, intentelo nuevamente" }, JsonRequestBehavior.AllowGet);
            }
        }

        /*--------------------------------------------------------------SITUACION OCUPACIONAL-------------------------------------------------------------------------------*/


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult SituOcupacional(int ID_persona)
        {
            try
            {

                SituacionOcupacionalVM SituOcu = new SituacionOcupacionalVM
                {
                    EstadoDescripcionVM = new SelectList(db.EstadoOcupacional.Where(m => m.Descripcion != "Jubilado" & m.Descripcion != "Retirado").ToList(), "Id", "Descripcion", "EstadoOcupacional1", 1),

                };



                List<string> InteresesSeleccionados = db.Persona.Find(ID_persona).Interes.Select(m => m.DescInteres).ToList();
                SituOcu.InteresesVM = db.Interes.Select(c => new SelectListItem { Text = c.DescInteres, Value = c.IdInteres.ToString(), Selected = InteresesSeleccionados.Contains(c.DescInteres) }).ToList();

                var situ = db.vPersona_SituacionOcupacional.FirstOrDefault(m => m.IdPersona == ID_persona);
                SituOcu.VPersona_SituacionOcupacionalVM = situ == null
                    ? new vPersona_SituacionOcupacional()
                    {
                        IdSituacionOcupacional = 0,
                        IdPersona = ID_persona,
                    }
                    : situ; ;

                return PartialView(SituOcu);
            }
            catch (Exception ex)
            {
                return PartialView(ex);
            }
        }

        [HttpPost]
        //[AuthorizacionPermiso("CreaEditaDatosP")]
        public ActionResult ViajesPostulante(int? idper, string? idpais, DateTime? fechaviaje, int? idviaje)
        {
            try
            {
                object resp = new { };
                if (idviaje == null)
                {
                    PostulanteViaje v = new PostulanteViaje { codigovPais = idpais, Comentario = "", IdPostulantePersona = (int)idper, FechaInicioviaje = fechaviaje };
                    db.PostulanteViaje.Add(v);
                    db.SaveChanges();
                    string pais = db.sp_vPaises(idpais).First().DESCRIPCION;
                    resp = new { text = pais + ", " + ((DateTime)fechaviaje).ToString("dd/MM/yyyy"), value = v.IdPostulanteViaje };
                }
                else
                {
                    var viaje = db.PostulanteViaje.FirstOrDefault(m => m.IdPostulanteViaje == idviaje);
                    db.PostulanteViaje.Remove(viaje);
                    db.SaveChanges();
                    resp = new { eli = true };
                }

                return Json(resp);
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public ActionResult SituOcupacional(SituacionOcupacionalVM situ)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var s = situ.VPersona_SituacionOcupacionalVM;
                    db.spSituacionOcupacionalIU(s.IdSituacionOcupacional, s.IdPersona, s.IdEstadoOcupacional, s.OcupacionActual, s.Oficio, s.AniosTrabajados, s.DomicilioLaboral);

                    Interes ins = new Interes();
                    var per = db.Persona.Find(s.IdPersona).Interes;
                    per.Clear();
                    if (situ.IdInteres != null)
                    {
                        foreach (var item in situ.IdInteres)
                        {
                            ins = db.Interes.Find(Double.Parse(item));
                            per.Add(ins);
                        };
                    };
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Datos Guardados" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" });
                }
            }
            return Json(new { success = false, msg = "Datos no validos, revise los mismos." });

        }

        /*--------------------------------------------------------------Antropometria------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Antropometria(int ID_persona)
        {
            vPersona_Antropometria antropo = db.vPersona_Antropometria.FirstOrDefault(m => m.IdPersona == ID_persona);
            return PartialView(antropo);
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public ActionResult Antropometria(vPersona_Antropometria a)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    a.LargoFalda ??= 0;
                    db.spAntropometriaIU(a.IdPersona, a.Altura, a.Peso, a.IMC, a.PerimCabeza, a.PerimTorax, a.PerimCintura, a.PerimCaderas, a.LargoPantalon, a.LargoEntrep, a.LargoFalda, a.Cuello, a.Calzado);
                    return Json(new { success = true, msg = "Datos Guardados." }, JsonRequestBehavior.AllowGet);

                }
                return Json(new { success = true, msg = "Datos no validos, revise los mismos." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" });
            }

        }

        public JsonResult VerificaAltIcm(int IdPostulante, string AltIcm, float num)
        {
            try
            {
                var p = db.Persona.FirstOrDefault(m => m.IdPersona == IdPostulante);
                var UltimaInscripcion = db.vPostPersonaEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == IdPostulante);
                var FechaNac = p.FechaNacimiento;
                object sexo = p.IdSexo;
                var inscrip = db.vInscripcionDetalle.FirstOrDefault(m => m.IdPersona == IdPostulante);
                string Carrera = db.CarreraOficio.Find(UltimaInscripcion.IdCarreraOficio).CarreraUoficio;
                sexo = (Carrera == "Médicos") ? "Medico" : sexo;
                string PopUp = "";
                var Restric = db.spRestriccionesParaEstePostulante(IdPostulante, FechaNac, UltimaInscripcion.IdPreferencia).First();
                string Aplica = "";
                switch (AltIcm)
                {
                    case "altura":
                        PopUp = db.Configuracion.First(m => m.NombreDato == "PopUpAltura").ValorDato;
                        switch (sexo)
                        {                        
                            case 2:
                            case 1:
                                Aplica = (Restric.AlturaMinM > num) ? "NO" : "SI";
                                break;
                            case 4:
                                Aplica = (Restric.AlturaMinF > num) ? "NO" : "SI";
                                break;
                            case "Medico":
                                Aplica = "SI";
                                break;
                        }
                        break;
                    case "imc":
                        Aplica = (Restric.IMC_max < num || Restric.IMC_min > num) ? "NO" : "SI";
                        PopUp = db.Configuracion.First(m => m.NombreDato == "PopUpICM").ValorDato;
                        break;
                    default:
                        break;
                }
                return Json(new { APLICA = Aplica, POPUP = PopUp, ALTIMC = AltIcm }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /*--------------------------------------------------------------FAMILIA------------------------------------------------------------------------------*/


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Familia(int ID_persona)
        {
            try
            {
                ViewBag.Id = ID_persona;
                //List<int> id_PER_FAMI = db.Familiares.Where(m => m.IdPostulantePersona == ID_persona).Select(m => m.IdPersona).ToList();
                List<sp_vPersona_Familiar_Result> FAMILIARES = db.sp_vPersona_Familiar(ID_persona).ToList();

                return PartialView(FAMILIARES);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [AuthorizacionPermiso("ListarRP")]
        //recibo el idFamilia, si es 0 creo  una personaFamilia y su relacion.
        public ActionResult FamiliaCUD(int ID_persona, int idPersonaFamilia)
        {

            Session["FamiTable"] = true;
            if (db.Familiares.Where(m => m.IdPostulantePersona == ID_persona && m.IdPersona == idPersonaFamilia).FirstOrDefault() == null && ID_persona != 0)
            {
                var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Relacion familiar inexistente."), "Account", "FamiliaCUD");
                //revisar logica para cuando expiro el token de reestablesimiento de contraseña
                return View("Lockout", x);
            };
            //verificar que al crear un postulante llenar con los datos completos, si no es asi el familiar no se mostrara en vPersona_Familiar
            //viewmodel creado para la creacion de un familiar
            //cargo los datos necesarios para los combobox

            PersonaFamiliaVM pers = new PersonaFamiliaVM
            {
                vParentecoVM = db.vParentesco.Select(m => new SelectListItem { Value = m.idParentesco.ToString(), Text = m.Relacion }).ToList(),
                SexoVM = db.Sexo.OrderBy(m => m.Descripcion).Where(m => m.Descripcion != "Seleccione Sexo").Select(m => new SelectListItem { Value = m.IdSexo.ToString(), Text = m.Descripcion }).ToList(),
                vEstCivilVM = db.vEstCivil.Select(m => new SelectListItem { Value = m.Codigo_n, Text = m.Descripcion }).ToList(),
                ReligionVM = db.vRELIGION.Select(m => new SelectListItem { Value = m.CODIGO, Text = m.DESCRIPCION }).ToList(),
                TipoDeNacionalidadVm = db.TipoNacionalidad.Where(i => i.IdTipoNacionalidad != 4).Select(m => new SelectListItem { Value = m.IdTipoNacionalidad.ToString(), Text = m.Descripcion }).ToList()
            };
            if (idPersonaFamilia != 0)
            {
                //verifoco la situacion del postulante actual
                if (HttpContext.User.IsInRole("Postulante"))
                {
                    var EtapaTabs = db.vPostulanteEtapaEstado.Where(id => id.IdPostulantePersona == ID_persona).OrderBy(m => m.IdEtapa).DistinctBy(id => id.IdEtapa).Select(id => id.IdEtapa).ToList();
                    EtapaTabs.ForEach(m => pers.IDETAPA += m + ",");
                    //le coloco 5 por si la pantalla esta cerrada
                    if (!(bool)db.spTildarPantallaParaPostulate(ID_persona).FirstOrDefault(m => m.IdPantalla == 9).Abierta) pers.IDETAPA += "5,";
                }
                else
                {
                    pers.IDETAPA = "5,0";
                };

                pers.ID_PER = idPersonaFamilia;
                //////////////////////////////////////
                

                pers.vPersona_FamiliarVM = db.vPersona_Familiar.FirstOrDefault(m => m.IdPersonaFamiliar == idPersonaFamilia);

                //cargo la situacion actual de la persona familiar
                var SituacionFamiliar = db.sp_InvestigaDNI(pers.vPersona_FamiliarVM.DNI).First();

                //verifico si la persona familiar es postulante
                if ((bool)SituacionFamiliar.ES_Postulante)
                {
                    //ver... verifico que sea un postulante que esta en una convocatoria abierta.
                    pers.postulante = (bool)SituacionFamiliar.Convocatoria_Activa;
                };

                //cargo inscripcion del Postulante actual
                var SituacionPostulante = db.sp_InvestigaDNI(db.Persona.Find(ID_persona).DNI).First();

                int[] secublock = { 14, 24, 16, 20 };

                //verifico si la validadcion de los datos del postulante actual estan siendo validados
                ViewBag.ValidacionEnCurso = secublock.Contains((int)SituacionPostulante.IdSecuencia);

                //verifico si la convocatoria del postulante vencio, para bloquear los formularios
                ViewBag.ValidacionEnCurso = !(bool)SituacionPostulante.Convocatoria_Activa ? true : ViewBag.ValidacionEnCurso;
              
                //en caso de ser delegacion modifico el valor de ValdacionENcusrso a true para bloquear las vistas, si es postulante dejo el anterior valor
                ViewBag.ValidacionEnCurso = HttpContext.User.IsInRole("Postulante") ? ViewBag.ValidacionEnCurso : true;

                //Si la pantalla de familiar esta cerrada bloqueo los controles en la vista
                ViewBag.ValidacionEnCurso = (db.VerificacionPantallasCerradas.FirstOrDefault(m => m.IdPantalla == 9 && m.IdPostulantePersona == ID_persona) != null) ? true : ViewBag.ValidacionEnCurso;
            }
            else
            {
                pers.IDETAPA = "0";
                pers.ID_PER = idPersonaFamilia;
                var IdAspUser = db.AspNetUsers.FirstOrDefault(e => e.Email == User.Identity.Name).Id;
                pers.vPersona_FamiliarVM = new vPersona_Familiar();
                pers.vPersona_FamiliarVM.IdPersonaPostulante = db.Postulante.FirstOrDefault(e => e.IdAspNetUser == IdAspUser).IdPersona;
                pers.vPersona_FamiliarVM.IdFamiliar = 0;
                pers.vPersona_FamiliarVM.IdPersonaFamiliar = 0;
            }

            return View(pers);
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        [ValidateAntiForgeryToken]
        public JsonResult FamiliaCUD(PersonaFamiliaVM fami)
        {


            if (ModelState.IsValid)
            {
                //datos del familiar a crear 
                var datos = fami.vPersona_FamiliarVM;
                int? idpersonafamiliar = 0;
                try
                {
                    //guardo la persona familiar
                    var per = db.Persona.FirstOrDefault(d => d.DNI == datos.DNI);
                    //guardo el id del postulante actual
                    var IDPOSTULANTE = datos.IdPersonaPostulante;
                    Familiares rela = null;
                    string msgs;

                    if (per != null)
                    {
                        rela = db.Persona.Find(IDPOSTULANTE).Postulante.Familiares.FirstOrDefault(m => m.IdPersona == per.IdPersona);
                        msgs = (rela != null) ? "Modificacion del Familiar Exitoso." : "Se agrego un familiar exitosamente. Refrescando Vista...";
                    }
                    else
                    {
                        msgs = "Creacion del Familiar Exitoso. Refrescando Vista...";
                    };

                    if ((per != null && rela != null) || per == null)
                    {
                        datos.IdReligion ??= "";
                        db.spPERSONAFamiliarIU(datos.IdPersonaFamiliar, datos.IdPersonaPostulante, datos.Mail, datos.Apellido, datos.Nombres, datos.IdSexo, datos.FechaNacimiento, datos.DNI, datos.CUIL,
                        datos.IdReligion, datos.IdEstadoCivil, datos.FechaCasamiento, datos.Telefono, datos.Celular, datos.Mail, datos.idTipoNacionalidad, 0, datos.idParentesco, datos.Vive, datos.ConVive);
                        idpersonafamiliar = (per == null) ? db.vPersona_Familiar.FirstOrDefault(d => d.DNI == datos.DNI).IdPersonaFamiliar : 0;
                    }
                    else if (rela == null)
                    {
                        db.spRelacionFamiliarIU(0, datos.IdPersonaPostulante, per.IdPersona, datos.idParentesco, datos.Vive, datos.ConVive);
                        datos.IdPersonaFamiliar = per.IdPersona;

                        idpersonafamiliar = db.vPersona_Familiar.FirstOrDefault(d => d.DNI == datos.DNI.ToString()).IdPersonaFamiliar;

                    }
                    per = db.Persona.FirstOrDefault(d => d.DNI == datos.DNI);
                    return Json(new { success = true, msg = msgs, IDperFAMI = idpersonafamiliar, IDperPOST = IDPOSTULANTE }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente." }, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(new { success = false, msg = "Datos no validos, revise los mismos." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult SinFamiliar(int idper)
        {

            try
            {
                db.spPERSONAFamiliarIU(0, idper, null, null, null, null, null, null, null, null, null, null, null, null,null,null,null,999,null,null);
                return Json(new { success = true, msg = "Sin familiar seleccionado", form = "Elimina", url_Tabla = "Familia", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" }, JsonRequestBehavior.AllowGet);

            }



        }
        public JsonResult VerificarDNI(int DNI, int ID)
        {
            try
            {
                var SituacionDNI = db.sp_InvestigaDNI(DNI.ToString()).First();

                if (SituacionDNI.IdPersona != 0)
                {
                    Familiares rela = db.Persona.Find(ID).Postulante.Familiares.FirstOrDefault(m => m.IdPersona == SituacionDNI.IdPersona);
                    //si ya tiene una relacion conel postulante la persona a agragr como familiar lo notifico
                    string msgs, resps;

                    msgs = (rela != null) ? $"La persona con Dni: {DNI}, ya esta cargado como familiar. Redirigiendo..." : $"La persona con Dni: {DNI} que desea agregar como familiar ya existe, ¿Desea agregarlo?";
                    resps = (rela != null) ? "son_familiares" : "existe";

                    return Json(new { resp = resps, msg = msgs, IDperFAMI = SituacionDNI.IdPersona, IDperPOST = ID }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { resp = "no_existe" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = false, msg = "Error en la operacion" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaFAMI(int ID_per, int ID_fami)
        {
            try
            {

                int filas = db.Familiares.Where(p => p.IdPersona == ID_per).Count();
                bool postulante = db.Postulante.Find(ID_per) != null;
                //si el familiar es un postulante solo elimino la relacion_familiar con el postulante actual
                if (postulante || filas > 1)
                {
                    db.spRelacionFamiliarEliminar(ID_fami);
                }
                else
                {
                    db.spFamiliarEliminar(ID_fami, ID_per);
                }

                return Json(new { success = true, msg = "Se elimino correctamente el Familiar", form = "Elimina", url_Tabla = "Familia", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error en la operacion, intentelo nuevamente" }, JsonRequestBehavior.AllowGet);
            }
        }
        /*-------------------------------------------------------------Documentacion------------------------------------------------------------------------------*/

        public ActionResult DocumentacionAnexo(int IdPersona)
        {
            DocuAnexoVM docu = new DocuAnexoVM()
            {
                IdPersona = IdPersona

            };
            //busco de la vista "vPostPersonaEtapaEstadoUltimoEstado", el ID de la ultima inscripcion
            int idinscrip = db.vPostPersonaEtapaEstadoUltimoEstado.FirstOrDefault(m=>m.IdPersona==IdPersona).IdInscripcionEtapaEstado;
            docu.docus = db.DocumentosNecesariosDelInscripto(idinscrip).OrderByDescending(m => m.Obligatorio).ToList();
            var secus = db.InscripcionEtapaEstado.Where(m => m.IdInscripcionEtapaEstado == idinscrip).OrderByDescending(n => n.Fecha).ToList();
            //verifico que ya haya tenido una respuesta de validacion departe de la delegacion
            ViewBag.secucu = secus[0].IdSecuencia == 13 && secus.FirstOrDefault(m => m.IdSecuencia == 14) != null;
            return PartialView("DocumentacionAnexo", docu);
        }

        //------------VALIDAR DATOS--------------------//         
        [HttpPost]
        [AuthorizacionPermiso("ModificarSecuenciaP")]
        public dynamic ValidarDatos(int ID_persona)
        {
            try
            {

                //int id_otroProblema;
                var ListaProblemaEncontrado = new List<DataProblemaEncontrado>();
              
                //var jss = new JavaScriptSerializer();
                //var ListasPantalla = jss.Deserialize<List<PantallasError>>(PantError).DistinctBy(m=>m.IdPantalla).ToList();

                
                //foreach (var Pantalla in ListasPantalla)
                //{
              
                //    //cargo un listado con los ID de las pantallas que presentan campos vacios o tablas sin registros.
                //    Pantalla.IdPant= db.VerificacionPantallas.FirstOrDefault(m => m.Pantalla == Pantalla.IdPantalla).IdPantalla;
                   
                //    //genero un registro de la novedad para cada pantalla que presente las misma
                //    id_otroProblema = db.DataVerificacion.FirstOrDefault(m => m.IdPantalla == Pantalla.IdPant && m.Descripcion == "Otros: Aclare").IdDataVerificacion;
                //    if (db.DataProblemaEncontrado.FirstOrDefault(m => m.IdDataVerificacion == id_otroProblema && m.IdPostulantePersona == ID_persona) == null)
                //    {
                //        ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                //        {
                //            IdPostulantePersona = ID_persona,
                //            Comentario = Pantalla.Pantalla+ ", en esta solapa no se han cargado datos o solo se cargo parcialmente, verificar lo mismo.",
                //            IdDataVerificacion = id_otroProblema
                //        });
                //    }

                //}

                ////verificacion FAMILIARES DATOS VACIOS
                //List<vPersona_Familiar> familiares = new List<vPersona_Familiar>();

                //db.Familiares.Where(m => m.IdPostulantePersona == ID_persona).ToList().ForEach(
                //    m=> familiares.Add(new vPersona_Familiar() { 
                //        IdPersonaFamiliar = m.IdPersona,
                //        Apellido= m.Persona.Apellido,
                //        Nombres = m.Persona.Nombres,

                //    })
                //    );
              

                //if (familiares.Count > 0)
                //{
                //    foreach (var familiar in familiares)
                //    {

                //        //Domicilio
                //        var domicilio = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == familiar.IdPersonaFamiliar);
                //        if (!this.TryValidateModel(domicilio) && db.DataProblemaEncontrado.FirstOrDefault(m=>m.IdDataVerificacion==43 && m.Comentario.Contains(familiar.Apellido.ToUpper() +" " + familiar.Nombres.ToUpper()))==null )
                //        {
                //            ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                //            {
                //                IdPostulantePersona = ID_persona,
                //                Comentario = String.Format("Para el Familiar, {0} {1}.", familiar.Apellido.ToUpper(), familiar.Nombres.ToUpper()),
                //                IdDataVerificacion = 43//id problema otro para la pantalla domicilio
                //            });
                //        }

                //        //Estudio
                //        if (db.VPersona_Estudio.Where(m => m.IdPersona == familiar.IdPersonaFamiliar).ToList().Count == 0 && db.DataProblemaEncontrado.FirstOrDefault(m => m.IdDataVerificacion == 44 && m.Comentario.Contains(familiar.Apellido.ToUpper() + " " + familiar.Nombres.ToUpper())) == null)
                //        {

                //            ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                //            {
                //                IdPostulantePersona = ID_persona,
                //                Comentario = String.Format("Para el Familiar, {0} {1}.", familiar.Apellido.ToUpper(), familiar.Nombres.ToUpper()),
                //                IdDataVerificacion = 44//id problema otro para la pantalla Estudios
                //            });
                //        }

                //        //Actividad militar
                //        if (db.vPersona_ActividadMilitar.Where(m => m.IdPersona == familiar.IdPersonaFamiliar).ToList().Count == 0 && db.DataProblemaEncontrado.FirstOrDefault(m => m.IdDataVerificacion == 45 && m.Comentario.Contains(familiar.Apellido.ToUpper() + " " + familiar.Nombres.ToUpper())) == null)
                //        {

                //            ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                //            {
                //                IdPostulantePersona = ID_persona,
                //                Comentario = String.Format("Para el Familiar, {0} {1}.", familiar.Apellido.ToUpper(), familiar.Nombres.ToUpper()),
                //                IdDataVerificacion = 45//id problema otro para la pantalla Actividad Miltar
                //            });
                //        }

                //    }
                //}

                var persona = db.vPersona_DatosPer_UltInscripc.FirstOrDefault(m => m.IdPersona == ID_persona);
                var antropo = db.Antropometria.FirstOrDefault(m => m.IdPostulantePersona == ID_persona);
                var problemasPostu = db.DataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona);

                //ALTURA Y IMC
                // para validadr la pantalla de antropometria veridico que se haya completado el formulario y que la misma este abierta
                if (antropo != null && (bool)db.spTildarPantallaParaPostulate(ID_persona).FirstOrDefault(m => m.IdPantalla == 8).Abierta)
                {
                    //verificacion de la altura si valida o no en caso de no ser se genera un registro de error para ser revisado por la Delegacion
                    var APLICAAltura = VerificaAltIcm(ID_persona, "altura", antropo.Altura).Data.ToString().Split(',')[0].ToString().Split('=')[1].Trim();
                    if (APLICAAltura == "NO" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 48) == null)
                    {
                        ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                        {
                            IdPostulantePersona = persona.IdPersona,
                            Comentario = db.DataVerificacion.First(m => m.IdDataVerificacion == 48).Descripcion,
                            IdDataVerificacion = 48
                        });
                    };
                    //verificacion de la altura si valida o no en caso de no ser se genera un registro de error para ser revisado por la Delegacion
                    var APLICAImc = VerificaAltIcm(ID_persona, "imc", (float)antropo.IMC).Data.ToString().Split(',')[0].ToString().Split('=')[1].Trim();
                    if (APLICAImc == "NO" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 49) == null)
                    {

                        ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                        {
                            IdPostulantePersona = persona.IdPersona,
                            Comentario = db.DataVerificacion.First(m => m.IdDataVerificacion == 49).Descripcion,
                            IdDataVerificacion = 49
                        });
                    };
                };
                var IDPREFE = db.Inscripcion.FirstOrDefault(m => m.IdInscripcion == persona.IdInscripcion).IdPreferencia;

                //ESTADO CIVIL Y TIPO DE NACIONALIDAD
                //de los registros traidos por 'spRestriccionesParaEstePostulante', eligo al cual corresponda al postulante
                var restriccionesEstadoCivil = db.spRestriccionesParaEstePostulante(persona.IdPersona, persona.FechaNacimiento, IDPREFE).First(m => m.IdInstitucion == IDPREFE);
                bool PantallaDatospersonales = (bool)db.spTildarPantallaParaPostulate(ID_persona).FirstOrDefault(m => m.IdPantalla == 1).Abierta;
                if (persona.IdModalidad != null && PantallaDatospersonales)
                {
                    //Verifico el estado civil y el tipo de nacionalidad
                    //verifico tipo de nacionalidad en caso de ser "Argentino por Opcion" y tenga modalidad distinta a "SMV", agrego un problema en DataProblemaEncontrado
                    if (persona.idTipoNacionalidad == 3 && persona.IdModalidad != "SMV" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 51) == null)
                    {
                        ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                        {
                            IdPostulantePersona = persona.IdPersona,
                            Comentario = "Verificar que al menos uno de los padres tenga tipo de nacionalidad NATIVO.",
                            IdDataVerificacion = 51
                        });
                    };
                    //verifico si cumple con la restrccion de Estado Civil para la modalidad que corresponde
                    if (restriccionesEstadoCivil.IdEstadoCivil != persona.IdEstadoCivil && restriccionesEstadoCivil.IdEstadoCivil != "" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 50) == null)
                    {

                        ListaProblemaEncontrado.Add(new DataProblemaEncontrado
                        {
                            IdPostulantePersona = persona.IdPersona,
                            Comentario = "Restriccion que causa la Interrupcion del Proceso de Inscripcion",
                            IdDataVerificacion = 50
                        });
                    };

                }
                db.DataProblemaEncontrado.AddRange(ListaProblemaEncontrado);
                db.SaveChanges();

                //ver esto solo disponible si se encuntra en la secuencia 13 "inicio De Carga/DOCUMENTACION"
                db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 14, "", "");

                //Envio de Mail para notificar a la delegacion correpondiente
                var ultimaInscripcion = db.vInscripcionDetalleUltInsc.FirstOrDefault(m => m.IdPersona == ID_persona);
                int ID_Delegacion = ultimaInscripcion.IdOficinasYDelegaciones;
                int ID_INSCRIP = ultimaInscripcion.IdInscripcion;

                var per = db.Persona.FirstOrDefault(m => m.IdPersona == ID_persona);
                ValidoCorreoPostulante datosMail = new ValidoCorreoPostulante()
                {

                    Apellido = "",
                    Apellido_P = per.Apellido,
                    Dni_P = per.DNI,
                    IdInscripcion_P = ID_INSCRIP,
                    Nombre_P = per.Nombres,
                    url = Url.Action("Documentacion", "Delegacion", new { id = ID_persona }, protocol: Request.Url.Scheme)
                };

                Func.EnvioDeMail(datosMail, "PlantillaInicioValidacionParaDelegacion", null, null, "MailAsunto7", ID_Delegacion, null);

               
                return Json(new { success = true, msg = "Operacion Exitosa", form = "ValidarDatos" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    msg = "Error en la operacion, intentelo nuevamente"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult ProblemasPantalla(int ID_persona, int? IdPantalla, string? Pantalla)
        {
            try
            {
                if (IdPantalla == null)
                {
                    IdPantalla = db.VerificacionPantallas.FirstOrDefault(m => m.Pantalla == Pantalla).IdPantalla;
                }
                return PartialView(db.vDataProblemaEncontrado.Where(p => p.IdPostulantePersona == ID_persona).Where(m => m.IdPantalla == IdPantalla).ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /*--------------------------------------------------------------PRESENTACION------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Presentacion(int ID_persona)
        {
            //vInscripcionDetalleUltInsc, vista que muestra solo la ultima inscipciondel postulante
            var ultimaInscripcion = db.vInscripcionDetalleUltInsc.First(m => m.IdPersona == ID_persona);
            Presentacion prese = new Presentacion
            { 
                IdPersona = ultimaInscripcion.IdPersona,
                Apellido = ultimaInscripcion.Apellido,
                Nombre = ultimaInscripcion.Nombres,
                ID_Inscripcion = ultimaInscripcion.IdInscripcion,
                Modalidad = ultimaInscripcion.Modalidad,
                Carrera = ultimaInscripcion.CarreraRelacionada.ToString(),
                Fecha_Inicio = Convert.ToDateTime(ultimaInscripcion.FechaInicio),
                Fecha_Fin=Convert.ToDateTime(ultimaInscripcion.FechaFinal)
                

            };
            ViewBag.Asignado = true;
            if (ultimaInscripcion.FechaRindeExamen == null)
            {
                ViewBag.Asignado = false;
            }
            else
            {
                //busco el establecimiento donde rinde examen, de acuerdo a la ultima inscripcion
                var establecimientoRinde = db.Inscripcion.FirstOrDefault(m => m.IdInscripcion == ultimaInscripcion.IdInscripcion).EstablecimientoRindeExamen;
                prese.DomicilioExamenNombre = establecimientoRinde.Nombre;
                prese.DomicilioExamen = establecimientoRinde.Jurisdiccion + ", " + establecimientoRinde.Localidad + ", " + establecimientoRinde.Direccion;
                prese.FechaPresentacion = (DateTime)ultimaInscripcion.FechaRindeExamen;
                string url = "http://" + HttpContext.Request.Url.Host + Url.Action("Index", "Postulante", new { ID_Postulante = ID_persona });
                prese.Qr = generarQR(url);
                ViewBag.QRCodeImageLink = url;
            };
            return PartialView(prese);
        }

        [AuthorizacionPermiso("CreaEditaDatosP")]
        private byte[] generarQR(string texto)
        {   //https://github.com/codebude/QRCoder ver documentacion ejemplo de usos ej logo en el qr entre muchas otras cosas
            //establesco la ubicacion del logo que aparecera em el codigo QR
            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
            string ubicacionImagen = $"{ubicacion}Imagenes\\AnclaQR.png";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, ColorTranslator.FromHtml("#212429"), System.Drawing.Color.White, (Bitmap)Bitmap.FromFile(ubicacionImagen), 25, 10);
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }

        }

        [AuthorizacionPermiso("CreaEditaDatosP")]
        public ActionResult InscripConvo(int ID_postulante)
        {
            ViewBag.idpostu = ID_postulante;
            return View(db.vPeriodosInscrip.ToList());

        }

        [AuthorizacionPermiso("CreaEditaDatosP")]
        public ActionResult InscriNueva(int ID_postulante, int id_institucion)
        {
            try
            {

                var postulante = db.Postulante.Find(ID_postulante);
                //mando como true el primer parametro  del 'spCreaPostulante', para indicar que se va a realizar  un reinscripcion
                var result = db.spCreaPostulante(true, postulante.IdPersona, postulante.Persona.Apellido, postulante.Persona.Nombres, postulante.Persona.DNI, 
                                                 postulante.Persona.Email, id_institucion, postulante.Inscripcion.First().IdDelegacionOficinaIngresoInscribio);
                Session["IncripcionNueva"] = "Se realizó correctamente la nueva inscripción en la Covocatoria seleccionada.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View();
            }
            
        }

        //resultado de presentacion a ver.....
        public ActionResult Resultado(int ID_persona)
        {
            ResultadoVM resultado = new ResultadoVM
            {
                vIncripcion = db.vInscripcionDetalle.FirstOrDefault(m=>m.IdPersona== ID_persona),
                result = true
            };
            return PartialView(resultado);
        }

    }
}
