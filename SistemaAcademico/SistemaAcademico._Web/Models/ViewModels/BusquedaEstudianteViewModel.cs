using System;
using System.Collections.Generic;

namespace SistemaAcademico._Web.Models.ViewModels
{
    public class BusquedaEstudianteViewModel
    {
        public int EstudianteID { get; set; }
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string DireccionCompleta { get; set; }
        public DateTime Fec_Nacimiento { get; set; }
        public int Edad { get; set; }
        public List<CursoMatriculadoViewModel> CursosMatriculados { get; set; }
    }
}