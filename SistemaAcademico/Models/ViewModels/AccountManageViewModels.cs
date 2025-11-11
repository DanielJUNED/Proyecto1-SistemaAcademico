using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.Models.ViewModels
{
    // ViewModel para mostrar el perfil
    public class ManageProfileViewModel
    {
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Última Conexión")]
        public DateTime? UltimaConexion { get; set; }

        [Display(Name = "Rol")]
        public string Rol { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}