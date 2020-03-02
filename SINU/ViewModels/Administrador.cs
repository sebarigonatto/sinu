using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SINU.Models;

namespace SINU.ViewModels
{
    public class AdministradorVm
    {
        public List<Institucion> Institucions { get; set; }
        public PeriodosInscripciones PeriodosInscripcionesVm { get; set; }
    }
}