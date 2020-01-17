using System.Web.Mvc;

namespace SINU.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult AccionNoAutorizada()
        {
            return View("Error");
        }
    }
}