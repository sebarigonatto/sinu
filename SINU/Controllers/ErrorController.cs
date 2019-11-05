using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SINU.Controllers
{
    public class ErrorController :Controller
    {
        // GET: Error
        public ActionResult AccionNoAutorizada()
        {
            return View("Error");
        }
    }
}