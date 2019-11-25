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
    }
}