//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SINU.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vInscriptosYRestriccionesCheck
    {
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int IdInscripcion { get; set; }
        public Nullable<int> IdPostulantePersona { get; set; }
        public string IdModalidad { get; set; }
        public Nullable<System.DateTime> FechaInscripcion { get; set; }
        public System.DateTime FechaInicio { get; set; }
        public System.DateTime FechaFinal { get; set; }
        public Nullable<System.DateTime> Fecha_Inicio_Proceso { get; set; }
        public Nullable<System.DateTime> Fecha_Fin_Proceso { get; set; }
        public int IdConvocatoria { get; set; }
        public string Genero { get; set; }
        public Nullable<int> Altura { get; set; }
        public Nullable<int> AlturaMinF { get; set; }
        public Nullable<int> AlturaMinM { get; set; }
        public string AlturaCheck { get; set; }
        public Nullable<decimal> IMC { get; set; }
        public Nullable<int> IMC_min { get; set; }
        public Nullable<int> IMC_max { get; set; }
        public string IMCCheck { get; set; }
        public Nullable<int> Edad { get; set; }
        public int EdadMinCAutoriz { get; set; }
        public int EdadMax { get; set; }
        public string EdadCheck { get; set; }
        public string IdEstadoCivil { get; set; }
        public string EstadoCivilId { get; set; }
        public string EstadoCiviCheck { get; set; }
        public string Oficina { get; set; }
        public Nullable<int> IdDelegacionOficinaIngresoInscribio { get; set; }
    }
}
