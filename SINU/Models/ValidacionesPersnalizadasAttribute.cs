using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SINU.Models
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =true)]
    public class VPers_ControlRangoPeriodos_Attribute : ValidationAttribute
    {
        string IdInst;
        public VPers_ControlRangoPeriodos_Attribute(string idinst)
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
                    var periodosinstitutos = db.PeriodosInscripciones.Where(m => m.IdInstitucion == valueid && (fechadada >= m.FechaInicio) && (fechadada <= m.FechaFinal)).ToList();//.OrderByDescending(m=>m.FechaFinal).ToList();
                
                    if (periodosinstitutos.Count>0)
                    {
                        //agarro el primer periodo ya que es el que tien la fecha de finalizacion mas ultimo
                        //DateTime ultimoperiodo = priodosinstitutos[0].FechaFinal;
                        //DateTime fechainicioa =  (DateTime)value;
                        valido = new ValidationResult(ErrorMessage + ": "+ periodosinstitutos[0].FechaInicio.ToShortDateString() + " - "+ periodosinstitutos[0].FechaFinal.ToShortDateString());
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


    //validacion especial para la comparacion de fechas de periodo de inscripcion

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VPers_FIMenorFF_Attribute  : ValidationAttribute
    {
        //Resumen de esta validacion :
        //     compara que la fecha final de un rango sea mayor que la fecha inicial del rango. 
        //     Acepta rangos de 1 dia esto quiere decir que si ff==fI es correcto
        //
        
        // Resumen de propiedad:
        //     Obtiene la propiedad que se va a comparar con la propiedad actual.
        //
        // Devuelve:
        //     La otra propiedad.
        public string OtherProperty { get; }
        //
        // Resumen de propiedad:
        //     Obtiene el nombre para mostrar de la otra propiedad.
        //
        // Devuelve:
        //     El nombre para mostrar de la otra propiedad.
        public string OtherPropertyDisplayName { get; }        //
        // Resumen:
        //     compra que la fecha final de un rango sea mayor que la fecha inicial del rango. 
        //     Acepta rangos de 1 dia esto quiere decir que si ff==fI es correcto
        //
        // Parámetros:
        //   otherProperty:
        //     Propiedad que se va a comparar con la propiedad actual.
        public VPers_FIMenorFF_Attribute(string otherProperty)
        { OtherProperty = otherProperty; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult valido = ValidationResult.Success;
            try
            {
                if (value != null)
                {
                    DateTime fechadelfinal = (DateTime)value;
                    var OtroCampo = validationContext.ObjectType.GetProperty(OtherProperty);
                    var idvalue = double.Parse(validationContext.ObjectType.GetProperty("IdInstitucion").GetValue(validationContext.ObjectInstance,null).ToString());
                    DateTime fechainicial = DateTime.Parse(OtroCampo.GetValue(validationContext.ObjectInstance, null).ToString());
                   
                    if (fechadelfinal <= fechainicial)
                    {
                        ErrorMessage = "Fecha Final debe ser superior a fecha de Inicio del rango.";
                        valido = new ValidationResult(ErrorMessage );
                    }

                    var periodos_inst = new SINUEntities().PeriodosInscripciones.Where(m => m.IdInstitucion == idvalue && (fechainicial < m.FechaInicio) && (m.FechaFinal < fechadelfinal)).ToList();
                    if (periodos_inst.Count > 0)
                    {
                        ErrorMessage = "El Periodo ingresado no puede contener otro Periodo dentro de si mismo.";
                        valido = new ValidationResult(ErrorMessage);
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