
namespace SistemaAcademico.API.DTOs
{
    public class CursoDTO
    {
        public int CursoId { get; set; }
        public string Codigo { get; set; }
        public string Nom_Curso { get; set; }
        public string Desc_Curso { get; set; }
        public int Num_Creditos { get; set; }
        public BitacoraBaseDTO Bitacora { get; set; }
    }

    // DTO para crear (sin ID)
    public class CrearCursoDTO
    {
        public string Codigo { get; set; }
        public string Nom_Curso { get; set; }
        public string Desc_Curso { get; set; }
        public int Num_Creditos { get; set; }
        public BitacoraBaseDTO Bitacora { get; set; }
    }
    public class AsignarDocenteDTO
    {
        public int CursoId { get; set; }
        public int CuatrimestreId { get; set; }
        public int DocenteId { get; set; }
    }

}
