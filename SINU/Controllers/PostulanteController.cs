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
    
    //[Authorize]
    public class PostulanteController : Controller
    {
        SINUEntities db = new SINUEntities();
        // GET: Postulante
        //[AuthorizacionPermiso] 
        
        public ActionResult Index()
        {
            string mailactual = HttpContext.User.Identity.Name;
            ViewBag.secuenciaactual = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.Email == mailactual).IdSecuencia;
            return View();
        }
        public ActionResult DatosBasicos()
        {
            try
            {
                //guardo el mail del usuario que esta logueado
                var mail= HttpContext.User.Identity.Name.ToString();
                DatosBasicos datosba = new DatosBasicos();
                datosba.Sexo = db.Sexo.ToList();
                datosba.vPeriodosInscrips = db.vPeriodosInscrip.ToList();
                datosba.OficinasYDelegaciones = db.OficinasYDelegaciones.ToList();
                //levanto el registro perteneciente al usuario logueado
                datosba.vPersona_DatosBasicos=db.vPersona_DatosBasicos.FirstOrDefault(b => b.Email == mail);
                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return View();
            }
            
        }
     
        public ActionResult Entrevista()
        {
            vEntrevistaLugarFecha entrevistafh = new vEntrevistaLugarFecha();

            int idperson = db.vPersona_DatosBasicos.FirstOrDefault(m => m.Email == HttpContext.User.Identity.Name).IdPersona;
            entrevistafh = db.vEntrevistaLugarFecha.FirstOrDefault(m => m.IdPersona==idperson);
            //se carga los texto parametrizados desde la tabla configuracion
            string[] consideraciones = { 
                db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTitulo").ValorDato.ToString(),
                db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTexto").ValorDato.ToString() 
            };
            ViewBag.Considere = consideraciones;

            return PartialView(entrevistafh);
        }

        [HttpPost]
        public JsonResult GuardaDatosBasicos(DatosBasicos person)
        {
            
            return Json(person);
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
