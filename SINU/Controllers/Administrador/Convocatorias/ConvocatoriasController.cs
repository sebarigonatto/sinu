﻿using SINU.Authorize;
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

namespace SINU.Controllers.Administrador.Convocatorias
{
    //[AuthorizacionPermiso("AdminMenu")]
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
            ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion");
            ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion");
            ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion");
            return View();
        }

        // POST: Convocatorias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPeriodoInscripcion,IdModalidad,IdGrupoCarrOficio,IdConvocatoria")] Convocatoria convocatoria)
        {
            if (ModelState.IsValid)
            {              
                db.Convocatoria.Add(convocatoria);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion", convocatoria.IdGrupoCarrOficio);
            ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion", convocatoria.IdModalidad);
            ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion", convocatoria.IdPeriodoInscripcion);
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
            ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion", convocatoria.IdPeriodoInscripcion);
            return View(convocatoria);
        }

        // POST: Convocatorias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPeriodoInscripcion,IdModalidad,IdGrupoCarrOficio,IdConvocatoria")] Convocatoria convocatoria)
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
    }
}
