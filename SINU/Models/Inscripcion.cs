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
    
    public partial class Inscripcion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Inscripcion()
        {
            this.DocPresentado = new HashSet<DocPresentado>();
            this.InscripcionEtapaEstado = new HashSet<InscripcionEtapaEstado>();
            this.DeclaracionJurada = new HashSet<DeclaracionJurada>();
        }
    
        public Nullable<System.DateTime> FechaInscripcion { get; set; }
        public string Numero { get; set; }
        public Nullable<int> IdPostulantePersona { get; set; }
        public Nullable<int> IdPreferencia { get; set; }
        public Nullable<int> IdEstablecimientoRindeExamen { get; set; }
        public Nullable<int> IdDelegacionOficinaIngresoInscribio { get; set; }
        public Nullable<System.DateTime> FechaEntrevista { get; set; }
        public Nullable<System.DateTime> FechaRindeExamen { get; set; }
        public int IdInscripcion { get; set; }
        public string IdModalidad { get; set; }
        public Nullable<int> IdCarreraOficio { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocPresentado> DocPresentado { get; set; }
        public virtual EstablecimientoRindeExamen EstablecimientoRindeExamen { get; set; }
        public virtual Institucion Institucion { get; set; }
        public virtual OficinasYDelegaciones OficinasYDelegaciones { get; set; }
        public virtual Postulante Postulante { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InscripcionEtapaEstado> InscripcionEtapaEstado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeclaracionJurada> DeclaracionJurada { get; set; }
    }
}
