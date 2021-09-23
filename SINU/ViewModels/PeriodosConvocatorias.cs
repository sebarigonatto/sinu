using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{
    public class FechaConvocatoria
    {
        [Display(Name ="Fecha Inicio Periodo de Inscripcion:")]
        [VPers_ControlRangoPeriodos_("IdInstitucion", "IdPeriodo", ErrorMessage = "La fecha de Inicio ingresada esta dentro de otro periodo")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaInicioPeriodo { get; set; }
        [Display(Name = "Fecha Fin Periodo de Inscripcion:")]
        [VPers_ControlRangoPeriodos_("IdInstitucion", "IdPeriodo", ErrorMessage = "La fecha de Fin ingresada esta dentro de otro periodo")]
        [VPers_FIMenorFF_("FechaInicioPeriodo", ErrorMessage = "Fecha Final debe ser superior a fecha de Inicio del rango.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaFinPeriodo { get; set; }
        [Display(Name = "Fecha Fin Proceso de Inscripcion:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaFinProceso { get; set; }
        [Display(Name = "Modalidad para la Convocatoria:")]

        public string IdModalidad { get; set; }
        public int IdInstitucion { get; set; }
        [Display(Name = "Grupo Carrera/Oficio:")]
        public string IdGrupoCarrOficio { get; set; }
        public int IDConvo { get; set; }
        public int IdPeriodo { get; set; }


    }

    public class CreacionConvocatoria
    {
        public FechaConvocatoria FechaConvo { get; set; }
        public SelectList GrupoCarrOficio { get; set; }
        public List<vInstitucionModalidad> Modalidades { get; set; }
    }

}