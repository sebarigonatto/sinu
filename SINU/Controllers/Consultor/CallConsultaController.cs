﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.Controllers.Consultor
{
    /// <summary>Este Controlador CallConsulta depende de las Consultas Principales
    /// que se encuentran escritas en la tabla ConsultaProgramada.
    /// Cada método ACTION aquí tiene el nombre exacto que se carga
    /// en el campo ACTION de la tabla ConsultProgramada
    /// TODO registro en la tabla ConsultaProgramada debe tener un dato en ACTION
    /// que se encuentre en este Controller y si aun no se creo su ACTION
    /// se colocara el action por DEFUALT: FaltaCrearNuevoAction
    /// </summary>
    public class CallConsultaController : Controller
    { 
        private SINUEntities db = new SINUEntities();
        /// <summary>Pantalla principal que genera el indice lateral de Consultas
        /// 
        /// </summary>
        /// <param name="id">parameto opcional que es el nro de id de consulta que deseo 
        /// este activada o seleccionada</param>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            //Si no dan un id asume el que tenga id de consulta en 1, 
            //si no hay ninguno en id 1 simplemente no selecciona ninguno
            ViewBag.ActivarId = id ?? 1;
            return View(db.ConsultaProgramada.OrderBy(m => m.OrdenConsulta).ToList());
        }

        /// <summary>FaltaCrearNuevoAction Es una rutina modelo 
        /// que se usa como plantilla para crear nuevas actions para nuevas CONSULTAS PRINCIPALES.
        /// Tiene asociada una View que solo emite el mensaje que falta desarrollar elementos.
        /// 
        /// Si agregamos a la tabla ConsultasProgramadas nuevos registros ,
        /// en el campo CONTROLLER se colocara CallConsulta y 
        /// en el  campo ACTION se colocara el nombre de esta rutina 
        /// hasta que esté finalizada la programación de la que le corresponda.
        /// </summary>
        /// <returns></returns>
        public ActionResult FaltaCrearNuevoAction()
        {
            return PartialView();
        }

        /// <summary>TotalesPorModalidadyGenero es una CONSULTA Principal 
        /// que adentro tiene links hacia otras subconsultas
        /// Tiene el conteo por GENERO con Totales y subtotales por modalidad
        /// </summary>
        /// <returns></returns>
        public ActionResult TotalesPorModalidadyGenero()
        {
            List<sp_ConsultaInscriptosModalidadGenero_Result> Datos = db.sp_ConsultaInscriptosModalidadGenero().ToList();

            if (Datos == null)
            {
                return HttpNotFound();
            }
            return PartialView(Datos);
        }

        /// <summary>Esta rutina InscriptosPorModalidad es una SUBCONSULTA que es disparada desde un link  
        /// generado en la CONSULTA TotalesPorModalidadyGenero
        /// Si la ModalidadElegida es TODOS muestra todos los inscriptos sin discriminar
        /// Si la ModalidadElegida es otra cosa muestra los datos según la modalidad elegida
        /// </summary>
        /// <param name="ModalidadElegida">Este parametro es la modalidad por la cual desea ver los datos filtrados</param>
        /// <returns>Devuelve los inscriptos de la vista vConsultaInscripciones pero filtrado por modalidad dada</returns>
        public ActionResult InscriptosPorModalidad(string ModalidadElegida)
        {
            //busco el id que le corresponde a la consulta original TotalesPorModalidadyGenero
            ViewBag.ActivarId = db.ConsultaProgramada.Where(m => m.Action == "TotalesPorModalidadyGenero").Select(m => m.IdConsulta).FirstOrDefault();
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
            return View(Listado);
        }
        /// <summary>ConsultaTotalPostulantesEs una CONSULTA principal (hasta ahora sin subconsulta)
        /// que muestra un simple listado de los postulantes que se encuentra realmente 
        /// en la etapa de inscripcion más allá de la etapa 5 que equivale a todos aquellos
        /// que pasaron la VALIDACION DEL REGISTRO INCIAL y se encuentra en un estado
        /// de carga de DATOS BASICOS o posterior.
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsultaTotalPostulantes()
        {
            List<vInscripcionEtapaEstadoUltimoEstado> Todos;

            Todos = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.IdSecuencia >= 5).ToList();
            return PartialView(Todos);

        }


    }
}
