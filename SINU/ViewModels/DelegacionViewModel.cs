using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{
    public class DelegacionPostulanteVM
    {
        public List<vConsultaInscripciones> cargadatosbasicosVM { get; set; }
        public List<vConsultaInscripciones> PostulantesIncriptosVM { get; set; }
        public List<vConsultaInscripciones> EntrevistaVM { get; set; }
        public List<vConsultaInscripciones> DocumentacionVM { get; set; }
        public List<vConsultaInscripciones> PresentacionVM { get; set; }

    }
    #region ViewModel Utilizado para enviar datos(Envia una lista con items para un Dropdown - envia un listado con los problemas encontrados en un Postulante) a la vista Dataproblema.cshtml 
    public class ProblemaEcontradoVM
        {
            public vDataProblemaEncontrado vListDataProblemasVM { get; set; }
            public SelectList ListDataVerificacionVM { get; set; }
            public int ID_PER { get; set; }

        }
    #endregion
    public class ProblemaPantallaVM
    {
        public List<vDataProblemaEncontrado> ListvDataProblemaEncontradoVM { get; set; }
        public DataProblemaEncontrado DataProblemaEncontradoVM { get; set; }
        public List<DataVerificacion> DataVerificacionVM { get; set; }
    }
    public class RestaurarPostulanteVM
    {
        public List<vDataProblemaEncontrado> vDataProblemaEncontradoVM { get; set; }
        public List<vInscripcionDetalle> vInscripcionDetallesVM { get; set; }
    }
    public class DocuNecesaria
    {
        public List<DocumentosNecesariosDelInscripto_Result1> DocumentosNecesarios { get; set; }
    }
    public class ListadoPostulanteAsignarFecha
    {
        public List<vInscripcionEtapaEstadoUltimoEstado> AsignarFechaVM { get; set; }
        public SelectList LugarPresentacion { get; set; }
        public DateTime FechaPresentacion { get; set; }
        public string DatosLugar { get; set; }
        public int listado { get; set; }
    }
    public class PresentaciondelPostulante
    {
        public vInscripcionDetalleUltInsc DetalleInscripcion { get; set; }
        public string DatosLugar { get; set; }
        [Required(ErrorMessage ="Dato Requerido")]
        public SelectList LugarPresentacion { get; set; }

    }
}