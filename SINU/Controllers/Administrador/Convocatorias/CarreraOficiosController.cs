﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.Controllers.Administrador.Convocatorias
{
    public class CarreraOficiosController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: CarreraOficios
        public ActionResult Index()
        {
            return View(db.CarreraOficio.ToList());
        }

        // GET: CarreraOficios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarreraOficio carreraOficio = db.CarreraOficio.Find(id);
            if (carreraOficio == null)
            {
                return HttpNotFound();
            }
            return View(carreraOficio);
        }

        // GET: CarreraOficios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CarreraOficios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCarreraOficio,CarreraUoficio,Personal")] CarreraOficio carreraOficio)
        {
            if (ModelState.IsValid)
            {
                db.CarreraOficio.Add(carreraOficio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(carreraOficio);
        }

        // GET: CarreraOficios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarreraOficio carreraOficio = db.CarreraOficio.Find(id);
            if (carreraOficio == null)
            {
                return HttpNotFound();
            }
            return View(carreraOficio);
        }

        // POST: CarreraOficios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCarreraOficio,CarreraUoficio,Personal")] CarreraOficio carreraOficio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(carreraOficio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(carreraOficio);
        }

        // GET: CarreraOficios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarreraOficio carreraOficio = db.CarreraOficio.Find(id);
            if (carreraOficio == null)
            {
                return HttpNotFound();
            }
            return View(carreraOficio);
        }

        // POST: CarreraOficios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CarreraOficio carreraOficio = db.CarreraOficio.Find(id);
            db.CarreraOficio.Remove(carreraOficio);
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
