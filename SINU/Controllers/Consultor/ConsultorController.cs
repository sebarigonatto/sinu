using SINU.Authorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//agregado por testeo
namespace SINU.Controllers
{
    /// <summary>Todos los Perfiles tienen su Controlador con el nombre del perfil
    /// Este es el controlador principal del perfil Consultor
    /// </summary>
    [AuthorizacionPermiso("ListarRP")]
    public class ConsultorController : Controller
    {
        // GET: Consultor
        public ActionResult Index()
        {
            //busco el id de la primer consulta que aparece en la tabla de consultas ordenada por el orden de la consulta 
            int ActivarId = (new Models.SINUEntities()).ConsultaProgramada.OrderBy(m => m.OrdenConsulta).Where(m => m.OrdenConsulta>= 1).Select(m => m.IdConsulta).FirstOrDefault();

            return RedirectToAction("Index", "CallConsulta",new { id = ActivarId });
            //debido a variaciones en el desarrollo del consultor todas las consultas se encuentran
            //desarrolladas en el controlador en CallConsulta
        }
    }
}
