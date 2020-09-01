using SINU.Authorize;
using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SINU.ViewModels;
using System.Threading.Tasks;
using System;
using System.IO;

namespace SINU.Controllers
{
    [AuthorizacionPermiso("RUDInscripcion")]
    public class DelegacionController : Controller
    {
        SINUEntities db = new SINUEntities();
        OficinasYDelegaciones UsuarioDelegacion;
        // GET: Delegacion
        public ActionResult Index()
        {
            try
            {
                //busco la delegacion que pertenece al usuario con perfil de delegacion            
                UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                ViewBag.Delegacion = UsuarioDelegacion.Nombre;
                // tomara los datos de incripciones correspondiente a la Delegacion /cuenta usario Asociado
                //cargo todos los registros que hayan validado la cuenta, y esten en la carga de los datos basicos, pero además que pertenezcan a la delegacion del usuario actual.
                DelegacionPostulanteVM datos = new DelegacionPostulanteVM()
                {
                    PostulantesIncriptosVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.IdSecuencia >= 5 && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList(),
                    cargadatosbasicosVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "DATOS BASICOS" && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList(),
                    EntrevistaVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "ENTREVISTA" && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList(),
                    DocumentacionVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "DOCUMENTACION" && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList(),
                    PresentacionVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "PRESENTACION" && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList(),
                 };

                return View("Index", datos);

            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Index"));
            }
        }

        // GET: Delegacion/Details/5
        public ActionResult Details(int? id)
        {
            vInscripcionDetalle vInscripcionDetalle;
            List<vPersona_DatosBasicos> vPersona_Datos;
            try
            {
                UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                ViewBag.Delegacion = UsuarioDelegacion.Nombre;
                if (id == null)
                {
                    return View("Error", Func.ConstruyeError("Falta el Nro de ID que desea buscar en la tabla de INSCRIPTOS", "Delegacion", "Details"));
                }
                vInscripcionDetalle = db.vInscripcionDetalle.FirstOrDefault(m => m.IdInscripcion == id);
                int x = vInscripcionDetalle.IdPersona;
                vPersona_Datos=db.vPersona_DatosBasicos.Where(m=>m.IdPersona==x && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList();
                if (vPersona_Datos.Count == 0)
                {
                    return View("Error", Func.ConstruyeError("Incorrecta la llamada a la vista detalle con el id " + id.ToString() + " ==> NO EXISTE o no le corresponde verlo", "Delegacion", "Details"));
                }
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Details"));
            }
            return View(vPersona_Datos.ToList()[0]);
        }
        public ActionResult EntrevistaAsignaFecha(int id)
        {
            try
            {
                vEntrevistaLugarFecha Dato = db.vEntrevistaLugarFecha.FirstOrDefault(m => m.IdPersona == id);

                return View(Dato);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        // POST: Delegacion/Create
        [HttpPost]
        public ActionResult EntrevistaAsignaFecha(vEntrevistaLugarFecha datos)
            {
            List<vInscripcionDetalle> InscripcionElegida;
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;

            try
            {
                // TODO: Add insert logic here

                var da = db.Inscripcion.Find(datos.IdInscripcion);
                da.FechaEntrevista = datos.FechaEntrevista;
                db.SaveChanges();
                db.spProximaSecuenciaEtapaEstado(0,datos.IdInscripcion,false,0,"","");


                MailConfirmacionEntrevista Modelo = new MailConfirmacionEntrevista{
                    Apellido = datos.Apellido,
                    FechaEntrevista = datos.FechaEntrevista
                };
                //verificar el llamado de una funcion asyncronica desde un metodo sincronico
                var Result = Func.EnvioDeMail(Modelo, "MailConfirmacionEntrevista", null, datos.IdPersona, "MailAsunto4");

                InscripcionElegida = db.vInscripcionDetalle.Where(m => m.IdInscripcion == datos.IdInscripcion).ToList();
                vInscripcionEtapas = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == datos.IdInscripcion);

                if (vInscripcionEtapas.Estado == "Asignada")
                {
                    db.spProximaSecuenciaEtapaEstado(0, datos.IdInscripcion, false, 0, "", "");
                }

                return RedirectToAction("Index");
            }


            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        [HttpPost]
        // GET: Delegacion/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                vInscripcionDetalle elegido = db.vInscripcionDetalle.First(m=>m.IdPersona==id);
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
        [HttpGet]
        public ActionResult Postular(int? id)
        {
            List<vInscripcionDetalle> InscripcionElegida;
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

                InscripcionElegida = db.vInscripcionDetalle.Where(m => m.IdInscripcion == id && m.IdOficinasYDelegaciones == UsuarioDelegacion.IdOficinasYDelegaciones).ToList();

                if (InscripcionElegida.Count == 0)
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
            return View(InscripcionElegida.ToList()[0]);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Postular(string botonPostular, int id)
        {
            List<vInscripcionDetalle> InscripcionElegida;
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;
            Configuracion configuracion;
            bool x = false;
            string cuerpo="";
            try
            {
                switch (botonPostular)
                {
                    case "Postular":
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "ENTREVISTA", "Postulado");
                        x = true;
                        configuracion = db.Configuracion.FirstOrDefault(m => m.IdConfiguracion == 1044);
                        cuerpo = configuracion.ValorDato.ToString();
                        break;
                    case "No Postular":
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "ENTREVISTA", "No Postulado");
                        configuracion = db.Configuracion.FirstOrDefault(m => m.IdConfiguracion == 1046);
                        cuerpo = configuracion.ValorDato.ToString();
                        break;
                    case "Volver":
                        db.spProximaSecuenciaEtapaEstado(0, id, true, 0, "", "");
                        break;
                }
                    InscripcionElegida = db.vInscripcionDetalle.Where(m => m.IdInscripcion == id).ToList();
                    vInscripcionEtapas = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == id);
                    
                    var callbackUrl = Url.Action("Index", "Postulante", null, protocol: Request.Url.Scheme);

                    var modeloPlanti = new ViewModels.MailPostular
                    {
                        Apellido = vInscripcionEtapas.Apellido,
                        MailCuerpo = cuerpo,
                        LinkConfirmacion = callbackUrl,
                        Postulado=x
                    };
                switch ((vInscripcionEtapas.Estado).ToString())
                {
                    case "Postulado":
                        bool envioP = await Func.EnvioDeMail(modeloPlanti, "MailPostulado", null, vInscripcionEtapas.IdPersona, "MailAsunto2");
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "", "");
                        break;
                    case "No Postulado":
                        bool envioNP = await Func.EnvioDeMail(modeloPlanti, "MailPostulado", null, vInscripcionEtapas.IdPersona, "MailAsunto2");
                        break;
                }
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Index"));
            }
            return RedirectToAction("Index");
        }

        //terminar codigo faltante
        [HttpGet]
        public ActionResult Documentacion(int id)
        {
            vInscripcionDetalle vdetalle;
            vdetalle = db.vInscripcionDetalle.FirstOrDefault(m => m.IdPersona == id);
            var NyA = vdetalle.Nombres +" "+ vdetalle.Apellido;


            IDPersonaVM personaVM = new IDPersonaVM();
            personaVM.ID_PER = id;
            personaVM.NomyApe = NyA;



            return View(personaVM);
        }
        [HttpPost]
        public ActionResult Documentacion(int? id)
        {
            try
            {
             db.spProximaSecuenciaEtapaEstado(id, 0, false, 0, "", "");
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
            return Json(new{View="Index"});
        }

        //fin del codigo
        public ActionResult DocPenal(int id)
        {
            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
            string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
            string NombreArchivo = id + "_Certificado.pdf";
            string Descargar = CarpetaDeGuardado + NombreArchivo;
            //string UbicacionPDF = $"{ubicacion}Documentacion\\"+id+".pdf";
            byte[] FileBytes = System.IO.File.ReadAllBytes(Descargar);
            //el tercer para obligar la descarga del archivo
            return File(FileBytes, "application/pdf", NombreArchivo);

        }


        public ActionResult GetAnexo2Pdf(int id)
        {
            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
            string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
            string NombreArchivo = id + "_Anexo2.pdf";
            string Descargar = CarpetaDeGuardado + NombreArchivo;
            //string UbicacionPDF = $"{ubicacion}Documentacion\\"+id+".pdf";
            byte[] FileBytes = System.IO.File.ReadAllBytes(Descargar);
            //el tercer para obligar la descarga del archivo
            return File(FileBytes, "application/pdf", NombreArchivo);
        }
        public ActionResult PresentacionAsignaFecha(int id)
        {
            try
            {
                vInscripcionDetalle Dato = db.vInscripcionDetalle.FirstOrDefault(m => m.IdPersona == id);

                return View(Dato);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        // POST: Delegacion/Create
        [HttpPost]
        public ActionResult PresentacionAsignaFecha(vInscripcionDetalle datos)
        {
            //List<vInscripcionDetalle> InscripcionElegida;
            //vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;

            try
            {
                // TODO: Add insert logic here

                var da = db.Inscripcion.Find(datos.IdInscripcion);
                da.FechaRindeExamen = datos.FechaRindeExamen;
                db.SaveChanges();
                db.spProximaSecuenciaEtapaEstado(0, datos.IdInscripcion, false, 0, "", "");


                //MailConfirmacionEntrevista Modelo = new MailConfirmacionEntrevista
                //{
                //    Apellido = datos.Apellido,
                //    FechaEntrevista = datos.FechaEntrevista
                //};
                ////verificar el llamado de una funcion asyncronica desde un metodo sincronico
                //var Result = Func.EnvioDeMail(Modelo, "MailConfirmacionEntrevista", null, datos.IdPersona, "MailAsunto4");

                //InscripcionElegida = db.vInscripcionDetalle.Where(m => m.IdInscripcion == datos.IdInscripcion).ToList();
                //vInscripcionEtapas = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == datos.IdInscripcion);

                //if (vInscripcionEtapas.Estado == "Asignada")
                //{
                //    db.spProximaSecuenciaEtapaEstado(0, datos.IdInscripcion, false, 0, "", "");
                //}

                return RedirectToAction("Index");
            }


            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        [HttpGet]
        public ActionResult ListaProblema(int ID_persona)
        {
            //List<DataVerificacion> dataVerificacions;
            //dataVerificacions = db.DataVerificacion.ToList();
            IDPersonaVM personaVM = new IDPersonaVM();

            List<DataVerificacion> data = new List<DataVerificacion>();
            data = db.DataVerificacion.ToList();

            personaVM.DataVerificacionVM = data;
            personaVM.ID_PER = ID_persona;
            return View(personaVM);
        }



        [HttpGet]
        public ActionResult DataProblema(int id, int id_per)
        {

            ProblemaEcontradoVM problema = new ProblemaEcontradoVM();
            problema.ID_PER = id_per;
            problema.IdDataverificacion = id;
            problema.Comentario = "";

            vInscripcionDetalle vdetalle;
            vdetalle = db.vInscripcionDetalle.FirstOrDefault(m => m.IdPersona == id_per);
            DataVerificacion data;
            data = db.DataVerificacion.FirstOrDefault(m => m.IdDataVerificacion == id);
            ViewBag.Nombre = vdetalle.Nombres;
            ViewBag.Apellido = vdetalle.Apellido;
            ViewBag.IdInscripcion = vdetalle.IdInscripcion;
            ViewBag.Texto = data.Descripcion;

            return View(problema);
        }
        [HttpPost]
        public ActionResult DataProblema(ProblemaEcontradoVM dato)
        {
            
            if (ModelState.IsValid)
            {
                
                
            }
            return View();
        }



    }
}
