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
using System;
using SINU.ViewModels;
using System.Collections.Generic;


namespace SINU.Controllers.Administrador
{
    //[AuthorizacionPermiso("CRUDParam")] //TODO De OTTINO:esto en algun momento lo vas a desmarcar no?
    public class ModalidadsController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: Modalidads
        public ActionResult Index()
        {
            List<Modalidad> modalidad  = db.Modalidad.ToList();

            for (int i = 0; i < modalidad.Count(); i++)
            {
                switch (modalidad[i].Personal)
                {
                    case "O":
                        modalidad[i].Personal = "Oficiales";
                        break;
                    case "S":
                        modalidad[i].Personal = "Suboficiales";
                        break;
                    case "M":
                        modalidad[i].Personal = "Marineros";
                        break;
                }
            }
            return View(modalidad);
        }

        // GET: Modalidads/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                //TODO De OTTINO: comentario para hacer ..cambiar esto
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Modalidad modalidad = db.Modalidad.Find(id);
            List<CarreraOficio> carreraOficio = db.CarreraOficio.ToList();

           
                switch (modalidad.Personal)
                {
                    case "O":
                        modalidad.Personal = "Oficiales";
                        break;
                    case "S":
                    modalidad.Personal = "Suboficiales";
                        break;
                    case "M":
                    modalidad.Personal = "Marineros";
                        break;
                }
            
            if (modalidad == null)
            {
                return HttpNotFound();
            }
            return View(modalidad);
        }

        // GET: Modalidads/Create
        public ActionResult Create()
        {
            //TODO De OTTINO: esto lo haremos con mucho cuidado
            return View();
        }

        // POST: Modalidads/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdModalidad,Descripcion,Personal")] Modalidad modalidad)
        {
            if (ModelState.IsValid)
            {
                db.Modalidad.Add(modalidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(modalidad);
        }

        // GET: Modalidads/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Modalidad modalidad = db.Modalidad.Find(id);
            if (modalidad == null)
            {
                return HttpNotFound();
            }
            return View(modalidad);
        }

        // POST: Modalidads/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdModalidad,Descripcion,Personal")] Modalidad modalidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modalidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modalidad);
        }

        // GET: Modalidads/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Modalidad modalidad = db.Modalidad.Find(id);
            if (modalidad == null)
            {
                return HttpNotFound();
            }
            return View(modalidad);
        }

        // POST: Modalidads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Modalidad modalidad = db.Modalidad.Find(id);
            db.Modalidad.Remove(modalidad);
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
