using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{
    public class AdministradorVm
    {

        public SelectList Instituciones { get; set; }
        public PeriodosInscripciones PeriodosInscripcionesVm { get; set; }
    }

    public class SetPasswordVM : SetPasswordViewModel
    {
        [Required]
        [Display(Name = "Mail Usuario:")] 
        public string Email { get; set; }
    }
    public class PostulantesAdminVM  : DataTableVM
    {

        public List<SelectListItem> Delegaciones { get; set; }
        public List<SelectListItem> Modalidad { get; set; }
       
    }

}