using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico._Web.Models.ViewModels
{
    // ViewModel para listar
    public class CursoListViewModel
    {
        public int CursoId { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Nombre del Curso")]
        public string Nombre { get; set; }
        [Display(Name = "Descripción del Curso")]
        public string Descripcion { get; set; }

        [Display(Name = "Créditos")]
        public int Creditos { get; set; }
    }

    // ViewModel para crear/editar
    public class CursoFormViewModel
    {
        public int CursoId { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        [Display(Name = "Nombre del Curso")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Los créditos son requeridos")]
        [Range(1, 10, ErrorMessage = "Los créditos deben estar entre 1 y 10")]
        [Display(Name = "Número de Créditos")]
        public int Creditos { get; set; }
    }

    // ViewModel para detalle completo
    public class CursoDetalleViewModel
    {
        public int CursoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Creditos { get; set; }
        public int CantidadEstudiantes { get; set; }
        public List<string> DocentesAsignados { get; set; }
    }  
}
