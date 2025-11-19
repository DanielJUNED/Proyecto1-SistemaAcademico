namespace SistemaAcademico.API.DTOs
{
    public class CursoCuatrimestreDTO
    {
        public int CursoCuatrimestreId { get; set; }
        public int CursoId { get; set; }
        public int CuatrimestreId { get; set; }
        public string Ind_Estado { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int TotalDocentes { get; set; }
        public int TotalEstudiantes { get; set; }
        public bool TieneEvaluaciones { get; set; }
        public List<DocenteAsignadoDTO> Docentes { get; set; }
    }
    public class CreateCursoCuatrimestreDTO
    {
        public int CursoId { get; set; }
        public int CuatrimestreId { get; set; }
        public List<int> DocenteIds { get; set; }
    }

    public class UpdateCursoCuatrimestreDTO
    {
        public int CursoCuatrimestreId { get; set; }
        public int CursoId { get; set; }
        public int CuatrimestreId { get; set; }
    }
}
