using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{
    public class AdministradorVm
    {

        public SelectList Institucions { get; set; }
        public PeriodosInscripciones PeriodosInscripcionesVm { get; set; }
    }

    public class SetPasswordVM : SetPasswordViewModel
    {
        [Required]
        [Display(Name = "Mail Usuario:")] 
        public string Email { get; set; }
    }

}