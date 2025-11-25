using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademico.Data.Entities
{
    [Table("Estudiante")]
    public partial class Estudiante
    { 
        public Estudiante()
        {
            EstudianteCurso = new HashSet<EstudianteCurso>();
        }
        [Key]
        [Display(Name = "Id Estudiante")]
        public int EstudianteId { get; set; }

        [Required(ErrorMessage = "La identificación es requerida")]
        [StringLength(20, ErrorMessage = "La identificación no puede exceder 20 caracteres")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime Fec_Nacimiento { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El distrito es requerido")]
        [Display(Name = "Distrito")]
        public int DistritoId { get; set; }

        [Required]
        [StringLength(2)]
        [Display(Name = "Estado")]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual Distrito Distrito { get; set; }
         
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } 

        public virtual ICollection<EstudianteCurso> EstudianteCurso { get; set; }
        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellidos}";


    }
}
