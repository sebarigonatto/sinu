﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//comentario para probar el stage changed


//lineas superiores agregadas junto con esta linea de comentario.. 
//para prueba de subida y bajada 01/06/2020 17:53

namespace SINU.ViewModels
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre Del Rol")]
        [Required(ErrorMessage = "{0} es obligatorio. ")]
        [StringLength(10, ErrorMessage = "La longitud de {0} debe ser menor a {1} y mayor a {2} ", MinimumLength = 1)]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "El campo {0} debe ser Alfabetico")]
        public String NombreRol { get; set; }

        public List<IdentityUser> Usuarios { get; set; }
    }
}