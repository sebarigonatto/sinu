using System;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace SINU.Models
{
    public class vPersona_DatosBasicosMetadata
    {
        [ScaffoldColumn(false)]
        public int IdPersona { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<int> IdPostulante { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio")]
        [StringLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio")]
        [StringLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Nombres { get; set; }
        [ScaffoldColumn(false)]
        public string Sexo { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        //el dni ingresado debe poseer 8 digitos para ser validado
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI ingresado no es valido")]
        //[RegularExpression(@"^([0-9]{2})([.])?([0-9]{3})([.])?([0-9]{3})$", ErrorMessage = "Caracteres ingresados NO validos")]
        //limito al dni que solo acepte numeros
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Caracteres ingresados NO validos")]
        public string DNI { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaNacimiento { get; set; }

        [TelefonoCelular("Celular", ErrorMessage = "Este Campo debe esta completado")]
        [DataType(DataType.PhoneNumber)]
        //la expresion regular tiene dos partes, que pueden o no estar separada por un "-", 
        //la primera parte acepta de 2 a 4 numeros  y la segunda de 6 a 8.
        [StringLength(50,MinimumLength = 10, ErrorMessage = "El telefono ingresado, tiene que tener al menos 10 digitos")]
                //[RegularExpression(@"^\(?([0-9]{2,4})\)?[-]?([0-9]{6,8})$", ErrorMessage = "El celular ingresado NO ES VALIDO")]
        [RegularExpression(@"^(\d){10}$", ErrorMessage = "El telefono ingresado NO ES VALIDO")]
        public string Telefono { get; set; }

        [TelefonoCelular("Telefono", ErrorMessage = "este cammpo debe estra completado")]
        [DataType(DataType.PhoneNumber)]
        //la expresion regular tiene dos partes, que pueden o no estar separada por un " - ", 
        //la primera parte acepta de 2 a 4 numeros  y la segunda de 6 a 8.
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El celular ingresado, tiene que tener al menos 10 digitos")]
        [RegularExpression(@"^(\d){10}$", ErrorMessage = "El celular ingresado NO ES VALIDO")]
        public string Celular { get; set; }
        //verificar esto en el caso de ser familiar o postulante si debe ser requiro o no
        //o crear un a validacion perzonalizada donde se requiere en el caso de que se el postulante y en caso de ser un familiar no es requerido
        //[Required]
        //valido los correos ingresados
        [EmailAddress(ErrorMessage = "Correo electronico ingresado no valido.")]
        //ver como modificar el mensaje de error al no ser validado el email
        //[DataType(DataType.EmailAddress,ErrorMessage ="El email ingresado no es valido")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Campo Requerido")]
        [Display(Name = "Comentario")]
        [MaxLength(100, ErrorMessage = "Dato ingresado supera el limite del campo")]
        public string ComoSeEntero { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Como se entero")]
        public Nullable<int> IdComoSeEntero { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> EmpezoACargarDatos { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> PidioIngresoAlSist { get; set; }
        [Required]
        [Display(Name = "Preferencia elegida:")]
        public Nullable<int> IdPreferencia { get; set; }
        [ScaffoldColumn(false)]
        public string NombreInst { get; set; }
        [ScaffoldColumn(false)]
        public string AspnetUser { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<int> IdSecuencia { get; set; }
        [ScaffoldColumn(false)]
        public string Etapa_Estado { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Genero")]
        public int IdSexo { get; set; }
        [Required]
        [Display(Name = "Oficinas y Delegaciones")]
        public Nullable<int> IdDelegacionOficinaIngresoInscribio { get; set; }
        [ScaffoldColumn(false)]
        public string Oficina { get; set; }
        [Range(minimum: 16,maximum:35,ErrorMessage ="Edad no valida, verifique Fecha de Nacimiento")]//verificar esto
        public Nullable<int> Edad { get; set; }
    }

    public class vPersona_DatosPerMetadata
    {
        [Required(ErrorMessage = "Campo Obligatorio")]
        [StringLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio")]
        [StringLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Nombres { get; set; }
        public string Email { get; set; }
        public int IdPersona { get; set; }
        [Required(ErrorMessage ="Campo Obligatorio")]
        //[RegularExpression(@"\b(20|23|24|27|30|33|34)(\D)?[0-9]{8}(\D)?[0-9]", ErrorMessage = "Debe ingresar un Cuil valido.")]
        [RegularExpression(@"\b(20|23|24|27|30|33|34)[0-9]{8}[0-9]", ErrorMessage = "Debe ingresar un Cuil valido.")]
        [StringLength(11, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string CUIL { get; set; }
        [Display(Name = "Fecha de Nacimiento")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaNacimiento { get; set; }
        //[Required]
        [Display(Name = "Edad")]
        public Nullable<int> edad { get; set; }
        [Required]
        [Display(Name = "Estado Civil")]
        public string IdEstadoCivil { get; set; }
        [Display(Name = "Religión")]
        public string IdReligion { get; set; }
        [Display(Name = "Tipo de Nacionalidad")]
        [Required]
        public int idTipoNacionalidad { get; set; }
        public string Nacionalidad { get; set; }
        [Required]
        [Display(Name = "Modalidad")]
        public string IdModalidad { get; set; }
        [Required]
        [Display(Name = "Carrera")]
        public Nullable<int> IdCarreraOficio { get; set; }
        public Nullable<int> IdInscripcion { get; set; }
    }

    public class vPersona_AntropometriaMetadata
    {
        public string Email { get; set; }
        public string Genero { get; set; }
        [Required]
        [Display(Name = "Altura(en cm)")]
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0  
        [Range(100,300,ErrorMessage ="Altura ingresada no valida")]
        public Nullable<int> Altura { get; set; }
        [Required]
        [Display(Name = "Peso(en kg)")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        [RegularExpression("([0-9]{1,3})(,[0-9])?([0-9])?", ErrorMessage = "Debe cumplir este formato 999,9")]
        public Nullable<decimal> Peso { get; set; }
        public Nullable<decimal> IMC { get; set; }
        [Required]
        [Display(Name = "1° Perimetro de Cabeza (en cm)")]
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        public Nullable<int> PerimCabeza { get; set; }
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        [Display(Name = "2° Perimetro de Torax (en cm)")]
        [Required]
        public Nullable<int> PerimTorax { get; set; }
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        [Display(Name = "3° Perimetro de Cintura (en cm)")]
        [Required]
        public Nullable<int> PerimCintura { get; set; }
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        [Required]
        [Display(Name = "4° Largo de Pantalón (en cm)")]
        public Nullable<int> LargoPantalon { get; set; }
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        [Required]
        [Display(Name = "5° Largo de Entrepierna (en cm)")]
        public Nullable<int> LargoEntrep { get; set; }
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        [Required]  
        [Display(Name = "6° Perimetro de Cadera (en cm)")]
        public Nullable<int> PerimCaderas { get; set; }
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        [RequiredIf("Genero",true, "Mujer",ErrorMessage ="Debe Completar este campo")]
        [Display(Name = "7° Largo de Falda (en cm)")]
        public Nullable<int> LargoFalda { get; set; }
        [Required]
        [Display(Name = "Cuello (en cm)")]
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        public Nullable<int> Cuello { get; set; }
        [Required]
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage = "Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        public Nullable<int> Calzado { get; set; }
        [Required]
        public int IdPersona { get; set; }
    }

    public partial class vPersona_DomicilioMetadata
    {
        public string Email { get; set; }
        [Required]
        public int IdPersona { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Dato ingresado supera el limite del campo.")]
        public string Calle { get; set; }
        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Solo se aceptan caracteres numericos")]
        [MaxLength(10, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Numero { get; set; }
        //[Required]
        [MaxLength(3, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Piso { get; set; }
        //[Required]
        [MaxLength(5, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Unidad { get; set; }
        public string Pais { get; set; }
        [RequiredIf("IdPais",true,"AR",ErrorMessage ="Debe seleccionar un Provincia")]
        public string Provincia { get; set; }
        [RequiredIf("IdPais", false, "AR", ErrorMessage = "Debe Ingresar una Localidad")]
        [MaxLength(150, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Localidad { get; set; }
        [Required]
        [Display(Name = "Codigo Postal")]
        [MaxLength(20, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string CODIGO_POSTAL { get; set; }
        public string Prov_Loc_CP { get; set; }
        [Display(Name = "Pais")]
        public string IdPais { get; set; }
        [RequiredIf("IdPais", true, "AR", ErrorMessage = "Debe seleccionar una Localidad")]
        public Nullable<int> IdLocalidad { get; set; }
        [Required]
        [Display(Name = "Calle")]
        [MaxLength(100, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string EventualCalle { get; set; }
        [Required]
        [Display(Name = "Numero")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Solo se aceptan caracteres numericos")]
        [MaxLength(10, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string EventualNumero { get; set; }
        //[Required]
        [Display(Name = "Piso")]
        [MaxLength(3, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string EventualPiso { get; set; }
        //[Required]
        [Display(Name = "Unidad")]
        [MaxLength(5, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string EventualUnidad { get; set; }
        [Display(Name = "Pais")]
        public string EventualPais { get; set; }
        [RequiredIf("EventualIdPais", true, "AR", ErrorMessage = "Debe seleccionar una Provincia")]
        [Display(Name = "Provincia")]
        public string EventualProvincia { get; set; }
        [RequiredIf("EventualIdPais", false, "AR", ErrorMessage = "Debe ingresar una Localidad")]
        [Display(Name = "Localidad")]
        [MaxLength(150, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string EventualLocalidad { get; set; }
        [Required]
        [Display(Name = "Codigo Postal")]
        [MaxLength(20, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string EventualCodigo_Postal { get; set; }
        public string EventualProv_Loc { get; set; }
        [RequiredIf("EventualIdPais", true, "AR", ErrorMessage = "Debe seleccionar una Localidad")]
        [Display(Name ="Localidad")]
        public Nullable<int> EventualIdLocalidad { get; set; }
        [Required]
        public string EventualIdPais { get; set; }
        [Required]
        public int IdDomicilioDNI { get; set; }
        [Required]
        public int IdDomicilioActual { get; set; }
    }

    public partial class DeclaracionJuradaMetadata 
    {
        public int IdDeclaracionJurada { get; set; }
        [Required(ErrorMessage ="Debe seleccionar una opcion.")]
        public string PoseeAntecedentes { get; set; }
        [RequiredIf("PoseeAntecedentes", true, "SI", ErrorMessage = "Debe detallar los antecedentes que posee.")]
        public string Antecedentes_Detalles { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una opcion.")]
        public string EsAdicto { get; set; }
        public string Comentario { get; set; }
        public int IdInscripcion { get; set; }

    }

    public partial class VPersona_EstudioMetadata
    {
        [Required]
        public int IdPersona { get; set; }
        [Required]
        [Display(Name = "Nivel de Estudio")]
        public Nullable<int> IdNiveldEstudio { get; set; }
        public int IdEstudio { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Dato ingresado supera el limite del campo")]
        public string Titulo { get; set; }
        [Required]
        public bool Completo { get; set; }
        [Required]
        public int IdInstitutos { get; set; }
        [RequiredIf("Completo", true,ErrorMessage ="El campo promedio es requerido.")]
        [RegularExpression("^-?[0-9]+([,][0-9]*)?$", ErrorMessage = "Solo se aceptan numeros enteros o decimales")]
        public Nullable<double> Promedio { get; set; }
        //[Required]
        [RequiredIf("CursandoUltimoAnio", false, ErrorMessage = "El campo Cantidad de Materias Adeudadas es requerido.")]
        [Display(Name = "Cantidad de Materias Adeudadas:")]
        public Nullable<int> CantidadMateriaAdeudadas { get; set; }
        //[Required]
        [RequiredIf("CursandoUltimoAnio", false, ErrorMessage = "El campo ultimo Año Cursado es requerido.")]
        [Display(Name = "Ultimo año Cursado:")]
        [RegularExpression("^-?[0-9]*", ErrorMessage = "Solo se aceptan numeros enteros")]
        public Nullable<int> ultimoAnioCursado { get; set; }
        public string Nivel { get; set; }
        //[Required]
        public string NombreYPaisInstituto { get; set; }
        [Required]
        [Display(Name = "Jurisdicción")]
        [MaxLength(200, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Jurisdiccion { get; set; }
        //[Required]
        [RequiredIf("INST_EXT", true, ErrorMessage = "El campo Nombre de Instituto es requerido.")]
        [Display(Name = "Nombre del Instituto:")]
        [MaxLength(200, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Nombre { get; set; }
        [Required]
        public string Localidad { get; set; }
        [Required]
        [Display(Name ="¿Cursando el Ultimo año?")]
        public bool CursandoUltimoAnio { get; set; }
     
    }

    public partial class vPersona_IdiomaMetadadata
    {
        
        public string Email { get; set; }
        public Nullable<int> IdPersona { get; set; }
        [Required(ErrorMessage = "Debe seleccionar Nivel de Habla.")]
        [Display(Name = "Nivel de Habla")]
        public int Habla { get; set; }
        [Required(ErrorMessage = "Debe seleccionar Nivel de Lectura.")]
        [Display(Name = "Nivel de Lectura")]
        public int Lee { get; set; }
        [Required(ErrorMessage = "Debe seleccionar Nivel de Escritura.")]
        [Display(Name = "Nivel de Escritura")]
        public int Escribe { get; set; }
        public string Idioma { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una Idioma.")]
        [Display(Name = "Idioma")]
        public string CodIdioma { get; set; }
        [Display(Name = "Habla")]
        public string NivelHabla { get; set; }
        [Display(Name = "Escritura")]
        public string NivelEscribe { get; set; }
        [Display(Name = "Lectura")]
        public string NivelLee { get; set; }
        public int IdPersonaIdioma { get; set; }
    }

    public partial class ActividadMilitarMetadadata
    {
        [Required]
        public Nullable<bool> Ingreso { get; set; }
        [RequiredIf("Ingreso", true, ErrorMessage = "Seleccione una Fecha")]
        [Display(Name = "Fecha de Ingreso")]
        public Nullable<System.DateTime> FechaIngreso { get; set; }
        [Display(Name = "Causa/Motivo de no ingreso")]
        [RequiredIf("Ingreso", false, ErrorMessage = "Debe completar este campo")]
        [MaxLength(150, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string CausaMotivoNoingreso { get; set; }
        [RequiredIf("Ingreso", true, ErrorMessage = "Debe completar este campo")]
        [MaxLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        [Display(Name = "Jerarquía")]
        public string Jerarquia { get; set; }
        [RequiredIf("Ingreso", true, ErrorMessage = "Debe completar este campo")]
        [MaxLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Cargo { get; set; }
        [RequiredIf("Ingreso", true, ErrorMessage = "Debe completar este campo")]
        [MaxLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Destino { get; set; }
        [RequiredIf("Ingreso", true, ErrorMessage = "Selecciones una Opción")]
        [Display(Name = "Situación de Revista")]
        public int IdSituacionRevista { get; set; }
        [Required(ErrorMessage = "Debe Seleccionar un Fuerza")]
        public int IdFuerza { get; set; }
        [RequiredIf("Ingreso", true, ErrorMessage = "Seleccione una Opción")]
        [Display(Name = "Motivo de Baja")]
        public int IdBaja { get; set; } 
        [RequiredIf("Ingreso", true, ErrorMessage = "Seleccione una Fecha")]
        [Display(Name = "Fecha de Baja")]
        public Nullable<System.DateTime> FechaBaja { get; set; }
        [RequiredIf("Ingreso", true, ErrorMessage = "Debe completar este campo")]
        [Display(Name = "Descripción de la Baja")]
        [MaxLength(150, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string MotivoBaja { get; set; }
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

    public partial class vPersona_SituacionOcupacionalMetadata
    {   
        public int IdPersona { get; set; }
        [Display(Name = "Estado Ocupacional")]
        public int IdEstadoOcupacional { get; set; }
        public int IdSituacionOcupacional { get; set; }
        public string EstadoOcupacional { get; set; }
        public string Descripcion { get; set; }
        [RequiredIf("IdEstadoOcupacional", false, "1|4|9", ErrorMessage = "Debe completar este campo")]
        [Display(Name = "Ocupación Actual")]
        [MaxLength(60, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string OcupacionActual { get; set; }
        [RequiredIf("IdEstadoOcupacional", false, "1|4|9", ErrorMessage = "Debe completar este campo")]
        [Display(Name = "Domicilio Laboral")]
        [MaxLength(400, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string DomicilioLaboral { get; set; }
        [RequiredIf("IdEstadoOcupacional", false, "1|4|9", ErrorMessage = "Debe completar este campo")]
        [Display(Name = "Años Trabajados")]
        [RegularExpression(pattern: "^[1-9]\\d*$", ErrorMessage ="Dato no valido")]//ver aqui el tema de los años si permitir mas de 0
        public Nullable<int> AniosTrabajados { get; set; }
        [Required]
        [MaxLength(60, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Oficio { get; set; }
        public bool CargaOcupacionActual { get; set; }
        public string Explicacion { get; set; }
        public string DescInteres { get; set; }

    }

    public partial class vEntrevistaLugarFechaMetadata
    {
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        [Required(ErrorMessage ="Elemento Vacio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] public Nullable<System.DateTime> FechaEntrevista { get; set; }
        public int IdPersona { get; set; }
        public int IdInscripcion { get; set; }
        public int IdSecuencia { get; set; }
        public string Etapa { get; set; }
        public string Estado { get; set; }
        public string Email { get; set; }
    }

    public class vUsuariosAdministrativosMetadata
    {
        [Required]
        [Display(Name = "Matricula de Revista")]
        public string mr { get; set; }
        [Required]
        public string Grado { get; set; }
        [Required]
        public string Destino { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        public string Comentario { get; set; }

        [ScaffoldColumn(false)]
        public System.DateTime FechUltimaAct { get; }

        [Required]
        //[RegularExpression("([a-z]|[A-Z]|[0-9]|[\\W]){4}[a-zA-Z0-9\\W]{3,11}", ErrorMessage = "Invalid password format")]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$", ErrorMessage = "El correo electronico ingresado no es valido!!!")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Perfil")]
        public string codGrupo { get; set; }

        public int IdOficinasYDelegaciones { get; set; }
    }

    public partial class PeriodosInscripcionesMetadata
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Inicio")]
        public System.DateTime FechaInicio { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [VPers_FIMenorFF_("FechaInicio", ErrorMessage = "Fecha Final debe ser superior a fecha de Inicio del rango.")]
        [Display(Name = "Fecha Final")]
        public System.DateTime FechaFinal { get; set; }
        [Required]
        [Display(Name = "Institución")]
        public int IdInstitucion { get; set; }
        [Display(Name = "Período de Inscripción")]
        public int IdPeriodoInscripcion { get; set; }

    }

    public partial class vPersona_FamiliarMetadata
    {
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaNacimiento { get; set; }
        [RequiredIf("IdEstadoCivil",true, "CA", ErrorMessage ="Debe Seleccionar una Fecha")]
        [Display(Name = "Fecha de Casamiento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaCasamiento { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Parentesco")]
        public Nullable<int> idParentesco { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Genero")]
        public Nullable<int> IdSexo { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Estado Civil")]
        public string IdEstadoCivil { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [MaxLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [MaxLength(50, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        //el dni ingresado debe poseer 8 digitos para ser validado
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI ingresado no es valido")]
        //[RegularExpression(@"^([0-9]{2})([.])?([0-9]{3})([.])?([0-9]{3})$", ErrorMessage = "Caracteres ingresados NO validos")]
        [MaxLength(9, ErrorMessage = "DNI debe tener una longintud maxima de 9 digitos"), MinLength(8, ErrorMessage = "DNI debe tener una longintud minima de 8 digitos")]
        [RegularExpression("^\\d+$", ErrorMessage = "DNI debe contener sólo números.")]//limito al dni que solo acepte numeros
        public string DNI { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        public Nullable<bool> Vive { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Convive")]
        public Nullable<bool> ConVive { get; set; }
        [Display(Name = "Religión")]
        public string IdReligion { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Tipo Nacionalidad")]
        public Nullable<int> idTipoNacionalidad { get; set; }
        [Required(ErrorMessage = "Campo Requerido")]
        [RegularExpression(@"\b(20|23|24|27|30|33|34)[0-9]{8}[0-9]", ErrorMessage = "Debe ingresar un Cuil valido.")]
        [MaxLength(11, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string CUIL { get; set; }
    }
    public partial class vInscripcionEtapaEstadoUltimoEstadoMetadata
    {
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Fecha { get; set; }
    }

    public partial class SexoMetadata
    {
        [Required]
        public string Descripcion { get; set; }
    }
    public partial class FuerzaMetadata
    { 
        [Required]
        [Display(Name = "Fuerza")]
        public string Fuerza1 { get; set; }
    }

    public partial class TipoNacionalidadMetadata
    {
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string Tipo { get; set; }
    }
    public partial class vConvocatoriaDetallesMetadata
    {
        [Display(Name = "Inicio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaInicio { get; set; }
        [Display(Name = "Finalización")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaFinal { get; set; }
        [Display(Name = "Modalidad")]
        public string Modalidad { get; set; }
        [Display(Name = "Grupo")]
        public string Desc_Grupo { get; set; }
        [Display(Name = "Instituto")]
        public string NombreInst { get; set; }
        [Display(Name = "Código Modalidad")]
        public string IdModalidad { get; set; }
        [Display(Name = "Código Grupo")]
        public string IdGrupoCarrOficio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Fecha_Inicio_Proceso { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Fecha_Fin_Proceso { get; set; }
    }
    public partial class InstitucionMetadata
    {
        [Display(Name = "Instituto")]
        public string NombreInst { get; set; }
    }

    public partial class GrupoCarrOficioMetadata
    {
        [Display(Name = "Grupo")]
        public string IdGrupoCarrOficio { get; set; }
    }
    public partial class ResGrupoMetadata
    {
        
        [Display(Name = "Edad al Año Siguiente")]
        public int Edad_Al_AnioSig { get; set; }

        [Display(Name = "Edad al Día Siguiente")]
        public int Edad_Al_Dia { get; set; }
        [Display(Name = "Edad al Mes")]
        public int Edad_Al_Mes { get; set; }

        [Display(Name = "Edad Minima con Autorización")]
        public int EdadMinCAutoriz { get; set; }
        [Display(Name = "Edad Mínima")]
        public int EdadMin { get; set; }

        [Display(Name = "Edad Máxima")]
        public int EdadMax { get; set; }
        [Display(Name = "Estado Civil")]
        public string IdEstadoCivil { get; set; }

        [Display(Name = "Edad a la Fecha")]
        public int Edad_a_fecha { get; set; }
    }

    public partial class ModalidadMetadata
    {
        [Required]
        public string Personal { get; set; }
        [Required]
        public string Descripcion { get; set; }
    }
    public partial class ConvocatoriaMetadata
    {
        [Required]
        [Display(Name = "Período Inscripción")]
        public int IdPeriodoInscripcion { get; set; }
        [Required]
        [Display(Name = "Modalidad")]
        public string IdModalidad { get; set; }
        [Required]
        [Display(Name = "Grupo de Carreras y Oficios")]
        public int IdGrupoCarrOficio { get; set; }
     
        [Display(Name = "Convocatoria")]
        public int IdConvocatoria { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Inicial del Proceso")]
        public System.DateTime Fecha_Inicio_Proceso { get; set; }
      
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Final del Proceso")]
        public System.DateTime Fecha_Fin_Proceso { get; set; }
      
      
    }
    public partial class CarrerauOficioMetadata
    {
        [Required(ErrorMessage = "Campo Obligatorio.")]
        [Display(Name = "Carrera/Oficio")]
        public string CarreraUoficio { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio.")]
        [Display(Name = "Personal")]
        public string Personal { get; set; }
        
    }

    #region Ver mas a profundidad esta metadata debido a que el display name no funciona en las vistas
    public partial class DataVerificacionMetadata
    {
        [Display(Name = "Problemas Relacionados")]
        public int IdDataVerificacion { get; set; }
    }
    #endregion
    public partial class vDataProblemaEncontradoMetadata
    {
        [Display(Name = "Problema relacionado")]
        public string DataVerificacion { get; set; }
    }
    public partial class vInscripcionDetalleMetadata
    {
        [Display(Name = "Como se entero")]
        public string OpcionSeEnteroPOR { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Incripcion")]
        public System.DateTime FechaInscripcion { get; set; }
        [Display(Name ="N° de Preinscripcion")]
        public int IdInscripcion { get; set; }
        [Display(Name ="Se Inscribio en")]
        public string Inscripto_En { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Entrevista")]
        public System.DateTime FechaEntrevista { get; set; }
    }

    public partial class vInscriptosconTitulosProblemasMetadata
    {
        [Display(Name = "Enlace Postulante")]
        public int IdPostulantePersona { get; set; }
        [Display(Name = "Enlace Problema")]
        public int IdDataVerificacion { get; set; }
        [Display(Name = "Enlace Inscripto")]
        public int IdInscripcion { get; set; }
        [Display(Name = "Modalidad")]
        public string IdModalidad { get; set; }
        [Display(Name = "Enlace Carrera")]
        public Nullable<int> IdCarreraOficio { get; set; }
        [Display(Name = "Enlace Oficina")]
        public Nullable<int> IdDelegacionOficinaIngresoInscribio { get; set; }
        [Display(Name = "Desc. Probl.")]
        public string Descripcion { get; set; }
        [Display(Name = "Inscripción")]
        public Nullable<System.DateTime> FechaInscripcion { get; set; }
        [Display(Name = "Ofic.Deleg.")]
        public string Nombre { get; set; }
        [Display(Name = "Inicio")]
        public System.DateTime FechaInicio { get; set; }
        [Display(Name = "FIN")]
        public System.DateTime FechaFinal { get; set; }
        [Display(Name = "Enlace Convocat.")]
        public int IdConvocatoria { get; set; }
        [Display(Name = "Inicio Proceso")]
        public Nullable<System.DateTime> Fecha_Inicio_Proceso { get; set; }
        [Display(Name = "FIN Proceso")]
        public Nullable<System.DateTime> Fecha_Fin_Proceso { get; set; }
    }

    public partial class EstablecimientoRindeExamenMetadata
    {
        [Required]
        public string Jurisdiccion { get; set; }
        [Required]
        public string Localidad { get; set; }
        [Required]
        public string Departamento { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Comentario { get; set; }
        public int IdEstablecimientoRindeExamen { get; set; }
        [Required]
        public string Direccion { get; set; }
        public bool ACTIVO { get; set; }
    }
    public partial class vInscriptosCantYTODASConvocatoriasMetaData
    {
        public string IdModalidad { get; set; }
        public string IdGrupoCarrOficio { get; set; }
        public System.DateTime FechaInicio { get; set; }
        public System.DateTime FechaFinal { get; set; }
        public string NombreInst { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Fecha_Inicio_Proceso { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Fecha_Fin_Proceso { get; set; }
        public int IdConvocatoria { get; set; }
        public int IdPeriodoInscripcion { get; set; }
        public Nullable<int> CantInscriptos { get; set; }
    }
    public partial class OficinasyDelegacionesMetadata
    {
        [Display(Name ="Teléfono")]
        [Required(ErrorMessage ="Campo requerido")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo requerido")]

        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]

        public string Direccion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]

        public string Localidad { get; set; }
        [Required(ErrorMessage = "Campo requerido")]

        public string Provincia { get; set; }
        [Required(ErrorMessage = "Campo requerido")]

        public string CP { get; set; }

        public string Celular { get; set; }
        [Required(ErrorMessage = "Campo requerido")]

        public string Email1 { get; set; }


        public string Email2 { get; set; }
        public int IdOficinasYDelegaciones { get; set; }
    }
    public partial class vConsultaInscripcionesMetadata
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Fecha { get; set; }
        [Display(Name = "Modalidad")]
        public string Modalidad_Siglas { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //[DataType(DataType.Date)]
        //public Nullable<System.DateTime> Fecha_Inicio_Proceso { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        //[DataType(DataType.Date)]
        //public Nullable<System.DateTime> Fecha_Fin_Proceso { get; set; }
    }


}
