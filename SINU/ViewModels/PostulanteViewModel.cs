using SINU.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace SINU.ViewModels
{
    public class IDPersonaVM
    {
        public int ID_PER { get; set; }
        public List<int> EtapaTabs { get; set; }
        public string IDETAPA { get; set; }
        public bool YAguardado { get; set; }

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
    }

    public class DatosBasicosVM
    {
        public vPersona_DatosBasicos vPersona_DatosBasicosVM { get; set; }
        public List<Sexo> SexoVM { get; set; }
        public List<vPeriodosInscrip> vPeriodosInscripsVM { get; set; }
        public List<OficinasYDelegaciones> OficinasYDelegacionesVM { get; set; }
        public List<ComoSeEntero> ComoSeEnteroVM { get; set; }
        
    }

    public class DatosPersonalesVM
    {
        public vPersona_DatosPer vPersona_DatosPerVM { get; set; }
        public List<vEstCivil> vEstCivilVM { get; set; }
        public List<vRELIGION> vRELIGIONVM { get; set; }
        public List<TipoNacionalidad> TipoNacionalidadVM { get; set; }
    }

    public class DomicilioVM
    {
        public vPersona_Domicilio vPersona_DomicilioVM { get; set; }
        public List<sp_vPaises_Result> sp_vPaises_ResultVM { get; set; }
        public List<string> provincias { get; set; }
        public List<vProvincia_Depto_Localidad> vProvincia_Depto_LocalidadREALVM { get; set; }
        public List<vProvincia_Depto_Localidad> vProvincia_Depto_LocalidadEVENTUALVM { get; set; }
    }

    public class EstudiosVM
    {
        public List<VPersona_Estudio> vPersona_EstudioListVM { get; set; }
        public VPersona_Estudio vPersona_EstudioIdVM { get; set; }
        [DisplayName("Nivel")]
        public List<NiveldEstudio> NivelEstudioVM { get; set; }
        [DisplayName("Provincia/Juridiccion")]
        public List<string> Provincia { get; set; }
        [DisplayName("Localidad")]
        public List<string> Localidad { get; set; }
        [DisplayName("Instituto")]
        public List<SelectListItem> InstitutoVM { get; set; }
        public bool INST_EXT { get; set; }

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
        public ActividadMilitar ACTMilitarIDVM { get; set;}
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

    }

}