namespace SistemaAcademico._Web.Models.ViewModels
{
    public class CursoCuatrimestreIndexViewModel
    {
        public int? CuatrimestreIdFiltro { get; set; }
        public List<CuatrimestreViewModel> Cuatrimestres { get; set; }
        public List<CursoCuatrimestreListViewModel> CursosCuatrimestre { get; set; }
    }

    public class CursoCuatrimestreListViewModel
    {
        public int CursoCuatrimestreId { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int TotalDocentes { get; set; }
        public int TotalEstudiantes { get; set; }
        public bool PuedeEliminar { get; set; }
    }

    public class CursoCuatrimestreCreateViewModel
    {
        public int CuatrimestreId { get; set; }
        public CuatrimestreViewModel Cuatrimestre { get; set; }
        public List<CursoViewModel> Cursos { get; set; }
        public List<DocenteViewModel> DocentesDisponibles { get; set; }
        public List<DocenteSeleccionadoViewModel> DocentesSeleccionados { get; set; }
    }

    public class CursoCuatrimestreEditViewModel
    {
        public int CursoCuatrimestreId { get; set; }
        public int CursoId { get; set; }
        public int CuatrimestreId { get; set; }
        public CuatrimestreViewModel Cuatrimestre { get; set; }
        public CursoViewModel Curso { get; set; }
        public List<DocenteViewModel> DocentesDisponibles { get; set; }
        public List<DocenteAsignadoViewModel> DocentesAsignados { get; set; }
        public bool TieneEstudiantes { get; set; }
        public bool TieneEvaluaciones { get; set; }
        public bool PuedeEditarDocentes { get; set; }
    }

    public class CursoCuatrimestreDetailViewModel
    {
        public int CursoCuatrimestreId { get; set; }
        public CuatrimestreViewModel Cuatrimestre { get; set; }
        public CursoViewModel Curso { get; set; }
        public List<DocenteAsignadoViewModel> Docentes { get; set; }
        public int TotalEstudiantes { get; set; }
        public bool TieneEvaluaciones { get; set; }
    }

    public class CuatrimestreViewModel
    {
        public int CuatrimestreId { get; set; }
        public string Nombre { get; set; }
        public int Anio { get; set; }
        public int Numero { get; set; }
        public DateTime Fec_Inicio { get; set; }
        public DateTime Fec_Fin { get; set; }
    }

    public class CursoViewModel
    {
        public int CursoId { get; set; }
        public string Codigo { get; set; }
        public string Nom_Curso { get; set; }
        public string Desc_Curso { get; set; }
        public int Num_Creditos { get; set; }
    }

    public class DocenteViewModel
    {
        public int DocenteId { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
    }

    public class DocenteSeleccionadoViewModel
    {
        public int DocenteId { get; set; }
        public string NombreCompleto { get; set; }
    }

    public class DocenteAsignadoViewModel
    {
        public int CursoCuatriDocenteId { get; set; }
        public int DocenteId { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
    }
}
