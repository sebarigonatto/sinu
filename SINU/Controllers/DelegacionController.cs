﻿using SINU.Authorize;
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
        public ActionResult Details(int? id)//Recibe el IdPersona
        {
            //List<vInscripcionDetalle> vInscripcionDetalle;
            //List<DataProblemaEncontrado> dataProblemas;
            try
            {
                RestaurarPostulanteVM datos = new RestaurarPostulanteVM()
                {
                    vInscripcionDetallesVM = db.vInscripcionDetalle.Where(m => m.IdPersona == id).ToList(),
                    vDataProblemaEncontradoVM = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == id).ToList()
                };
                //UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                //ViewBag.Delegacion = UsuarioDelegacion.Nombre;
                //if (id == null)
                //{
                //    return View("Error", Func.ConstruyeError("Falta el Nro de ID que desea buscar en la tabla de INSCRIPTOS", "Delegacion", "Details"));
                //}
                //vInscripcionDetalle=db.vInscripcionDetalle.Where(m=>m.IdPersona == id && m.IdOficinasYDelegaciones == UsuarioDelegacion.IdOficinasYDelegaciones).ToList();
                //dataProblemas = db.DataProblemaEncontrado.Where(m => m.IdPostulantePersona == id).ToList();
                ////ViewBag.DataProblema == dataProblemas;
                //if (vInscripcionDetalle.Count == 0)
                //{
                //    return View("Error", Func.ConstruyeError("Incorrecta la llamada a la vista detalle con el id " + id.ToString() + " ==> NO EXISTE o no le corresponde verlo", "Delegacion", "Details"));
                //}
                return View(datos);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Details"));
            }

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
                db.spProximaSecuenciaEtapaEstado(0, datos.IdInscripcion, false, 0, "", "");


                MailConfirmacionEntrevista Modelo = new MailConfirmacionEntrevista
                {
                    Apellido = datos.Apellido,
                    FechaEntrevista = datos.FechaEntrevista
                };
                //verificar el llamado de una funcion asyncronica desde un metodo sincronico
                var Result = Func.EnvioDeMail(Modelo, "MailConfirmacionEntrevista", null, datos.IdPersona, "MailAsunto4",null);

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
                    Postulado = x
                };
                switch ((vInscripcionEtapas.Estado).ToString())
                {
                    case "Postulado":
                        bool envioP = await Func.EnvioDeMail(modeloPlanti, "MailPostulado", null, vInscripcionEtapas.IdPersona, "MailAsunto2",null);
                        db.spProximaSecuenciaEtapaEstado(0, id, false, 0, "", "");
                        break;
                    case "No Postulado":
                        bool envioNP = await Func.EnvioDeMail(modeloPlanti, "MailPostulado", null, vInscripcionEtapas.IdPersona, "MailAsunto2",null);
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
            var NyA = vdetalle.Nombres + " " + vdetalle.Apellido;
            ViewBag.NInscripcion = vdetalle.IdInscripcion;

            IDPersonaVM personaVM = new IDPersonaVM();
            personaVM.ID_PER = id;
            personaVM.NomyApe = NyA;

            var PantallasEstadoProblemas = new List<Array>();
            db.spTildarPantallaParaPostulate(personaVM.ID_PER).ForEach(m => PantallasEstadoProblemas.Add(new object[] { m.Pantalla, m.Abierta, m.CantComentarios }));
            personaVM.ListProblemaCantPantalla = PantallasEstadoProblemas;
            ViewBag.PantallasEstadoProblemas2 = JsonConvert.SerializeObject(PantallasEstadoProblemas);

            return View(personaVM);
        }
        [HttpPost]
        public ActionResult Documentacion(int? id)
        {
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapaEstado;
            Configuracion configuracion;
            List<vDataProblemaEncontrado> data;
            string cuerpo = "";
            try
            {
                vInscripcionEtapaEstado = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == id);
                configuracion = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpoDocumentacionValidado");
                cuerpo = configuracion.ValorDato.ToString();
                data = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == id).ToList();
                var PantallaCerradas = db.spTildarPantallaParaPostulate(id).Where(m => m.Abierta == true).ToList();
                if (PantallaCerradas.Count == 0)
                {
                    var modeloPlantilla = new ViewModels.MailDocumentacion
                    {
                        Etapa = vInscripcionEtapaEstado.Etapa,
                        MailCuerpo = cuerpo,
                        Apellido = vInscripcionEtapaEstado.Apellido,
                        Errores = data
                    };
                    var Result = Func.EnvioDeMail(modeloPlantilla, "MailDocumentacion", null, id, "MailAsunto4",null);
                    db.spProximaSecuenciaEtapaEstado(id, 0, false, 0, "", "");
                    return Json(new { View = "Index" });
                };
                return Json(new { success = true, msg = "Hay Pantallas no validadas"});
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }

        }

        public async Task<ActionResult> VolverEtapa(int? ID_persona)
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
                        Etapa = vInscripcionEtapaEstado.Etapa,
                        MailCuerpo = cuerpo,
                        Apellido = vInscripcionEtapaEstado.Apellido,
                        Errores = data
                    };
                    bool envioNP = await Func.EnvioDeMail(modeloPlantilla, "MailDocumentacion", null, ID_persona, "MailAsunto4");

                    db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "DOCUMENTACION", "Inicio De Carga");
                    return Json(new { View ="Index" });

                }
                return Json(new { success = true, msg = "Si el postulante no contiene problemas presione el boton confirmar" },JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
        }

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
                        Etapa = vInscripcionEtapaEstado.Etapa,
                        MailCuerpo = cuerpo,
                        Apellido = vInscripcionEtapaEstado.Apellido,
                        Errores = data
                    };
                    bool envioNP = await Func.EnvioDeMail(modeloPlantilla, "MailDocumentacion", null, ID_persona, "MailAsunto4",null);
                    db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "DOCUMENTACION", "No Validado");
                    //db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "", "");
                    //return RedirectToAction("Index");
                    return Json(new { View = "Index" });

                }
                return Json(new { success = true, msg = "Si el postulante no contiene problemas presione el boton confirmar" });
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
            //return Json(new { View = "Index" });
        }
        //fin del codigo
        public ActionResult DocPenal(int id, string docu)
        {
            try
            {
                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string Ubicacionfile = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string[] archivos = Directory.GetFiles(Ubicacionfile, id + "&" + docu + "*");
                if (archivos.Count()==0)
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


        public ActionResult GetAnexo2Pdf(int id)
        {
            try
            {
                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string NombreArchivo = id + "_Anexo2.pdf";
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
        [HttpPost]
        public ActionResult PresentacionAsignaFecha(string[] select, DateTime fecha)
        {

            try
            {
                // TODO: Add insert logic here
                foreach (var item in select)
                {
                    int x = Convert.ToInt32(item);
                    var da = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == x);
                    da.FechaRindeExamen = fecha;

                    db.spProximaSecuenciaEtapaEstado(x, 0, false, 0, "", "");

                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        public ActionResult ListaProblema(int ID_persona,int IdPanatlla)
        {
            List<vDataProblemaEncontrado> problema = db.vDataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona).Where(m=>m.IdPantalla== IdPanatlla).ToList();


            return View(problema);

        }
        [HttpGet]
        public ActionResult DataProblema(int? ID, int ID_persona)
        {
            try
            {
                ProblemaEcontradoVM problema = new ProblemaEcontradoVM
                {
                    ListDataVerificacionVM = new SelectList(db.DataVerificacion.Where(m=>m.IdPantalla==10).ToList(), "IdDataVerificacion", "Descripcion")
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
            var ProblemaExiste = db.DataProblemaEncontrado.FirstOrDefault(m => m.IdDataVerificacion == datos.DataProblemaEncontradoVM.IdDataVerificacion && m.IdPostulantePersona == datos.DataProblemaEncontradoVM.IdPostulantePersona);
            try
            {
                if (ProblemaExiste == null)
                {
                    var data = datos.DataProblemaEncontradoVM;
                    db.DataProblemaEncontrado.Add(data);
                    db.SaveChanges();
                    int idPantalla = db.DataVerificacion.Find(data.IdDataVerificacion).IdPantalla;
                    return Json(new { success = true, form = "Elimina", msg = "Problema Agregado", url_Tabla = "ProblemaPantalla", url_Controller = "Delegacion", IdPantalla = idPantalla }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, msg = "El problema que desea agregar ya existe" });
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
        [HttpPost]
        public ActionResult CerrarPantalla(int id, int IdPanatlla,int AoC)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    var TieneProblema = db.spTieneProblemasEnPantallaEstePostulate(id, IdPanatlla).ToList();
                    if (TieneProblema.ToList().First() == true)
                    {
                        return Json(new { success = false, msg = "No se puede cerrar la ventana por que tiene Problemas cargados" });
                    }
                    db.spCierraPantallaDePostulante(IdPanatlla, id,Convert.ToBoolean(AoC));
                    return Json(new { success = true, msg = "Se valido Correctamente los datos" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message });
            }

            return Json(new { success = false, msg = "Error en el Modelo Recibido" });
        }
        [HttpGet]
        public ActionResult DocumentosNecesarios(int IdPostulante)
        {
            var inscrip = db.vInscripcionDetalle.FirstOrDefault(m => m.IdPersona == IdPostulante);
            var DocuNecesarios = db.DocumentosNecesariosDelInscripto(inscrip.IdInscripcion).ToList();

            DocuNecesaria datos = new DocuNecesaria()
            {
                DocumentosNecesarios = DocuNecesarios,
            };
            var listDocu = DocuNecesarios.ToList();
            return PartialView(datos);
        }
        [HttpPost]
        public ActionResult DocumentosNecesarios(string[]select,int IdInscripto)
        {
            //db.spDocumentoInscripto(,)
            return View("Index");
        }


        /// <summary>
        /// /Aca se crea un action por que era necesario anteriormente se iba a utilizar un mismo action para 2 acciones que cumplia la misma funcion pero
        /// en una vista funcionaba correctamente y en la otra no para no tener tanto problema se crea esta accion igual a la la accion VolverEtapa
        /// </summary>
        /// <param name="ID_persona"></param>
        /// <returns></returns>
        public async Task<ActionResult> RestaurarPostulante(int? ID_persona)
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
                        Etapa = vInscripcionEtapaEstado.Etapa,
                        MailCuerpo = cuerpo,
                        Apellido = vInscripcionEtapaEstado.Apellido,
                        Errores = data
                    };
                    bool envioNP = await Func.EnvioDeMail(modeloPlantilla, "MailDocumentacion", null, ID_persona, "MailAsunto4");

                    db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "DOCUMENTACION", "Inicio De Carga");
                    return RedirectToAction("Index");

                }
                return Json(new { success = true, msg = "Si el postulante no contiene problemas presione el boton confirmar" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Delete"));
            }
        }

        //        //{
        //         try
        //            {
        //                // TODO: Add insert logic here
        //                foreach (var item in select)
        //                {
        //                      int x = Convert.ToInt32(item);
        //                      var da = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == x);
        //                      da.FechaRindeExamen = fecha;
        //                      db.spProximaSecuenciaEtapaEstado(x, 0, false, 0, "", "");

        //                }
        //    db.SaveChanges();
        //                return RedirectToAction("Index");
        //}
        //        //}



    }
}
