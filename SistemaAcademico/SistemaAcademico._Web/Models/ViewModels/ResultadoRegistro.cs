using System.Collections.Generic;

namespace SistemaAcademico._Web.Models.ViewModels
{
    public class ResultadoRegistro
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public int? PersonaID { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
}