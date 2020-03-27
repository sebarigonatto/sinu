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
        string IdInst;
        public ValidacionesPersnalizadasAttribute(string idinst)
        {
            this.IdInst = idinst;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult valido = ValidationResult.Success;
            try
            {
                if (value != null)
                {
                    DateTime fechadada =(DateTime)value;
                    var idinstituto = validationContext.ObjectType.GetProperty(IdInst);
                    var valueid = int.Parse(idinstituto.GetValue(validationContext.ObjectInstance,null).ToString());
                    //ver como solucionar esto, si cuando se vence un ´periodo se lo elimina asi solo abria un solo registro con el cual comparar 
                    //o buscar el que tiene la fecha de finalizacion mas cercana y compararlo con aquel
                    var db = new SINUEntities();
                    var priodosinstitutos = db.PeriodosInscripciones.Where(m => m.IdInstitucion == valueid && (fechadada >= m.FechaInicio) && (fechadada <= m.FechaFinal)).ToList();//.OrderByDescending(m=>m.FechaFinal).ToList();
                    if (priodosinstitutos.Count>0 & value != null)
                    {
                        //agarro el primer periodo ya que es el que tien la fecha de finalizacion mas ultimo
                        //DateTime ultimoperiodo = priodosinstitutos[0].FechaFinal;
                        //DateTime fechainicioa =  (DateTime)value;
                        //if (fechainicioa < ultimoperiodo)
                        //{
                            valido = new ValidationResult(ErrorMessage);
                        //}
                    }
                }
               
            }
            catch (Exception ex)
            {

                valido = new ValidationResult(ErrorMessage + " " + ex.InnerException.Message);
            }
            return valido;
        }
    }
}