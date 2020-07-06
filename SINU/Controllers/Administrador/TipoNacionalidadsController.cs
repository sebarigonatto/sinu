using SINU.Authorize;
using SINU.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;

namespace SINU.Controllers.Administrador
{
    [AuthorizacionPermiso("CRUDParam")]
    public class TipoNacionalidadsController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: TipoNacionalidads
        public ActionResult Index()
        {
            return View(db.TipoNacionalidad.ToList());
        }

        // GET: TipoNacionalidads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoNacionalidad tipoNacionalidad = db.TipoNacionalidad.Find(id);
            if (tipoNacionalidad == null)
            {
                return HttpNotFound();
            }
            return View(tipoNacionalidad);
        }

        // GET: TipoNacionalidads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoNacionalidads/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Tipo,Descripcion,IdTipoNacionalidad")] TipoNacionalidad tipoNacionalidad)
        {
            if (ModelState.IsValid)
            {
                db.TipoNacionalidad.Add(tipoNacionalidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoNacionalidad);
        }

        // GET: TipoNacionalidads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoNacionalidad tipoNacionalidad = db.TipoNacionalidad.Find(id);
            if (tipoNacionalidad == null)
            {
                return HttpNotFound();
            }
            return View(tipoNacionalidad);
        }

        // POST: TipoNacionalidads/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Tipo,Descripcion,IdTipoNacionalidad")] TipoNacionalidad tipoNacionalidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoNacionalidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoNacionalidad);
        }

        // GET: TipoNacionalidads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoNacionalidad tipoNacionalidad = db.TipoNacionalidad.Find(id);
            if (tipoNacionalidad == null)
            {
                return HttpNotFound();
            }
            return View(tipoNacionalidad);
        }

        // POST: TipoNacionalidads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                TipoNacionalidad tipoNacionalidad = db.TipoNacionalidad.Find(id);
                db.TipoNacionalidad.Remove(tipoNacionalidad);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "TipoNacionalidad", "Delete"));
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
