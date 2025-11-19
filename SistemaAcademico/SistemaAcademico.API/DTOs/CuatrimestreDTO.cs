using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.API.DTOs
{
    public class CuatrimestreDTO
    {
        public int CuatrimestreId { get; set; } 
        public string Nombre { get; set; } = string.Empty; 
        public int Anio { get; set; }  
        public int Numero { get; set; } 
        public DateTime Fec_Inicio { get; set; } 
        public DateTime Fec_Fin { get; set; }
    }
}
