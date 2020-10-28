using System;
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
       
        //[Display(Name = "Fecha de finlaización de Proceso")]
        //public string ff { get; set; }
        public SelectList Instituciones { get; set; }
    }
}