using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// borre los comentarios anteriores
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
