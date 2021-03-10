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
    public class Delegacion_EstablecExamenController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: Delegacion_EstablecExamen
        public ActionResult Index()
        {
            var delegacion_EstablecExamen = db.Delegacion_EstablecExamen.Include(d => d.EstablecimientoRindeExamen).Include(d => d.OficinasYDelegaciones);
            return View(delegacion_EstablecExamen.ToList());
        }

        // GET: Delegacion_EstablecExamen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delegacion_EstablecExamen delegacion_EstablecExamen = db.Delegacion_EstablecExamen.Find(id);
            if (delegacion_EstablecExamen == null)
            {
                return HttpNotFound();
            }
            return View(delegacion_EstablecExamen);
        }

        // GET: Delegacion_EstablecExamen/Create
        public ActionResult Create()
        {
            ViewBag.IdEstablecimientoRindeExamen = new SelectList(db.EstablecimientoRindeExamen, "IdEstablecimientoRindeExamen", "Jurisdiccion");
            ViewBag.idOficinasYDelegaciones = new SelectList(db.OficinasYDelegaciones, "IdOficinasYDelegaciones", "Nombre");
            return View();
        }

        // POST: Delegacion_EstablecExamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idOficinasYDelegaciones,idDelegacion_EstablecExamen,IdEstablecimientoRindeExamen")] Delegacion_EstablecExamen delegacion_EstablecExamen)
        {
            if (ModelState.IsValid)
            {
                db.Delegacion_EstablecExamen.Add(delegacion_EstablecExamen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdEstablecimientoRindeExamen = new SelectList(db.EstablecimientoRindeExamen, "IdEstablecimientoRindeExamen", "Jurisdiccion", delegacion_EstablecExamen.IdEstablecimientoRindeExamen);
            ViewBag.idOficinasYDelegaciones = new SelectList(db.OficinasYDelegaciones, "IdOficinasYDelegaciones", "Nombre", delegacion_EstablecExamen.idOficinasYDelegaciones);
            return View(delegacion_EstablecExamen);
        }

        // GET: Delegacion_EstablecExamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delegacion_EstablecExamen delegacion_EstablecExamen = db.Delegacion_EstablecExamen.Find(id);
            if (delegacion_EstablecExamen == null)
            {
                return HttpNotFound();
            }
            //ViewBag.IdEstablecimientoRindeExamen = new SelectList(db.EstablecimientoRindeExamen, "IdEstablecimientoRindeExamen", "Jurisdiccion", delegacion_EstablecExamen.IdEstablecimientoRindeExamen);
            //ViewBag.idOficinasYDelegaciones = new SelectList(db.OficinasYDelegaciones, "IdOficinasYDelegaciones", "Nombre", delegacion_EstablecExamen.idOficinasYDelegaciones);
            ViewBag.ListaEstablecimientoRindeExamen = new SelectList(db.EstablecimientoRindeExamen, "IdEstablecimientoRindeExamen", "Nombre", delegacion_EstablecExamen.IdEstablecimientoRindeExamen);
            ViewBag.ListaOficinasYDelegaciones = new SelectList(db.OficinasYDelegaciones, "IdOficinasYDelegaciones", "Nombre", delegacion_EstablecExamen.idOficinasYDelegaciones);
            return View(delegacion_EstablecExamen);
        }

        // POST: Delegacion_EstablecExamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idOficinasYDelegaciones,idDelegacion_EstablecExamen,IdEstablecimientoRindeExamen")] Delegacion_EstablecExamen delegacion_EstablecExamen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delegacion_EstablecExamen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdEstablecimientoRindeExamen = new SelectList(db.EstablecimientoRindeExamen, "IdEstablecimientoRindeExamen", "Jurisdiccion", delegacion_EstablecExamen.IdEstablecimientoRindeExamen);
            ViewBag.idOficinasYDelegaciones = new SelectList(db.OficinasYDelegaciones, "IdOficinasYDelegaciones", "Nombre", delegacion_EstablecExamen.idOficinasYDelegaciones);
            return View(delegacion_EstablecExamen);
        }

        // GET: Delegacion_EstablecExamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delegacion_EstablecExamen delegacion_EstablecExamen = db.Delegacion_EstablecExamen.Find(id);
            if (delegacion_EstablecExamen == null)
            {
                return HttpNotFound();
            }
            return View(delegacion_EstablecExamen);
        }

        // POST: Delegacion_EstablecExamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Delegacion_EstablecExamen delegacion_EstablecExamen = db.Delegacion_EstablecExamen.Find(id);
            db.Delegacion_EstablecExamen.Remove(delegacion_EstablecExamen);
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
