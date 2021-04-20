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

namespace SINU.Controllers.Administrador.Convocatorias
{
    [AuthorizacionPermiso("AdminMenu")]
    public class CarreraOficiosController : Controller
    {
         
    
    private SINUEntities db = new SINUEntities();

        // GET: CarreraOficios
        public ActionResult Index()
        {
            List <CarreraOficio> carreraOficio = db.CarreraOficio.ToList();

            for (int i = 0; i < carreraOficio.Count(); i++)
            {
                switch (carreraOficio[i].Personal)
                {
                    case "O":
                        carreraOficio[i].Personal = "Oficiales";
                        break;
                    case "S":
                        carreraOficio[i].Personal = "Suboficiales";
                        break;
                    case "M":
                        carreraOficio[i].Personal = "Marineros";
                        break;
                }
            }

            
            return View(carreraOficio);

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
            switch (carreraOficio.Personal)
            {
                case "O":
                    carreraOficio.Personal = "Oficiales";
                    break;
                case "S":
                    carreraOficio.Personal = "Suboficiales";
                    break;
                case "M":
                    carreraOficio.Personal = "Marineros";
                    break;
            }
            ViewBag.Mensaje = "";
            if (carreraOficio == null)
            {
                return HttpNotFound();
            }
            if (carreraOficio.GrupoCarrOficio.Count > 0)
            {
                switch (carreraOficio.Personal)
                {
                    case "O":
                        carreraOficio.Personal = "Oficiales";
                        break;
                    case "S":
                        carreraOficio.Personal = "Suboficiales";
                        break;
                    case "M":
                        carreraOficio.Personal = "Marineros";
                        break;
                }
                var link = Url.Action("Index", "GrupoCarrOficios",null, protocol: Request.Url.Scheme);
                
                ViewBag.Mensaje = "Carrera u Oficio asignado a uno o varios Grupo Carrera/Oficio. Primero elimine la asignacion a dicho/s grupo. Para mas detalle ir al siguiente <a target='_blank' href='"+ link +"'>link</a>" ;
                
            }
            return View(carreraOficio);
        }

        // POST: CarreraOficios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CarreraOficio carreraOficio = db.CarreraOficio.Find(id);
            try
            {
                db.CarreraOficio.Remove(carreraOficio);
                db.SaveChanges();
                ViewBag.Mensaje = "Se elimino una Carrera u Oficio correctamente!!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error al eliminar la carrera.";
                return View();
            }
          
           
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
