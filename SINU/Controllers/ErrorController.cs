using System;
using System.Web.Mvc;

namespace SINU.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: Error
        /// <summary>Aca llega desde el AuthorizacionPermiso
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AccionNoAutorizada()
        {
            //ver cambio de pantalla de error
            var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Esta cuenta no tiene permiso para realizar la accion deseada"), "ERROR", "AccionNoAutorizada");
            return View("Error", x);

            //return View("Error");
        }
        public ActionResult ErrorGeneral(System.Web.HttpContextBase httpContext)
        {
            
            //ver cambio de pantalla de error
            var x = new System.Web.Mvc.HandleErrorInfo(httpContext.Error, "Error", "ErrorGeneral");
            return View("Error", x);

            //return View("Error");
        }
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200
            return View("NotFound");
        }
        public ViewResult Forbidden()
        {
            Response.StatusCode = 403;  //you may want to set this to 200
            return View("Forbidden");
        }
    }
}