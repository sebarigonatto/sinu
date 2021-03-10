using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.Controllers.Administrador
{
    public class EstablecimientoRindeExamenController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: EstablecimientoRindeExamen
        public ActionResult Index()
        {
            // Se crea el listado de Establecimientos, exceptuando el registro "No Asignado",
            //debido a que cuando se crea un postulante, se relaciona por defecto con el registro "No asignado"
            var Establecimientos = db.EstablecimientoRindeExamen.Where(m => m.Nombre != m.Jurisdiccion).ToList();
            return View(Establecimientos);
        }

        // GET: EstablecimientoRindeExamen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstablecimientoRindeExamen establecimientoRindeExamen = db.EstablecimientoRindeExamen.Find(id);
            if (establecimientoRindeExamen == null)
            {
                return HttpNotFound();
            }
            return View(establecimientoRindeExamen);
        }

        // GET: EstablecimientoRindeExamen/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstablecimientoRindeExamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Jurisdiccion,Localidad,Departamento,Nombre,Comentario,Direccion,ACTIVO,IdEstablecimientoRindeExamen")] EstablecimientoRindeExamen establecimientoRindeExamen)
        {
            if (ModelState.IsValid)
            {
                if (establecimientoRindeExamen.Comentario == null) { establecimientoRindeExamen.Comentario = ""; }
                db.EstablecimientoRindeExamen.Add(establecimientoRindeExamen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(establecimientoRindeExamen);
        }

        // GET: EstablecimientoRindeExamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstablecimientoRindeExamen establecimientoRindeExamen = db.EstablecimientoRindeExamen.Find(id);
            if (establecimientoRindeExamen == null)
            {
                return HttpNotFound();
            }
            return View(establecimientoRindeExamen);
        }

        // POST: EstablecimientoRindeExamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Jurisdiccion,Localidad,Departamento,Nombre,Comentario,Direccion,ACTIVO,IdEstablecimientoRindeExamen")] EstablecimientoRindeExamen establecimientoRindeExamen)
        {
            if (ModelState.IsValid)
            {
                if (establecimientoRindeExamen.Comentario == null) { establecimientoRindeExamen.Comentario = ""; }
                db.Entry(establecimientoRindeExamen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(establecimientoRindeExamen);
        }

        // GET: EstablecimientoRindeExamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstablecimientoRindeExamen establecimientoRindeExamen = db.EstablecimientoRindeExamen.Find(id);
            if (establecimientoRindeExamen == null)
            {
                return HttpNotFound();
            }
            return View(establecimientoRindeExamen);
        }

        // POST: EstablecimientoRindeExamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EstablecimientoRindeExamen establecimientoRindeExamen = db.EstablecimientoRindeExamen.Find(id);
            db.EstablecimientoRindeExamen.Remove(establecimientoRindeExamen);
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
    }
}
