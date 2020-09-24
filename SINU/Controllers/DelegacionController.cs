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
                        configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4Postulado");
                        cuerpo = configuracion.ValorDato.ToString();
                        break;
                    case "No Postular":
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "ENTREVISTA", "No Postulado");
                        //el texto de respuesta aqui depende de la preferencia que selecciono al moneto de llenar datos basicos
                        configuracion = (db.Inscripcion.FirstOrDefault(m => m.IdInscripcion == id).IdPreferencia == 6) ? db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado2") : db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado1");
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
        [HttpPost]
        public ActionResult VolverEtapa(int? ID_persona)
        {
            try
            {
                db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "DOCUMENTACION", "Inicio De Carga");
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
            return Json(new { View = "Index" });
        }
        //fin del codigo
        public ActionResult DocPenal(int id)
        {
            try
            {
                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string NombreArchivo = id + "_Certificado.pdf";
                string Descargar = CarpetaDeGuardado + NombreArchivo;
                bool file = System.IO.File.Exists(Descargar);
                if (file == false)
                {
                  return View("Error", Func.ConstruyeError("Hubo un problema con el postulante N° " + id.ToString() + " No existe documento para dicho postulante", "Delegacion", "Details"));
                }
                byte[] FileBytes = System.IO.File.ReadAllBytes(Descargar);
                return File(FileBytes, "application/pdf", NombreArchivo);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "_Docupenal"));
            }
        }


        public ActionResult GetAnexo2Pdf(int id)
        {
            try
            {
                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string NombreArchivo = id + "_Certificado.pdf";
                string Descargar = CarpetaDeGuardado + NombreArchivo;
                bool file = System.IO.File.Exists(Descargar);
                if (file == false)
                {
                    return View("Error", Func.ConstruyeError("Hubo un problema con el postulante N° " + id.ToString() + " No existe documento para dicho postulante", "Delegacion", "Details"));
                }
                byte[] FileBytes = System.IO.File.ReadAllBytes(Descargar);
                return File(FileBytes, "application/pdf", NombreArchivo);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "_Docupenal"));
            }
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
        public ActionResult PresentacionAsignaFecha(string[] select, DateTime fecha)
        {
            //List<vInscripcionDetalle> InscripcionElegida;
            //vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;

            try
            {
                // TODO: Add insert logic here
                foreach (var item in select)
                {
                    int x = Convert.ToInt32(item);
                    var da = db.Inscripcion.FirstOrDefault(m=>m.IdPostulantePersona==x);
                    da.FechaRindeExamen = fecha;
                    
                    db.spProximaSecuenciaEtapaEstado(x, 0, false, 0, "", "");

                }
                db.SaveChanges();

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
        public ActionResult ListaProblema(int ID_persona)
        {
            List<vDataProblemaEncontrado> problema = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona).ToList();

            
            return View(problema);

        }
        [HttpGet]
        public ActionResult DataProblema(int? ID,int ID_persona)
        {
            try
            {
                ProblemaEcontradoVM problema = new ProblemaEcontradoVM
                {
                    ListDataVerificacionVM = new SelectList(db.DataVerificacion.ToList(), "IdDataVerificacion", "Descripcion")
                };
                var postu = db.Persona.FirstOrDefault(m => m.IdPersona == ID_persona);
                if (ID == null)
                {
                    problema.vListDataProblemasVM = new vDataProblemaEncontrado() {
                        IdPostulantePersona= ID_persona,
                        Apellido_Y_Nombres= postu.Apellido +", "+postu.Nombres,
                        DNI= postu.DNI
                    };
                     
                }
                else
                {
                    problema.vListDataProblemasVM = db.vDataProblemaEncontrado.FirstOrDefault(m => m.IdDataProblemaEncontrado==ID);
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
                            IdDataProblemaEncontrado=dato.vListDataProblemasVM.IdDataProblemaEncontrado
                        };
                        db.Entry(dataProblemaEncontrado).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, msg = "Se Inserto correctamente el Problema" });
                    }
                }
            }
            catch(Exception ex)
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
        //public ActionResult ProblemaPantalla(int IdPostulante, int IdPantalla)
        //{
        //    try
        //    {
        //        ProblemaPantallaVM datos = new ProblemaPantallaVM()
        //        {
        //            ListDataProblemaPantallaVM= db.DataProblemaPantalla.Where(m => m.IdPostulantePersona == IdPostulante).Where(m => m.IdPantalla == IdPantalla).ToList(),
        //            DataProblemaPantallaVM= new DataProblemaPantalla() {
        //                IdPantalla= IdPantalla,
        //                IdPostulantePersona= IdPostulante
        //            }
        //        };
               
        //        return PartialView(datos);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
            
        //}
        //[HttpPost]
        //public JsonResult ProblemaPantalla(ProblemaPantallaVM datos)
        //{
        //    try
        //    {
        //      DataProblemaPantalla data = datos.DataProblemaPantallaVM;
        //                db.DataProblemaPantalla.Add(data);
        //                db.SaveChanges();
        //                return Json(new { success = true, msg = "Comentario Agregado" });
        //    }
        //    catch (Exception x)
        //    {

        //        return Json(new { msg = x.InnerException.Message });
        //    }
          
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DataProblema([Bind(Include = "Comentario,IdPostulantePersona,IdDataVerificacion")] ProblemaEcontradoVM problemaEcontradoVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        DataProblemaEncontrado DataProblemaEncontrado = new DataProblemaEncontrado
        //        {
        //            Comentario = ProblemaEcontradoVM.,
        //            IdPostulantePersona = ProblemaEcontradoVM.IdPostulantePersona,
        //            IdDataVerificacion = ProblemaEcontradoVM.IdDataVerificacion
        //        };
        //        db.DataProblemaEncontrado.Add(DataProblemaEncontrado);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(ProblemaEcontradoVM);
        //}
        //public FileResult GetHTMLPageAsPDF(long empID)
        //{
        //    string htmlPagePath = "anypath...";
        //    // convert html page to pdf
        //    PageToPDF obj_PageToPDF = new PageToPDF();
        //    byte[] databytes = obj_PageToPDF.ConvertURLToPDF(htmlPagePath);

        //    //return resulted pdf document        
        //    var contentLength = databytes.Length;
        //    Response.AppendHeader("Content-Length", contentLength.ToString());
        //    Response.AppendHeader("Content-Disposition", "inline; filename=" + empID + ".pdf");

        //    return File(databytes, "application/pdf;");
        //}



        }
}
