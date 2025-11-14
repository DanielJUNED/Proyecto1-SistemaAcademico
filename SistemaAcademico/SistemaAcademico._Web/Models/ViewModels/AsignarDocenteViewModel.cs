using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico._Web.Models.ViewModels
{
    public class AsignarDocenteViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un curso")]
        [Display(Name = "Curso")]
        public int CursoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cuatrimestre")]
        [Display(Name = "Cuatrimestre")]
        public int CuatrimestreId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un docente")]
        [Display(Name = "Docente")]
        public int DocenteId { get; set; }

        // Para los dropdowns
        public List<CursoSelectViewModel> Cursos { get; set; }
        public List<CuatrimestreSelectViewModel> Cuatrimestres { get; set; }
        public List<DocenteSelectViewModel> Docentes { get; set; }
    }

    public class CursoSelectViewModel
    {
        public int CursoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string DisplayText => $"{Codigo} - {Nombre}";
    }

    public class CuatrimestreSelectViewModel
    {
        public int CuatrimestreId { get; set; }
        public string Nombre { get; set; }
        public int Anio { get; set; }
        public string DisplayText => $"{Nombre} ({Anio})";
    }

    public class DocenteSelectViewModel
    {
        public int DocenteId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string DisplayText => $"{Nombre} {Apellidos}";
    }
}
