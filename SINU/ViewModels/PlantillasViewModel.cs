using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SINU.ViewModels
{
    public class PlantillaMailConfirmacion
    {
        public string Apellido { get; set; }

        public string LinkConfirmacion { get; set; }
    }
}