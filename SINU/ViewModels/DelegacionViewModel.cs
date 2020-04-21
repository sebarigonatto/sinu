using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public class DelegacionEntrevistaVm
    {

    }
}