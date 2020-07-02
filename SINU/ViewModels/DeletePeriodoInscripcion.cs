using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{
    public class EliminarPeriodo
    {

        public List <vConvocatoriaDetalles> ConvocatorVm { get; set; }
        public PeriodosInscripciones PeriodosInscripcionesVm { get; set; }
    }
}