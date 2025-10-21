using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaAcademico.Models.ViewModels
{
    public class EstudianteViewModel
    {
        [Required(ErrorMessage = "La identificación es requerida")]
        [StringLength(20, ErrorMessage = "La identificación no puede exceder 20 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La identificación solo debe contener números")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo debe contener letras")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Los apellidos solo deben contener letras")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La provincia es requerida")]
        [Display(Name = "Provincia")]
        public int ProvinciaID { get; set; }

        [Required(ErrorMessage = "El cantón es requerido")]
        [Display(Name = "Cantón")]
        public int CantonID { get; set; }

        [Required(ErrorMessage = "El distrito es requerido")]
        [Display(Name = "Distrito")]
        public int DistritoID { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cuatrimestre")]
        [Display(Name = "Cuatrimestre")]
        public int CuatrimestreID { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos un curso")]
        [Display(Name = "Cursos Matriculados")]
        public List<int> CursosSeleccionados { get; set; } = new List<int>();
    }
}