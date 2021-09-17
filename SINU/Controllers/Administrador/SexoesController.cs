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
    public class SexoesController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: Sexoes
        public ActionResult Index()
        {
            return View(db.Sexo.ToList());
        }

        // GET: Sexoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexo.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            return View(sexo);
        }

        // GET: Sexoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sexoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdSexo,Descripcion")] Sexo sexo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Sexo.Add(sexo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Sexoes", "Create"));
            }
            return View(sexo);
        }

        // GET: Sexoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexo.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            return View(sexo);
        }

        // POST: Sexoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdSexo,Descripcion")] Sexo sexo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sexo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sexo);
        }

        // GET: Sexoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexo.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            return View(sexo);
        }

        // POST: Sexoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Sexo sexo = db.Sexo.Find(id);
                db.Sexo.Remove(sexo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Sexoes", "Edit"));
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
