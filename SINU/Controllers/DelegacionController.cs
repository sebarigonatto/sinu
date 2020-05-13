using SINU.Authorize;
using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SINU.ViewModels;

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
                    EntrevistaVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "ENTREVISTA"  && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList(),
                    DocumentacionVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "DOCUMENTACION"  && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList(),
                    PresentacionVM = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.Etapa == "PRESENTACION" && m.IdDelegacionOficinaIngresoInscribio == UsuarioDelegacion.IdOficinasYDelegaciones).ToList()
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
            List<vInscripcionDetalle> InscripcionElegida;
            vInscripcionEtapaEstadoUltimoEstado vInscripcionEtapas;
            vSecuencia_EtapaEstado vSecuencia_EtapaEstado;
            try
            {
                //busco la delegacion que pertenece al usuario con perfil de delegacion            
                UsuarioDelegacion = db.Usuario_OficyDeleg.Find(User.Identity.Name).OficinasYDelegaciones;
                ViewBag.Delegacion = UsuarioDelegacion.Nombre;


                if (id == null)
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);               
                    return View("Error", Func.ConstruyeError("Falta el Nro de ID que desea buscar en la tabla de INSCRIPTOS", "Delegacion", "Details"));
                }

                InscripcionElegida = db.vInscripcionDetalle.Where(m => m.IdInscripcion == id && m.IdOficinasYDelegaciones == UsuarioDelegacion.IdOficinasYDelegaciones).ToList();
                vInscripcionEtapas = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdInscripcionEtapaEstado == id);
                ViewBag.Secuen = vInscripcionEtapas.IdSecuencia;

                //Se creo una variable para poder guardar el id secuencia y luego para saber cual es el paso anterior
                int secu = vInscripcionEtapas.IdSecuencia; 
                vSecuencia_EtapaEstado = db.vSecuencia_EtapaEstado.FirstOrDefault(m => m.IdSecuencia == secu);
                ViewBag.Ant = vSecuencia_EtapaEstado.Anterior;

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
            return View(InscripcionElegida.ToList()[0]);

        }
        //Terminar codigo faltante no devuelve nada la vista investigar  por que
        [HttpPost]
        public ActionResult Details(vInscripcionDetalle vInscripcion)
        {
            try
            {
                db.spProximaSecuenciaEtapaEstado(vInscripcion.IdPersona, vInscripcion.IdInscripcion);
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Index"));
            }
            return RedirectToAction("Index");
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
            try
            {
                // TODO: Add insert logic here

                var da = db.Inscripcion.Find(datos.IdInscripcion);
                da.FechaEntrevista = datos.FechaEntrevista;
                db.SaveChanges();
                db.spProximaSecuenciaEtapaEstado(datos.IdPersona, datos.IdInscripcion);
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }

        public ActionResult EntrevistaConfirmaFecha(int id)
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
        public ActionResult EntrevistaConfirmaFecha(vEntrevistaLugarFecha datos)
        {
            try
            {

                db.spProximaSecuenciaEtapaEstado(datos.IdPersona, datos.IdInscripcion);

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Delegacion", "Create"));
            }
        }
        // GET: Delegacion/Edit/5
        public ActionResult Edit(int? id)
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
    }
}
