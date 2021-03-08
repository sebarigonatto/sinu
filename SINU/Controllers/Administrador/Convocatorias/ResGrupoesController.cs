using System;
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
    public class ResGrupoesController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: ResGrupoes
        public ActionResult Index()
        {
            return View(db.ResGrupo.ToList());
        }

        // GET: ResGrupoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResGrupo resGrupo = db.ResGrupo.Find(id);
            if (resGrupo == null)
            {
                return HttpNotFound();
            }
            return View(resGrupo);
        }

        // GET: ResGrupoes/Create
        static int lent;
        public ActionResult Create(int Length)
        {
            lent = Length;
            return View();
        }

        // POST: ResGrupoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Edad_Al_AnioSig,Edad_Al_Dia,Edad_Al_Mes,EdadMinCAutoriz,EdadMin,EdadMax,IdEstadoCivil,Edad_a_fecha,IdResGrupo")] ResGrupo resGrupo)
        {
            if (ModelState.IsValid)
            {
                db.ResGrupo.Add(resGrupo);
                db.SaveChanges();
                if (lent == 4 && GrupoCarrOficiosController.le == 5) return Redirect("../GrupoCarrOficios/Create?Length=5");
                if (lent == 4 && GrupoCarrOficiosController.le == 1) return Redirect("../GrupoCarrOficios/Create?Length=1");
                if (lent == 2 && GrupoCarrOficiosController.Listint == 1)
                {
                    string liid = GrupoCarrOficiosController.Listid;
                    GrupoCarrOficiosController.Listint = -1;
                    return Redirect("../GrupoCarrOficios/Edit/" + liid);
                }
                return RedirectToAction("Index");
            }

            return View(resGrupo);
        }

        // GET: ResGrupoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResGrupo resGrupo = db.ResGrupo.Find(id);
            if (resGrupo == null)
            {
                return HttpNotFound();
            }
            return View(resGrupo);
        }

        // POST: ResGrupoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Edad_Al_AnioSig,Edad_Al_Dia,Edad_Al_Mes,EdadMinCAutoriz,EdadMin,EdadMax,IdEstadoCivil,Edad_a_fecha,IdResGrupo")] ResGrupo resGrupo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resGrupo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(resGrupo);
        }

        // GET: ResGrupoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResGrupo resGrupo = db.ResGrupo.Find(id);
            if (resGrupo == null)
            {
                return HttpNotFound();
            }
            return View(resGrupo);
        }

        // POST: ResGrupoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResGrupo resGrupo = db.ResGrupo.Find(id);
            db.ResGrupo.Remove(resGrupo);
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
