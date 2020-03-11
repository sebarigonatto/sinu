using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SINU.Models;

namespace SINU.ViewModels
{
    public class AdministradorVm
    {
        public List<Institucion> Institucions { get; set; }
        public PeriodosInscripciones PeriodosInscripcionesVm { get; set; }
    }

    //public class UsuarioVm
    //{
    //    [ScaffoldColumn(false)]
    //    public string codUsuario { get; set; }
    //    public string mr { get; set; }
    //    public string Grado { get; set; }
    //    public string Destino { get; set; }
    //    public string Nombre { get; set; }
    //    public string Apellido { get; set; }
    //    public string Comentario { get; set; }

    //    [ScaffoldColumn(false)]
    //    public System.DateTime FechUltimaAct { get; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Contraseña")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirmar contraseña")]
    //    [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
    //    public string ConfirmPassword { get; set; }

    //    public string Email { get; set; }


    //    [Required]
    //    [DisplayName("Perfil")]
    //    public string codGrupo { get; set; }


    //    public int IdOficinasYDelegaciones { get; set; }
    //    public virtual vSeguridad_Grupos Grupo { get; set; }
    //    public virtual OficinasYDelegaciones OficinasYDelegaciones { get; set; }
    //}

}