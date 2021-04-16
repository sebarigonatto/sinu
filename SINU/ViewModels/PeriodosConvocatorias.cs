using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{
    public class PeriodosConvocatorias
    {
        //Periodos inscripcion
        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[VPers_ControlRangoPeriodos_("IdInstitucion", ErrorMessage = "La fecha de Inicio ingresada esta dentro de otro periodo")]
        [Display(Name = "Fecha de Inicio")]
        public System.DateTime FechaInicio { get; set; }
        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[VPers_ControlRangoPeriodos_("IdInstitucion", ErrorMessage = "La fecha Final ingresada esta dentro de otro periodo")]
        //[VPers_FIMenorFF_("FechaInicio", ErrorMessage = "Fecha Final debe ser superior a fecha de Inicio del rango.")]
        [Display(Name = "Fecha Final")]
        public System.DateTime FechaFinal { get; set; }
        [Required]
        [Display(Name = "Institución")]
        public int IdInstitucion { get; set; }
        [Display(Name = "Período de Inscripción")]
        public int IdPeriodoInscripcion { get; set; }
        //Convocatorias
        //[Required]
        //[Display(Name = "Período Inscripción")]
        //public int IdPeriodoInscripcion { get; set; }
        [Required]
        [Display(Name = "Modalidad")]
        public string IdModalidad { get; set; }
        [Required]
        [Display(Name = "Grupo de Carreras y Oficios")]
        public string IdGrupoCarrOficio { get; set; }
       
        [Display(Name = "Convocatoria")]
        public int IdConvocatoria { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Inicial del Proceso")]
        public System.DateTime Fecha_Inicio_Proceso { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Final del Proceso")]
        public System.DateTime Fecha_Fin_Proceso { get; set; }
     
        public SelectList Instituciones { get; set; }
    }
    public class FechaConvocatoria
    {
        [Display(Name ="Fecha Inicio Periodo de Inscripcion:")]
        [VPers_ControlRangoPeriodos_("IdInstitucion", "IdPeriodo", ErrorMessage = "La fecha de Inicio ingresada esta dentro de otro periodo")]
        public DateTime FechaInicioPeriodo { get; set; }
        [Display(Name = "Fecha Fin Periodo de Inscripcion:")]
        [VPers_ControlRangoPeriodos_("IdInstitucion", "IdPeriodo", ErrorMessage = "La fecha de Fin ingresada esta dentro de otro periodo")]
        [VPers_FIMenorFF_("FechaInicioPeriodo", ErrorMessage = "Fecha Final debe ser superior a fecha de Inicio del rango.")]
        public DateTime FechaFinPeriodo { get; set; }
        [Display(Name = "Fecha Fin Proceso de Inscripcion:")]
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