using SINU.Authorize;
using SINU.Models;
using SINU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net;
using System.Globalization;

namespace SINU.Controllers.Administrador.Convocatorias
{
    
    [AuthorizacionPermiso("AdminMenu")]
    public class ConvocatoriasController : Controller
    {
      
        private SINUEntities db = new SINUEntities();

        // GET: Convocatorias
        public ActionResult Index()
        {
            var convocatoria = db.Convocatoria.Include(c => c.GrupoCarrOficio).Include(c => c.Modalidad).Include(c => c.PeriodosInscripciones);
            return View(convocatoria.ToList());
        }

        // GET: Convocatorias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Convocatoria convocatoria = db.Convocatoria.Find(id);
            if (convocatoria == null)
            {
                return HttpNotFound();
            }
            return View(convocatoria);
        }

        // GET: Convocatorias/Create
        public ActionResult Create()
        {
            Convocatoria convoca = new Convocatoria();
            ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion");
            ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion");
            ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion");

            return View(convoca);
        }

        // POST: Convocatorias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPeriodoInscripcion,IdModalidad,IdGrupoCarrOficio,IdConvocatoria,ff")] Convocatoria convocatoria)
        {
            
            //if (ModelState.IsValid)
            //{
            //    convocatoria.Fecha_Inicio_Proceso = DateTime.Today;
            //    convocatoria.Fecha_Fin_Proceso = DateTime.Parse(convocatoria.ff);         
            //    db.Convocatoria.Add(convocatoria);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion", convocatoria.IdGrupoCarrOficio);
            //ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion", convocatoria.IdModalidad);
            //ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion", convocatoria.IdPeriodoInscripcion);
            return View(convocatoria);
        }

        // GET: Convocatorias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Convocatoria convocatoria = db.Convocatoria.Find(id);
            if (convocatoria == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion", convocatoria.IdGrupoCarrOficio);
            ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion", convocatoria.IdModalidad);
            /* generando una selectlist con el detalle de los periodos de inscripcion de id periodo, fecha de inicio y fin*/
            ViewBag.PeriododeInscripcion = new SelectList(from p in db.PeriodosInscripciones.ToList()
                                                          select new SelectListItem
                                                          {
                                                              Text = p.IdPeriodoInscripcion.ToString()+ ": " + p.Institucion.NombreInst + " [" + p.FechaInicio.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " , " + p.FechaFinal.ToString("dd/MM/yyyy") + " ]",
                                                              Value = p.IdPeriodoInscripcion.ToString()
                                                          }, "Value", "Text",convocatoria.IdPeriodoInscripcion);

            ViewBag.FechaFinProc = convocatoria.Fecha_Fin_Proceso;
            ViewBag.fechaInscripcion = convocatoria.IdPeriodoInscripcion;
           
            return View(convocatoria);
        }

        // POST: Convocatorias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPeriodoInscripcion,IdModalidad,IdGrupoCarrOficio,IdConvocatoria,Fecha_Fin_Proceso,Fecha_Inicio_Proceso")] Convocatoria convocatoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(convocatoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion", convocatoria.IdGrupoCarrOficio);
            ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion", convocatoria.IdModalidad);
            ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion", convocatoria.IdPeriodoInscripcion);
            return View(convocatoria);
        }

        // GET: Convocatorias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Convocatoria convocatoria = db.Convocatoria.Find(id);
            if (convocatoria == null)
            {
                return HttpNotFound();
            }
            return View(convocatoria);
        }

        // POST: Convocatorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Convocatoria convocatoria = db.Convocatoria.Find(id);
            db.Convocatoria.Remove(convocatoria);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult DevolverFechas(int RegionId)
        {
            using (db = new SINUEntities())
            {

                //ViewBag.FechasInicio = db.PeriodosInscripciones.Where(x => x.IdPeriodoInscripcion == RegionId).Select(x => x.FechaInicio);
                //ViewBag.FechasFinal = db.PeriodosInscripciones.Where(x => x.IdPeriodoInscripcion == RegionId).Select(x => x.FechaFinal);
                var FechasIF = db.PeriodosInscripciones.Where(x=> x.IdPeriodoInscripcion == RegionId).Select(m => new SelectListItem
                {
                    
                    //Value = m.FechaFinal.ToString(),                    
                    Text = m.FechaInicio.ToString() + " / " + m.FechaFinal.ToString()
                }).ToList();
                return Json(FechasIF, JsonRequestBehavior.AllowGet);
                
            }
        }

        public JsonResult DevolverCArrerasFiltradas(string ModalidadId)
        {
            using (db = new SINUEntities())
            {
                if (ModalidadId!="")
                {

                var ModTipoPersonal = db.Modalidad.Where(x => x.IdModalidad == ModalidadId).Select(m => m.Personal).ToList();
                string y = ModTipoPersonal[0].ToString();
                var grupo_carreras = db.GrupoCarrOficio.Where(x => x.Personal == y).Select(m => new SelectListItem
                {Value = m.IdGrupoCarrOficio,
                Text=m.Descripcion
                }).ToList();
                
                return Json(grupo_carreras, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
                //carrerasFiltradas
            }
        }

        public JsonResult FiltrarModalidadIDGrupoCArreraOficio(string ModalidadId)
        {


            using (db = new SINUEntities())

               
            {
                if (ModalidadId != "")
                {

                    var ModTipoPersonal = db.Modalidad.Where(x => x.IdModalidad == ModalidadId).Select(m => m.Personal).ToList();
                    string y = ModTipoPersonal[0].ToString();
                    var grupo_carreras = db.GrupoCarrOficio.Where(x => x.Personal == y).Select(m => new SelectListItem
                    {
                        Value = m.IdGrupoCarrOficio,
                        Text = m.Descripcion
                    }).ToList();

                    return Json(grupo_carreras, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
                //carrerasFiltradas
            }
        }
    }
}
