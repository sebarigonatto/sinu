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
        [DataType(DataType.PhoneNumber)]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El celular ingresado, tiene que tener al menos 10 digitos")]
        //la expresion regular tiene dos partes, que pueden o no estar separada por un "-", 
        //la primera parte acepta de 2 a 4 numeros  y la segunda de 6 a 8.
        [RegularExpression(@"^\(?([0-9]{2,4})\)?[-]?([0-9]{6,8})$", ErrorMessage = "El celular ingresado NO ES VALIDO")]
        public string Telefono { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10,MinimumLength =10,ErrorMessage ="El celular ingresado, tiene que tener al menos 10 digitos")]
        [RegularExpression(@"^\(?([0-9]{2,4})\)?[-]?([0-9]{6,8})$", ErrorMessage = "El celular ingresado NO ES VALIDO")]
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

    public  class vPersona_AntropometriaMetadata
    {
        public string Email { get; set; }
        public string Genero { get; set; }
        [Required]
        [Display(Name = "Altura(en cm)")]
        public Nullable<int> Altura { get; set; }
        [Required]
        [Display(Name = "Peso(en kg)")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        [RegularExpression("([0-9]{1,3})(,[0-9])?", ErrorMessage = "Debe cumplir esta forma 999,9")]
        public Nullable<decimal> Peso { get; set; }
        public Nullable<decimal> IMC { get; set; }
        [Required]
        public Nullable<int> PerimCabeza { get; set; }
        [Required]
        public Nullable<int> PerimTorax { get; set; }
        [Required]
        public Nullable<int> PerimCintura { get; set; }
        [Required]
        public Nullable<int> PerimCaderas { get; set; }
        [Required]
        public Nullable<int> LargoPantalon { get; set; }
        [Required]
        public Nullable<int> LargoEntrep { get; set; }
        public Nullable<int> LargoFalda { get; set; }
        [Required]
        public Nullable<int> Cuello { get; set; }
        [Required]
        public Nullable<int> Calzado { get; set; }
        [Required]
        public int IdPersona { get; set; }
    }
}