using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SINU.Models
{
    public class vPersona_DatosBasicosMetadata
    {
        [ScaffoldColumn(false)]
        [Required]
        public int IdPersona { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public Nullable<int> IdPostulante { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Nombres { get; set; }
        [Required]
        public string Sexo { get; set; }
        [Required]
        public string DNI { get; set; }
        [Required]
        //[CelTel("Celular", ErrorMessage = "Not valid")]
        public string Telefono { get; set; }
        [Required]
        public string Celular { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string ComoSeEntero { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> EmpezoACargarDatos { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> PidioIngresoAlSist { get; set; }
        [Required]
        public Nullable<int> IdPreferencia { get; set; }
        [Required]
        public string NombreInst { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public string AspnetUser { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public Nullable<int> IdSecuencia { get; set; }
        [Required]
        [ScaffoldColumn(false)]
        public string Etapa_Estado { get; set; }
    }

    //[AttributeUsage(AttributeTargets.Class)]
    //public class CelTelAttribute : ValidationAttribute,ic
    //{
    //    private readonly string _celular;

    //    public CelTelAttribute(string Celular)
    //    {
    //        _celular = Celular;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {

    //        var currentValue = (DateTime)value;

    //        var Cel = validationContext.ObjectType.GetProperty(_celular).ToString();
    //        //var Cel1 = validationContext.ObjectInstance.
    //        if (Cel == null || Cel == "")
    //        {
    //            return new ValidationResult("Celular No Ingresado");
    //        }
    //        return ValidationResult.Success;
        //}
    //}
  

    //public class EnrollmentMetadata
    //{
    //    [Range(0, 4)]
    //    public Nullable<decimal> Grade;
    //}
}