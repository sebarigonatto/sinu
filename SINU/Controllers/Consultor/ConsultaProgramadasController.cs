using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SINU.Models;
using SINU.Authorize;

namespace Consulta.Controllers
{
    [Authorize]
    [AuthorizacionPermiso("ListarRP")]
    public class ConsultaProgramadasController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: ConsultaProgramadas
        public ActionResult Index()
        {
            return View(db.ConsultaProgramada.OrderBy(m => m.OrdenConsulta).ToList());
        }

        // GET: ConsultaProgramadas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConsultaProgramada consultaProgramada = db.ConsultaProgramada.Find(id);
            if (consultaProgramada == null)
            {
                return HttpNotFound();
            }
            return View(consultaProgramada);
        }

        // GET: ConsultaProgramadas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConsultaProgramadas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdConsulta,NombreMenu,DescripcionConsulta,OrdenConsulta,Link,Controller,Action,Parametros")] ConsultaProgramada consultaProgramada)
        {
            if (ModelState.IsValid)
            {
                db.ConsultaProgramada.Add(consultaProgramada);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(consultaProgramada);
        }

        // GET: ConsultaProgramadas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConsultaProgramada consultaProgramada = db.ConsultaProgramada.Find(id);
            if (consultaProgramada == null)
            {
                return HttpNotFound();
            }
            return View(consultaProgramada);
        }

        // POST: ConsultaProgramadas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdConsulta,NombreMenu,DescripcionConsulta,OrdenConsulta,Link,Controller,Action,Parametros")] ConsultaProgramada consultaProgramada)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consultaProgramada).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(consultaProgramada);
        }

        // GET: ConsultaProgramadas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConsultaProgramada consultaProgramada = db.ConsultaProgramada.Find(id);
            if (consultaProgramada == null)
            {
                return HttpNotFound();
            }
            return View(consultaProgramada);
        }

        // POST: ConsultaProgramadas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConsultaProgramada consultaProgramada = db.ConsultaProgramada.Find(id);
            db.ConsultaProgramada.Remove(consultaProgramada);
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
