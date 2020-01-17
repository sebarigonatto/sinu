using SINU.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace SINU.ViewModels
{

    public class DatosBasicosVM
    {
        public vPersona_DatosBasicos vPersona_DatosBasicosVM { get; set; }
        [DisplayName("Sexo")]
        public List<Sexo> SexoVM { get; set; }
        [DisplayName("Instituto a Inscribirse")]
        public List<vPeriodosInscrip> vPeriodosInscripsVM { get; set; }
        [DisplayName("Oficinas y Delegaciones")]
        public List<OficinasYDelegaciones> OficinasYDelegacionesVM { get; set; }
    }

    public class DatosPersonalesVM
    {
        public vPersona_DatosPer vPersona_DatosPerVM { get; set; }
        [DisplayName("Estado Civil")]
        public List<vEstCivil> vEstCivilVM { get; set; }
        [DisplayName("Religion")]
        public List<vRELIGION> vRELIGIONVM { get; set; }
        [DisplayName("Tipo de Nacionalidad")]
        public List<TipoNacionalidad> TipoNacionalidadVM { get; set; }
    }

    public class DomicilioVM
    {
        public vPersona_Domicilio vPersona_DomicilioVM { get; set; }
        [DisplayName("Pais")]
        public List<sp_vPaises_Result> sp_vPaises_ResultVM { get; set; }
        [DisplayName("Provincias")]
        public List<string> provincias { get; set; }
        public List<vProvincia_Depto_Localidad> vProvincia_Depto_LocalidadREALVM { get; set; }
        public List<vProvincia_Depto_Localidad> vProvincia_Depto_LocalidadEVENTUALVM { get; set; }
    }

    public class EstudiosVM
    {
        public IList<VPersona_Estudio> vPersona_EstudioListVM { get; set; }
        public VPersona_Estudio vPersona_EstudioIdVM { get; set; }
        [DisplayName("Nivel")]
        public List<NiveldEstudio> NivelEstudioVM { get; set; }
        [DisplayName("Provincia/Juridiccion")]
        public List<string> Provincia { get; set; }
        [DisplayName("Localidad")]
        public List<string> Localidad { get; set; }
        [DisplayName("Instituto")]
        public List<SelectListItem> InstitutoVM { get; set; }

    }

    public class IdiomasVM
    {
        public vPersona_Idioma VPersona_IdiomaIdVM { get; set; }
        public List<NivelIdioma> NivelIdiomaVM { get; set; }
        public List<sp_vIdiomas_Result> Sp_VIdiomas_VM { get; set; }

    }
}