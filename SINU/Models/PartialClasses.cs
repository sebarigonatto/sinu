using System.ComponentModel.DataAnnotations;
/*
 * EN LAS PARTIAL CLASSES SE AGREGAN CAMPOS QUE SE NECESITAN PERO QUE NO ESTÁN EN LA TABLA/VISTA/ENTIDAD ORIGINAL
 * ADEMÁS aQUÍ ASOCIO LA CLASE CON LA CLASE DE SU METADATA CORRESPONDIENTE
 * 
 * EJEMPLO: vUsuariosAdministrativos LE AGREGO 2 CAMPOS PARA SU CARGA Y GRABACIÓN
 * Y ASOCIO LA METADATA CORRESPONDIENTE DENOMINADA vUsuariosAdministrativosMetadata
 * */
namespace SINU.Models
{
    [MetadataType(typeof(vPersona_DatosBasicosMetadata))]
    public partial class vPersona_DatosBasicos
    {
    }

    [MetadataType(typeof(vPersona_DatosPerMetadata))]
    public partial class vPersona_DatosPer
    {
    }

    [MetadataType(typeof(vPersona_DomicilioMetadata))]
    public partial class vPersona_Domicilio
    {
    }

    
    [MetadataType(typeof(VPersona_EstudioMetadata))]
    public partial class VPersona_Estudio
    {
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
    //[MetadataType(typeof(GrupoCarrOficioMetadata))]
    //public partial class GrupoCarrOficio
    //{ }
    //[MetadataType(typeof(ModalidadMetadata))]
    //public partial class Modalidad
    //{ }
    [MetadataType(typeof(ModalidadMetadata))]
    public partial class Modalidad
    { }
    [MetadataType(typeof(ConvocatoriaMetadata))]
    public partial class Convocatoria
    { }
} 

