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
        //el dni ingresado debe poseer 8 digitos para ser validado
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI ingresado no es valido")]
        //[RegularExpression(@"^([0-9]{2})([.])?([0-9]{3})([.])?([0-9]{3})$", ErrorMessage = "Caracteres ingresados NO validos")]
        //limito al dni que solo acepte numeros
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Caracteres ingresados NO validos")]
        public string DNI { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        //la expresion regular tiene dos partes, que pueden o no estar separada por un "-", 
        //la primera parte acepta de 2 a 4 numeros  y la segunda de 6 a 8.
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El telefono ingresado, tiene que tener al menos 10 digitos")]
        //[RegularExpression(@"^\(?([0-9]{2,4})\)?[-]?([0-9]{6,8})$", ErrorMessage = "El celular ingresado NO ES VALIDO")]
        [RegularExpression(@"^(\d){10}$", ErrorMessage = "El telefono ingresado NO ES VALIDO")]
        public string Telefono { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        //la expresion regular tiene dos partes, que pueden o no estar separada por un " - ", 
        //la primera parte acepta de 2 a 4 numeros  y la segunda de 6 a 8.
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El celular ingresado, tiene que tener al menos 10 digitos")]
        [RegularExpression(@"^(\d){10}$", ErrorMessage = "El celular ingresado NO ES VALIDO")]
        public string Celular { get; set; }
        [Required]
        //valido los correos ingresados
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$", ErrorMessage = "El correo electronico ingresado no es valido!!!")]
        //ver como modificar el mensaje de error al no ser validado el email
        //[DataType(DataType.EmailAddress,ErrorMessage ="El email ingresado no es valido")]
        public string Email { get; set; }
        [Display(Name ="Como se entero")]
        public string ComoSeEntero { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> EmpezoACargarDatos { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> PidioIngresoAlSist { get; set; }
        [Display(Name="Instituto a Inscribirse")]
        public Nullable<int> IdPreferencia { get; set; }
        [ScaffoldColumn(false)]
        public string NombreInst { get; set; }
        [ScaffoldColumn(false)]
        public string AspnetUser { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<int> IdSecuencia { get; set; }
        [ScaffoldColumn(false)]
        public string Etapa_Estado { get; set; }
        [Display(Name ="Sexo")]
        public int IdSexo { get; set; }
        [Display(Name="Oficinas y Delegaciones")]
        public Nullable<int> IdDelegacionOficinaIngresoInscribio { get; set; }
        [ScaffoldColumn(false)]
        public string Oficina { get; set; }
    }

    public class vPersona_DatosPerMetadata
    {
        public string Email { get; set; }
        public int IdPersona { get; set; }
        public string CUIL { get; set; }
        [Display(Name ="Fecha de Nacimiento")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)] 
        public Nullable<System.DateTime> FechaNacimiento { get; set; }
        //[Required]
        [Display(Name ="Edad")]
        public Nullable<int> edad { get; set; }
        [Display(Name="Estado Civil")]
        public string IdEstadoCivil { get; set; }
        public string EstadoCivil { get; set; }
        [Display(Name="Religion")]
        public string IdReligion { get; set; }
        public string Religion { get; set; }
        [Display(Name="Tipo de Nacionalidad")]
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
        [RegularExpression("([0-9]{1,3})(,[0-9])?", ErrorMessage = "Debe cumplir este formato 999,9")]
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

    public partial class vPersona_DomicilioMetadata
    {
        public string Email { get; set; }
        public int IdPersona { get; set; }
        public string Calle { get; set; }
        [RegularExpression("^[0-9]+$", ErrorMessage = "Solo se aceptan caracteres numericos")]
        public string Numero { get; set; }
        public string Piso { get; set; }
        public string Unidad { get; set; }
        public string Pais { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        [Display(Name ="Codigo Postal")]
        public string CODIGO_POSTAL { get; set; }
        public string Prov_Loc_CP { get; set; }
        [Display(Name = "Pais")]
        public string IdPais { get; set; }
        public Nullable<int> IdLocalidad { get; set; }
        [Display(Name = "Calle")]
        public string EventualCalle { get; set; }
        [Display(Name = "Numero")]
        [RegularExpression("^[0-9]+$",ErrorMessage ="Solo se aceptan caracteres numericos")]
        public string EventualNumero { get; set; }
        [Display(Name = "Piso")]
        public string EventualPiso { get; set; }
        [Display(Name = "Unidad")]
        public string EventualUnidad { get; set; }
        [Display(Name = "Pais")]
        public string EventualPais { get; set; }
        [Display(Name = "Provincia")]
        public string EventualProvincia { get; set; }
        [Display(Name = "Localidad")]
        public string EventualLocalidad { get; set; }
        [Display(Name = "Codigo Postal")]
        public string EventualCodigo_Postal { get; set; }
        public string EventualProv_Loc { get; set; }
        public Nullable<int> EventualIdLocalidad { get; set; }
        public string EventualIdPais { get; set; }
        public int IdDomicilioDNI { get; set; }
        public int IdDomicilioActual { get; set; }
    }

    public partial class VPersona_EstudioMetadata
    {
        public int IdPersona { get; set; }
        public Nullable<int> IdNiveldEstudio { get; set; }
        public int IdEstudio { get; set; }
        public string Titulo { get; set; }
        public bool Completo { get; set; }
        public int IdInstitutos { get; set; }
        public string Nombre { get; set; }
        public Nullable<double> Promedio { get; set; }
        [Display(Name ="Cantidad de Materias Adeudadas")]
        public Nullable<int> CantidadMateriaAdeudadas { get; set; }
        [Display(Name = "Ultimo año Cursado")]
        public Nullable<int> ultimoAnioCursado { get; set; }
        public string Nivel { get; set; }
        public string NombreYPaisInstituto { get; set; }
        public string Jurisdiccion { get; set; }
        public string Localidad { get; set; }
    }
    public partial class ActividadMilitarMetadadata
    {

        public Nullable<bool> Ingreso { get; set; }
        [Display(Name = "Fecha de Ingreso")]
        public Nullable<System.DateTime> FechaIngreso { get; set; }
        [Display(Name ="Fecha de Baja")]
        public Nullable<System.DateTime> FechaBaja { get; set; }
        public string CausaMotivoNoingreso { get; set; }
        [Display(Name = "Descripcion de la Baja")]
        public string MotivoBaja { get; set; }
        public string Jerarquia { get; set; }
        public string Cargo { get; set; }
        public string Destino { get; set; }
        [Display(Name = "Situacion de Revista")]
        public int IdSituacionRevista { get; set; }
        public int IdFuerza { get; set; }
        [Display(Name = "Motivo de Baja")]
        public int IdBaja { get; set; }
        public int IdActividadMilitar { get; set; }

        public virtual Fuerza Fuerza { get; set; }
    }

    public partial class vPersona_ActividadMilitarMetadata
    {
        public int IdPersona { get; set; }
        public int IdActividadMilitar { get; set; }
        public int IdFuerza { get; set; }
        public string Fuerza { get; set; }
        public string Jerarquia { get; set; }
        public string Cargo { get; set; }
        public string Destino { get; set; }
        public Nullable<System.DateTime> FechaIngreso { get; set; }
        public Nullable<System.DateTime> FechaBaja { get; set; }
        public Nullable<bool> Ingreso { get; set; }
    }

    public class vUsuariosAdministrativosMetadata
    {
        public string mr { get; set; }
        public string Grado { get; set; }
        public string Destino { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Comentario { get; set; }

        [ScaffoldColumn(false)]
        public System.DateTime FechUltimaAct { get; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }


        [Required]
        [Display(Name = "Perfil")]
        public string codGrupo { get; set; }

        public int IdOficinasYDelegaciones { get; set; }
    }

    public partial class PeriodosInscripcionesMetadata
    {
        [Required]
        [VPers_ControlRangoPeriodos_("IdInstitucion",ErrorMessage = "La fecha de Inicio ingresada esta dentro de otro periodo")]
        public System.DateTime FechaInicio { get; set; }
        [Required]
        [VPers_ControlRangoPeriodos_("IdInstitucion", ErrorMessage = "La fecha Final ingresada esta dentro de otro periodo")]
        [VPers_FIMenorFF_("FechaInicio", ErrorMessage = "Fecha Final debe ser superior a fecha de Inicio del rango.")]
        public System.DateTime FechaFinal { get; set; }
        [Required]
        public int IdInstitucion { get; set; }
        public int IdPeriodoInscripcion { get; set; }

    }
}