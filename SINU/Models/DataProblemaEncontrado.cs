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
    
    public partial class DataProblemaEncontrado
    {
        public int IdPostulantePersona { get; set; }
        public int IdDataVerificacion { get; set; }
        public string Comentario { get; set; }
        public int IdDataProblemaEncontrado { get; set; }
    
        public virtual DataVerificacion DataVerificacion { get; set; }
        public virtual Postulante Postulante { get; set; }
    }
}