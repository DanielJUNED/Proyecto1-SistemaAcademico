using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Entities
{
    // Models/NotaCurso.cs - Entidad para notas con info del curso
    public class NotaCurso
    {
        public int EvaluacionId { get; set; }
        public int CursoId { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public int CuatrimestreId { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int AnioCuatrimestre { get; set; }
        public int NumeroCuatrimestre { get; set; }
        public decimal Nota { get; set; }
        public string Estado { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public string TipoParticipacion { get; set; }
    }

    // Models/RendimientoCuatrimestre.cs - Agregación por cuatrimestre
    public class RendimientoCuatrimestre
    {
        public int CuatrimestreId { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int Anio { get; set; }
        public int Numero { get; set; }
        public decimal PromedioNotas { get; set; }
        public int CursosAprobados { get; set; }
        public int CursosReprobados { get; set; }
        public int TotalCursos { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    // Models/EstadisticasEstudiante.cs - Estadísticas calculadas
    public class EstadisticasEstudiante
    {
        public int EstudianteId { get; set; }
        public decimal PromedioGeneral { get; set; }
        public int TotalCursosAprobados { get; set; }
        public int TotalCursosReprobados { get; set; }
        public int TotalCursosCursados { get; set; }
        public decimal PorcentajeAprobacion { get; set; }
        public decimal NotaMasAlta { get; set; }
        public decimal NotaMasBaja { get; set; }
        public string CursoMejorNota { get; set; }
        public string CursoPeorNota { get; set; }
    }

    // Models/FiltrosRendimiento.cs - Criterios de búsqueda
    public class FiltrosRendimiento
    {
        public int EstudianteId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public List<int> CursosIds { get; set; }
        public List<int> CuatrimestresIds { get; set; }
    }

    // Models/RendimientoEstudiante.cs - Agregación completa
    public class RendimientoEstudiante
    {
        public Estudiante Estudiante { get; set; }
        public List<NotaCurso> NotasCursos { get; set; }
        public List<RendimientoCuatrimestre> RendimientoPorCuatrimestre { get; set; }
        public EstadisticasEstudiante Estadisticas { get; set; }
    }
}
