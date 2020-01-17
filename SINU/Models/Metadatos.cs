using System;
using System.ComponentModel.DataAnnotations;

namespace SINU.Models
{
    public class vPersona_DatosBasicosMetadata
    {
        [ScaffoldColumn(false)]
        public int IdPersona { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<int> IdPostulante { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Nombres { get; set; }
        [ScaffoldColumn(false)]
        public string Sexo { get; set; }
        [Required]
        public string DNI { get; set; }
        [Required]
        public string Telefono { get; set; }
        [Required]
        public string Celular { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string ComoSeEntero { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> EmpezoACargarDatos { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> PidioIngresoAlSist { get; set; }
        [Required]
        public Nullable<int> IdPreferencia { get; set; }
        [ScaffoldColumn(false)]
        public string NombreInst { get; set; }
        [ScaffoldColumn(false)]
        public string AspnetUser { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<int> IdSecuencia { get; set; }
        [ScaffoldColumn(false)]
        public string Etapa_Estado { get; set; }
        [Required]
        public int IdSexo { get; set; }
        [Required]
        public Nullable<int> IdDelegacionOficinaIngresoInscribio { get; set; }
        [ScaffoldColumn(false)]
        public string Oficina { get; set; }
    }

    public class vPersona_DatosPerMetadata
    {
        public string Email { get; set; }
        public int IdPersona { get; set; }
        public string CUIL { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> FechaNacimiento { get; set; }
        public Nullable<int> edad { get; set; }
        public string IdEstadoCivil { get; set; }
        public string EstadoCivil { get; set; }
        public string IdReligion { get; set; }
        public string Religion { get; set; }
        public int idTipoNacionalidad { get; set; }
        public string Nacionalidad { get; set; }
    }
}