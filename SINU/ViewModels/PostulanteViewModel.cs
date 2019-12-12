using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using SINU.Models;

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

    //public class DatosPersonalesVM
    //{
    //    public vPersona_DatosPer vPersona_DatosPerVM { get; set; }
    //    [DisplayName("Estado Civil")]
    //    public List<vEstCivil> vEstCivilVM { get; set; }
    //    [DisplayName("Religion")]
    //    public List<vRELIGION> vRELIGIONVM { get; set; }
    //    [DisplayName("Tipo de Nacionalidad")]
    //    public List<TipoNacionalidad> TipoNacionalidadVM { get; set; }
    //}

}