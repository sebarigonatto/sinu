using SINU.Authorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//agregado por testeo
namespace SINU.Controllers
{
    [AuthorizacionPermiso("ListarRP")]
    public class ConsultorController : Controller
    {
        // GET: Consultor
        public ActionResult Index()
        {
            //return RedirectToAction("Index", "ConsultaProgramadas");
            return View();
        }
    }
}