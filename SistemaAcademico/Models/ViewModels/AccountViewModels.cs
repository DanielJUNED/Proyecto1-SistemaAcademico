using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaAcademico.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nombre de usuario")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "¿Recordar cuenta?")]
        public bool RememberMe { get; set; }
    }
    public class RegisterViewModel
    {
        [Required] 
        [Display(Name = "Nombre de usuario")]
        public string UserName { get; set; }
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
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
    public class LoginResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
        public int? RemainingAttempts { get; set; }
        public bool IsLockedOut { get; set; }
    }
}