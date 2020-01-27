using SINU.Authorize;
using SINU.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SINU.Controllers.Administrador
{
    [AuthorizacionPermiso("CRUDParam")]
    public class OficinasYDelegacionesController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: OficinasYDelegaciones
        public ActionResult Index()
        {
            return View(db.OficinasYDelegaciones.ToList());
        }

        // GET: OficinasYDelegaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OficinasYDelegaciones oficinasYDelegaciones = db.OficinasYDelegaciones.Find(id);
            if (oficinasYDelegaciones == null)
            {
                return HttpNotFound();
            }
            return View(oficinasYDelegaciones);
        }

        // GET: OficinasYDelegaciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OficinasYDelegaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre,Descripcion,Direccion,Localidad,Provincia,CP,Telefono,Celular,Email1,Email2,IdOficinasYDelegaciones")] OficinasYDelegaciones oficinasYDelegaciones)
        {
            if (ModelState.IsValid)
            {
                db.OficinasYDelegaciones.Add(oficinasYDelegaciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(oficinasYDelegaciones);
        }

        // GET: OficinasYDelegaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OficinasYDelegaciones oficinasYDelegaciones = db.OficinasYDelegaciones.Find(id);
            if (oficinasYDelegaciones == null)
            {
                return HttpNotFound();
            }
            return View(oficinasYDelegaciones);
        }

        // POST: OficinasYDelegaciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Nombre,Descripcion,Direccion,Localidad,Provincia,CP,Telefono,Celular,Email1,Email2,IdOficinasYDelegaciones")] OficinasYDelegaciones oficinasYDelegaciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oficinasYDelegaciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oficinasYDelegaciones);
        }

        // GET: OficinasYDelegaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OficinasYDelegaciones oficinasYDelegaciones = db.OficinasYDelegaciones.Find(id);
            if (oficinasYDelegaciones == null)
            {
                return HttpNotFound();
            }
            return View(oficinasYDelegaciones);
        }

        // POST: OficinasYDelegaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OficinasYDelegaciones oficinasYDelegaciones = db.OficinasYDelegaciones.Find(id);
            db.OficinasYDelegaciones.Remove(oficinasYDelegaciones);
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
