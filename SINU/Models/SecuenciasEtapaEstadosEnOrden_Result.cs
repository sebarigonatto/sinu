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
    
    public partial class SecuenciasEtapaEstadosEnOrden_Result
    {
        public int Orden { get; set; }
        public Nullable<int> IdSecuencia { get; set; }
        public string Etapa { get; set; }
        public string Estado { get; set; }
        public Nullable<int> Anterior { get; set; }
        public Nullable<int> Siguiente { get; set; }
    }
}