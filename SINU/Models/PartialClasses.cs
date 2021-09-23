using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Documents;
/*
* EN LAS PARTIAL CLASSES SE AGREGAN CAMPOS QUE SE NECESITAN PERO QUE NO ESTÁN EN LA TABLA/VISTA/ENTIDAD ORIGINAL
* ADEMÁS aQUÍ ASOCIO LA CLASE CON LA CLASE DE SU METADATA CORRESPONDIENTE
* 
* EJEMPLO: vUsuariosAdministrativos LE AGREGO 2 CAMPOS PARA SU CARGA Y GRABACIÓN
* Y ASOCIO LA METADATA CORRESPONDIENTE DENOMINADA vUsuariosAdministrativosMetadata
* */
namespace SINU.Models

{
    public partial class OficinasYDelegaciones
    {
        public int Count { get; set; }
    }
    [MetadataType(typeof(vPersona_DatosBasicosMetadata))]
    public partial class vPersona_DatosBasicos
    {
        //[Required]
        //[Display(Name ="Carrera u Oficio")]
        //public string IdGrupoCarreraOficio { get; set; }
    }

    [MetadataType(typeof(vPersona_DatosPerMetadata))]
    public partial class vPersona_DatosPer_UltInscripc
    {
    }

    [MetadataType(typeof(vPersona_DomicilioMetadata))]
    public partial class vPersona_Domicilio
    {
        //public string IdpersonaPostu { get; set; }
        [RequiredIf("IdPais", false,"AR")]
        [MaxLength(150, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string TBoxProvincia { get; set; }
        [RequiredIf("EventualIdPais", false, "AR")]
        [MaxLength(150, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string TBoxEventualProvincia { get; set; }
    }

    
    [MetadataType(typeof(VPersona_EstudioMetadata))]
    public partial class VPersona_Estudio
    {
        [Required]
        [Display(Name = "Instituto en el Exterior?")]
        public bool INST_EXT { get; set; }
        [RequiredIf("IdInstitutos",true,"0",ErrorMessage ="Nombre de Instituto Requerido")]
        [MaxLength(200, ErrorMessage = "Dato ingresado supera el limite del campo.")]
        public string otro_inst { get; set; }
        [MaxLength(500, ErrorMessage = "Dato ingresado supera el limite del campo.")]

        public string prov_localidad { get; set; }

    }
    [MetadataType(typeof(vPersona_IdiomaMetadadata))]
    public partial class vPersona_Idioma
    {
    }
    

    [MetadataType(typeof(vPersona_AntropometriaMetadata))]
    public partial class vPersona_Antropometria
    {
    }
    
    [MetadataType(typeof(ActividadMilitarMetadadata))]
    public partial class ActividadMilitar
    {
    }
    
    [MetadataType(typeof(vPersona_ActividadMilitarMetadata))]
    public partial class vPersona_ActividadMilitar
    {
    }
    [MetadataType(typeof(vPersona_SituacionOcupacionalMetadata))]
    public partial class vPersona_SituacionOcupacional
    {
    }

  

    [MetadataType(typeof(vUsuariosAdministrativosMetadata))]
    public partial class vUsuariosAdministrativos
    {
        //public string Email { get; set; }
        //public string mr { get; set; }
        //public string Grado { get; set; }
        //public string Destino { get; set; }
        //public string Nombre { get; set; }
        //public string Apellido { get; set; }
        //public string Comentario { get; set; }

        //public System.DateTime FechUltimaAct { get; }

        //public string codGrupo { get; set; }

        //public int IdOficinasYDelegaciones { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }


    }

    [MetadataType(typeof(PeriodosInscripcionesMetadata))]
    public partial class PeriodosInscripciones
    {
    }
    [MetadataType(typeof(vPersona_FamiliarMetadata))]
    public partial class vPersona_Familiar
    { }
    [MetadataType(typeof(vInscripcionEtapaEstadoUltimoEstadoMetadata))]
    public partial class vInscripcionEtapaEstadoUltimoEstado
    {
    }
    [MetadataType(typeof(SexoMetadata))]
    public partial class Sexo
    { }
    [MetadataType(typeof(FuerzaMetadata))]
    public partial class Fuerza
    { }
    [MetadataType(typeof(TipoNacionalidadMetadata))]
    public partial class TipoNacionalidad
    { }
    [MetadataType(typeof(InstitucionMetadata))]
    public partial class Institucion
    { }
    [MetadataType(typeof(vConvocatoriaDetallesMetadata))]
    public partial class vConvocatoriaDetalles
    { }
    [MetadataType(typeof(GrupoCarrOficioMetadata))]
    public partial class GrupoCarrOficio
    { }
    [MetadataType(typeof(ResGrupoMetadata))]
    public partial class ResGrupo
    { }
    [MetadataType(typeof(ModalidadMetadata))]
    public partial class Modalidad
    { }
    [MetadataType(typeof(ConvocatoriaMetadata))]
    public partial class Convocatoria
    {
        
        internal static object ToList()
        {
            throw new NotImplementedException();
        }
    }
    
    [MetadataType(typeof(CarrerauOficioMetadata))]
    public partial class CarreraOficio
    { }
    [MetadataType(typeof(vEntrevistaLugarFechaMetadata))]
    public partial class vEntrevistaLugarFecha
    { }
    [MetadataType(typeof(DataVerificacionMetadata))]
    public partial class DataVerificacion
    { }
    [MetadataType(typeof(vDataProblemaEncontradoMetadata))]
    public partial class vDataProblemaEncontrado
    { }
    [MetadataType(typeof(vInscripcionDetalleMetadata))]
    public partial class vInscripcionDetalle
    { }
    [MetadataType(typeof(DeclaracionJuradaMetadata))]
    public partial class DeclaracionJurada
    {
        public int IdPersona { get; set; }
    }
    [MetadataType(typeof(vInscriptosconTitulosProblemasMetadata))]
    public partial class vInscriptosconTitulosProblemas
    {
        //[Required]
        //[Display(Name ="xxx")]
        //public string xxx { get; set; }
    }
    [MetadataType(typeof(EstablecimientoRindeExamenMetadata))]
    public partial class EstablecimientoRindeExamen
    { }
    [MetadataType(typeof(vInscriptosCantYTODASConvocatoriasMetaData))]
    public partial class vInscriptosCantYTODASConvocatorias
    { }
    [MetadataType(typeof(OficinasyDelegacionesMetadata))]
    public partial class OficinasYDelegaciones
    { }
    [MetadataType(typeof(vConsultaInscripcionesMetadata))]
    public partial class vConsultaInscripciones
    { }

}



