using System;
using System.Web.Mvc;

namespace SINU.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult AccionNoAutorizada()
        {
            //ver cambio de pantalla de error
            var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Esta cuenta no tiene permiso para realizar la accion deseada"), "ERROR", "AccionNoAutorizada");
            return View("Error", x);

            //return View("Error");
        }
    }
}