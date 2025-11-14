using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico._Web.Models.ViewModels
{
    // ViewModel para registrar evaluación
    public class RegistrarEvaluacionViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un estudiante y curso")]
        public int EstudianteCursoID { get; set; }

        [Required(ErrorMessage = "La nota es requerida")]
        [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100")]
        [Display(Name = "Nota")]
        public decimal Nota { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Required(ErrorMessage = "El tipo de participación es requerido")]
        [Display(Name = "Tipo de Participación")]
        public string TipoParticipacion { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        // Datos adicionales para mostrar
        public string NombreEstudiante { get; set; }
        public string IdentificacionEstudiante { get; set; }
        public string NombreCurso { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCuatrimestre { get; set; }
    }

    // ViewModel para actualizar evaluación
    public class ActualizarEvaluacionViewModel
    {
        [Required]
        public int EvaluacionID { get; set; }

        [Required(ErrorMessage = "La nota es requerida")]
        [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100")]
        public decimal Nota { get; set; }

        [StringLength(1000)]
        public string Observaciones { get; set; }

        [Required(ErrorMessage = "El tipo de participación es requerido")]
        public string TipoParticipacion { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; }
    }

    // Resultado de operación de evaluación
    public class EvaluacionResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? EvaluacionID { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public EvaluacionDetalleViewModel Evaluacion { get; set; }
    }

    // Detalle completo de evaluación
    public class EvaluacionDetalleViewModel
    {
        public int EvaluacionID { get; set; }
        public string NombreEstudiante { get; set; }
        public string IdentificacionEstudiante { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public string NombreCuatrimestre { get; set; }
        public decimal Nota { get; set; }
        public string Observaciones { get; set; }
        public string TipoParticipacion { get; set; }
        public string Estado { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public string NombreDocente { get; set; }
    }
}