using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//cambio de preuba sinu 26/05/2020

namespace SINU.ViewModels
{
    public class vContacto
    {

        [Display(Name = "Lista De Configuracion")]
        public List<SINU.Models.Configuracion> Configuracion { get; set; }

        [Display(Name = "Lista De Oficinas")]
        public List<SINU.Models.OficinasYDelegaciones> listoficinas { get; set; }
    }
}
