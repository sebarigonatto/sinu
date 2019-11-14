using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.Controllers
{
    public class HomeController : Controller
    {
        private SINUEntities db = new SINUEntities();

        public ActionResult Index()
        {
            return View(db.vPeriodosInscrip.ToList());
        }
      
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}