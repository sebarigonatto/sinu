using Microsoft.SqlServer.Server;
using SINU.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace SINU.ViewModels
{
    public class IDPersonaVM
    {
        public int ID_PER { get; set; }
        public List<int> EtapaTabs { get; set; }
        public string IDETAPA { get; set; }
        public string NomyApe { get; set; }
        public bool ProcesoInterrumpido { get; set; }
        public bool NoPostulado { get; set; }
        public List<Array> ListProblemaCantPantalla { get; set; }
        public bool DocuPenal { get; set; }
        public OficinasYDelegaciones OfiDele { get; set; }
        public bool anexo2 { get; set; }
        public bool certificado { get; set; }
        public List<vDataProblemaEncontrado> vDataProblemaEncontradosVmDocu { get; set; }
    }

    public class PersonaFamiliaVM
    {
        public int ID_PER { get; internal set; }
        public vPersona_Familiar vPersona_FamiliarVM { get; set; }
        public List<SelectListItem> SexoVM { get; set; }
        public List<SelectListItem> vParentecoVM { get; set; }
        public List<SelectListItem> vEstCivilVM { get; set; }
        public List<SelectListItem> ReligionVM { get; set; }
        public List<SelectListItem> TipoDeNacionalidadVm { get; set; }
        public bool postulante { get; set; }
        public string IDETAPA { get; set; }
    }

    public class DatosBasicosVM
    {
        public vPersona_DatosBasicos vPersona_DatosBasicosVM { get; set; }
        public List<Sexo> SexoVM { get; set; }
        public string Preferenacia { get; set; }
        public List<OficinasYDelegaciones> OficinasYDelegacionesVM { get; set; }
        public List<ComoSeEntero> ComoSeEnteroVM { get; set; }
        public List<vInstitucionesConvocadasYCarrerasAsociadas> GrupoCarrearOficio { get; set; }
    }

    public class DatosPersonalesVM
    {
        public vPersona_DatosPer_UltInscripc vPersona_DatosPerVM { get; set; }
        public List<vEstCivil> vEstCivilVM { get; set; }
        public List<vRELIGION> vRELIGIONVM { get; set; }
        public List<TipoNacionalidad> TipoNacionalidadVM { get; set; }
        public List<ComboModalidad> ModalidadVm { get; set; }
        public List<spCarrerasParaEsteInscripto_Result2> CarreraOficioVm { get; set; }
    }

    //para crear el combo de modalidad y poder aplicar la restriccion de esta civil que corresponda
    public class ComboModalidad
    {
        public string IdModalidad { get; set; }
        public string Modalidad { get; set; }
        public string EstCivil { get; set; }

    }

    public class DomicilioVM
    {
        public vPersona_Domicilio vPersona_DomicilioVM { get; set; }
        public List<sp_vPaises_Result> sp_vPaises_ResultVM { get; set; }
        public List<SelectListItem> provincias { get; set; }
        public List<vProvincia_Depto_Localidad> vProvincia_Depto_LocalidadREALVM { get; set; }
        public List<vProvincia_Depto_Localidad> vProvincia_Depto_LocalidadEVENTUALVM { get; set; }

        public List<SelectListItem> PostulanteViajeListaVM { get; set; }

    }

    public class EstudiosVM
    {
        public List<VPersona_Estudio> vPersona_EstudioListVM { get; set; }
        public VPersona_Estudio vPersona_Estudioidvm { get; set; }
        [DisplayName("Nivel")]
        public List<NiveldEstudio> NivelEstudioVM { get; set; }
        [DisplayName("Provincia/Juridiccion")]
        public List<SelectListItem> Provincia { get; set; }
        [DisplayName("Localidad")]
        public List<SelectListItem> Localidad { get; set; }
        [DisplayName("Instituto")]
        public List<SelectListItem> InstitutoVM { get; set; }

    }

    public class IdiomasVM
    {
        public vPersona_Idioma VPersona_IdiomaIdVM { get; set; }
        public List<NivelIdioma> NivelIdiomaVM { get; set; }
        public List<sp_vIdiomas_Result> Sp_VIdiomas_VM { get; set; }

    }

    public class ActividadMIlitarVM
    {
        public int IDPErsona { get; set; }
        public ActividadMilitar ACTMilitarIDVM { get; set; }
        public List<Fuerza> FuerzasVM { get; set; }
        public List<Baja> BajaVM { get; set; }
        public List<SituacionRevista> SituacionRevistaVM { get; set; }
    }
    public class SituacionOcupacionalVM
    {
        public vPersona_SituacionOcupacional VPersona_SituacionOcupacionalVM { get; set; }
        public SelectList EstadoDescripcionVM { get; set; }
        public List<SelectListItem> InteresesVM { get; set; }
        public List<string> IdInteres { get; set; }


    }

    public class Domiciolio_API
    {
        public vPersona_Domicilio vPersona_Domicilio_API { get; set; }
        public List<SelectListItem> Pais_API { get; set; }
        public List<SelectListItem> Provincia_API { get; set; }
        public List<SelectListItem> Localidad_API { get; set; }
        public List<SelectListItem> PostulanteViajeListaVM { get; set; }


    }

    public class Presentacion {
        public int IdPersona { get; set; }

        [DisplayName("Nombre")]
        public string Nombre { get; set; }
        [DisplayName("Apellido")]
        public string Apellido { get; set; }
        [DisplayName("Numero de Inscripción:")]
        public int ID_Inscripcion { get; set; }
        [DisplayName("Fecha de Presentación")]
        public DateTime FechaPresentacion { get; set; }
        [DisplayName("Lugar de Presentación")]
        public string DomicilioExamenNombre { get; set; }
        [DisplayName("Dirección")]
        public string DomicilioExamen { get; set; }
        public string Modalidad { get; set; }
        public string Carrera { get; set; }
        public byte[] Qr { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha_Inicio { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha_Fin { get; set; }
    }

    public class DocuPenalVM
    {
        public int IdPersona { get; set; }

        public int IdInscrip { get; set; }
        //[Required(ErrorMessage = "Falta Certificado Penal")]

        public HttpPostedFileBase ConstanciaAntcPenales { get; set; }
        //[Required(ErrorMessage = "Falta Anexo 2")]

        public HttpPostedFileBase FormularioAanexo2 { get; set; }
       
        public string PathFormularioAanexo2 { get; set; }
        public string PathConstanciaAntcPenales { get; set; }

        public DeclaracionJurada PenalDeclaJurada { get; set; }

    }


    public class DocuAnexoVM
    {

        public int IdPersona { get; set; }
        public List<DocumentosNecesariosDelInscripto_Result1> docus { get; set; }

    }

    public class ResultadoVM
    {
        public vInscripcionDetalle vIncripcion { get; set; }

        public bool result { get; set; }
    }

    public class PantallasError
    {
        public string IdPantalla { get; set; }
        public string Pantalla { get; set; }
        public int IdPant { get; set; }

    }

}