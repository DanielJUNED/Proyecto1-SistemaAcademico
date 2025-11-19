using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.API.DTOs
{
    public class DocenteDTO
    {
        public int DocenteId { get; set; } 
        public string Nombre { get; set; } = string.Empty; 
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
     
    }
    public class DocenteAsignadoDTO
    {
        public int CursoCuatriDocenteId { get; set; }
        public int DocenteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class AsignarDocenteGeneralDTO
    {
        public int CursoCuatrimestreId { get; set; }
        public int DocenteId { get; set; }
    }
}
