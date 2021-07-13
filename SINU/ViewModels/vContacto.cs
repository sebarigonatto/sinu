using SINU.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SINU.ViewModels
{
    public class vContacto
    {

        public OficinasYDelegaciones DPTOincorporacion { get; set; }

        [Display(Name = "Lista De Oficinas")]
        public List<OficinasYDelegaciones> listoficinas { get; set; }
    }
}