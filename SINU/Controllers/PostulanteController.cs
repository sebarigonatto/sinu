using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Authorize;

using SINU.Models;
using SINU.ViewModels;

namespace SINU.Controllers
{
    [Authorize]
    public class PostulanteController : Controller
    {
        SINUEntities db = new SINUEntities();
        // GET: Postulante
        //[AuthorizacionPermiso]
        public ActionResult Index()
        {
       
            var mail= HttpContext.User.Identity.Name.ToString();
            DatosBasicos datosba = new DatosBasicos();
            datosba.Sexo = db.Sexo.ToList();
            datosba.vPeriodosInscrips = db.vPeriodosInscrip.ToList();
            datosba.vPersona_DatosBasicos=db.vPersona_DatosBasicos.FirstOrDefault(b => b.Email == mail);
            return View(datosba);
        }
              
        public ActionResult Secuencias()
        {

            return View();
        }

        // GET: Postulante/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Postulante/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Postulante/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Postulante/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Postulante/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Postulante/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
