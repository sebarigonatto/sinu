﻿using SINU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SINU.ViewModels
{

    public class IndexConsultorModel : DataTableVM
    {
        public List<ConsultaProgramada> ConsultaProgramadaVm { get; set; }
        public List<SelectListItem> Etapas { get; set; }
        public SelectList Estados { get; set; }
    }
    
    public class ConsultaPorConvocatoria
    {
        public List<sp_Totales_FullRestriccion_Result> restriccionesConvocatoria { get; set; }
        public List<vInscripcionDetalleUltInsc> inscriptosConvocatoria { get; set; }
        public int idConvocatoria { get; set; }
        public vConvocatoriaDetalles infoConvocatoria { get; set; }
    }

}