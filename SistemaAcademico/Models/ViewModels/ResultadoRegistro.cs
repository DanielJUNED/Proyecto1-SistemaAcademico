using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademico.Models.ViewModels
{
    public class ResultadoRegistro
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public int? EstudianteID { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
}