using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SINU.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "¿Recordar este explorador?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage ="Debe ingregar un Correo!!!")]
        [Display(Name = "Correo electrónico")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="Debe igresar una Contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "¿Recordar cuenta?")]
        public bool RememberMe { get; set; }

        [Required]
        public string CaptchaInputText { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string Apellido { get; set; }

        [Required]
        [Display(Name = "Nombres")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "DNI")]
        [MaxLength(9, ErrorMessage = "DNI debe tener una longintud minima de 9 digitos"), MinLength(8, ErrorMessage = "DNI debe tener una longintud minima de 8 digitos")]
        [RegularExpression("^\\d+$", ErrorMessage = "DNI debe contener sólo números.")]
        public string DNI { get; set; }

        public DateTime FeachaToken { get; set; }

        //este parametro sera recibido de la vista index al momento de hacer clic en inscribirse no sera necesario mostra su valor en la vista Register.cshtml
        //sera requerido al momento de guardar su preferencia en el controlador AccountController.cs
        [Required]
        [Display(Name = "Modalidad a inscribirse")]
        public int IdInstituto { get; set; }
        //este parametro sera recibido de la vista index al momento de hacer clic en inscribirse
        [Required]
        [Display(Name = "Delegacion - Oficina")]
        public int idOficinaYDelegacion { get; set; }

        public SelectList ListOficinaYDelegacion { get; set; }

        public SelectList ListIntitutos { get; set; }
        public string DatosDelegacion { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

    }

    public class RecuperarCuenta
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico Original")]
        public string Email { get; set; }
    }

    public class RecuperacionCuenta
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico Original")]
        public string EmailOriginal { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Original")]
        public string PasswordOriginal { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = " Nuevo Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nueva Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
