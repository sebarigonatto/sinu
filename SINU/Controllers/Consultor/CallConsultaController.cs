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
            return PartialView(Datos);
        }
        /// <summary>Esta rutina es una subconsulta que es disparada desde un link  
        /// generado en la consulta TotalesPorModalidadyGenero
        /// Si la ModalidadElegida es TODOS muestra todos los inscriptos sin discriminar
        /// Si la ModalidadElegida es otra cosa muestra los datos según la modalidad elegida
        /// </summary>
        /// <param name="ModalidadElegida">Este parametro es la modalidad por la cual desea ver los datos filtrados</param>
        /// <returns>Devuelve los inscriptos de la vista vConsultaInscripciones pero filtrado por modalidad dada</returns>
        public ActionResult InscriptosPorModalidad(string ModalidadElegida)
        {
          //si la modalidad elegida es string, el signo ?? Verifica si esta nula dicha var, asignandole lo q sigue a ella, en este caso "" , de lo contrario queda con su valor original
            ModalidadElegida = ModalidadElegida ?? "";
            List<vConsultaInscripciones> Listado;
            if (ModalidadElegida == "TODOS")
            {
                Listado = db.vConsultaInscripciones.ToList();
                ViewBag.ModalidadElegida = "Todas las Modalidades";
            }
            else
            {
                Listado = db.vConsultaInscripciones.Where(m => m.Modalidad_Siglas == ModalidadElegida).ToList();
                ViewBag.modalidadElegida = ModalidadElegida;
            }
            return PartialView(Listado);
        }
        public ActionResult ConsultaTotalPostulantes()
        {
            List<vInscripcionEtapaEstadoUltimoEstado> Todos;
           
            Todos = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.IdSecuencia >= 5).ToList();
            return PartialView(Todos);

        }
       

    }
}