using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.Controllers.Consultor
{
    public class CallConsultaController : Controller
    {
        // GET: CallConsulta
        //public ActionResult Index()
        //{
        //    return View();
        //}

        private SINUEntities db = new SINUEntities();

       

        public ActionResult TotalesPorModalidadyGenero()
        {
            List<sp_ConsultaInscriptosModalidadGenero_Result> Datos = db.sp_ConsultaInscriptosModalidadGenero().ToList();

            if (Datos == null)
            {
                return HttpNotFound();
            }
            return View(Datos);
        }
    }
}