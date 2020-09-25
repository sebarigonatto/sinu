using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{
    public class DelegacionPostulanteVM
    {
        public List<vInscripcionEtapaEstadoUltimoEstado> cargadatosbasicosVM { get; set; }
        public List<vInscripcionEtapaEstadoUltimoEstado> PostulantesIncriptosVM { get; set; }
        public List<vInscripcionEtapaEstadoUltimoEstado> EntrevistaVM { get; set; }
        public List<vInscripcionEtapaEstadoUltimoEstado> DocumentacionVM { get; set; }
        public List<vInscripcionEtapaEstadoUltimoEstado> PresentacionVM { get; set; }
       
    }
    public class ProblemaEcontradoVM
    {
        public vDataProblemaEncontrado vListDataProblemasVM { get; set; }
        public SelectList ListDataVerificacionVM { get; set; }
        public int ID_PER { get; set; }
       
    }
    public class ProblemaPantallaVM
    {
        public List<vDataProblemaEncontrado> ListvDataProblemaEncontradoVM { get; set; }
        public DataProblemaEncontrado DataProblemaEncontradoVM { get; set; }
        public List<DataVerificacion> DataVerificacionVM { get; set; }
    }

}