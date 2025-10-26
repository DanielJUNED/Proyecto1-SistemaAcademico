using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademico.Models.ViewModels
{
    // ViewModel para cursos matriculados
    public class CursoMatriculadoViewModel
    {
        public int EstudianteCursoID { get; set; }
        public int CursoID { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int CuatrimestreID { get; set; }
        public int DocenteId { get; set; }
        public string NombreDocente { get; set; }
        public int Creditos { get; set; }
        public bool TieneEvaluacion { get; set; }
        public int? EvaluacionId { get; set; }
        public string TipoParticipacion { get; set; }
        public string Observacion { get; set; }
        public decimal? NotaActual { get; set; }
        public string EstadoActual { get; set; }
    }
}