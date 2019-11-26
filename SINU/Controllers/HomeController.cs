using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SINU.Models;
using SINU.ViewModels;

namespace SINU.Controllers
{
    public class HomeController : Controller
    {
        SINUEntities db= new SINUEntities();
        public ActionResult Index()
        {
            ViewBag.TextoPAGINAPRINCIPAL = db.Configuracion.FirstOrDefault(b => b.NombreDato == "TextoPAGINAPRINCIPAL").ValorDato;
            return View(db.vPeriodosInscrip.ToList());
        }
       
        public ActionResult About()
        {
            

            return View();
        }
     
       

        public ActionResult Contact()
        {
            vContacto myModel = new vContacto();
            myModel.Configuracion = db.Configuracion.ToList();
            myModel.listoficinas = db.OficinasYDelegaciones.ToList();
            return View(myModel);
        }

        public ActionResult Vaciar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Vaciar(string email)
        {
            try
            {
                var res = db.Vaciar(email, 1);
            }
            catch (Exception ex)
            {
                ViewBag.vaciar = ex;
                return View();
            }
            ViewBag.vaciar = "Exito, en la eliminacion de los registros del correo" + email;
            return View();
        }
    }
}