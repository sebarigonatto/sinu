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
    
    public partial class DocPresentado
    {
        public Nullable<int> IdTipoDocPresentado { get; set; }
        public string PathAlmacenamiento { get; set; }
        public Nullable<int> IdInscripcion { get; set; }
        public int IdDocPresentado { get; set; }
    
        public virtual Inscripcion Inscripcion { get; set; }
        public virtual TipoDocPresentado TipoDocPresentado { get; set; }
    }
}
