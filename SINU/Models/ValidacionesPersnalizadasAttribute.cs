using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SINU.Models
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =true)]
    public class ValidacionesPersnalizadasAttribute : ValidationAttribute
    {
        public ValidacionesPersnalizadasAttribute()
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult valido = ValidationResult.Success;
            //var campomodelo = validationContext.MemberName;
            //var idinstituto = validationContext.ObjectInstance.
            //try
            //{
            //    var db = new SINUEntities();
            //    var priodosinstitutos = db.PeriodosInscripciones.Where(m=>m.IdInstitucion==)
            //    if (true)
            //    {
            //        validationContext.
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            return valido;
        }
    }
}