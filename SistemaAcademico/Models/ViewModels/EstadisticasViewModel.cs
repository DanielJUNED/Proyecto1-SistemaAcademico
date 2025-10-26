using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademico.Models.ViewModels
{
    // ViewModel principal de estadísticas
    public class EstadisticasViewModel
    {
        public int? CuatrimestreID { get; set; }
        public int? CursoID { get; set; }
        public string NombreCuatrimestre { get; set; }
        public string NombreCurso { get; set; }
        public string CodigoCurso { get; set; }

        // Indicadores principales
        public EstadisticasGeneralesViewModel Generales { get; set; }

        // Datos para gráficos
        public EstadisticasGraficosViewModel Graficos { get; set; }

        // Listado de estudiantes
        public List<EstudianteEstadisticaViewModel> Estudiantes { get; set; }
    }

    // Estadísticas generales
    public class EstadisticasGeneralesViewModel
    {
        // Totales
        public int TotalEstudiantes { get; set; }
        public int TotalEvaluaciones { get; set; }
        public int TotalMatriculados { get; set; }

        // Aprobación
        public int EstudiantesAprobados { get; set; }
        public int EstudiantesReprobados { get; set; }
        public int EstudiantesEnProceso { get; set; }

        // Porcentajes
        public decimal PorcentajeAprobacion { get; set; }
        public decimal PorcentajeReprobacion { get; set; }
        public decimal PorcentajeEnProceso { get; set; }
        public decimal PorcentajeParticipacion { get; set; }

        // Promedios
        public decimal PromedioGeneral { get; set; }
        public decimal PromedioAprobados { get; set; }
        public decimal PromedioReprobados { get; set; }

        // Participación
        public int ParticipacionExcelente { get; set; }
        public int ParticipacionBuena { get; set; }
        public int ParticipacionRegular { get; set; }
        public int ParticipacionBaja { get; set; }
        public int ParticipacionNinguna { get; set; }
    }

    // Datos para gráficos
    public class EstadisticasGraficosViewModel
    {
        // Gráfico de estados
        public List<string> EstadosLabels { get; set; }
        public List<int> EstadosData { get; set; }
        public List<string> EstadosColors { get; set; }

        // Gráfico de participación
        public List<string> ParticipacionLabels { get; set; }
        public List<int> ParticipacionData { get; set; }
        public List<string> ParticipacionColors { get; set; }

        // Gráfico de distribución de notas
        public List<string> NotasRangos { get; set; }
        public List<int> NotasDistribucion { get; set; }

        // Gráfico de tendencia
        public List<string> TendenciaLabels { get; set; }
        public List<decimal> TendenciaData { get; set; }
    }

    // Estudiante con estadísticas
    public class EstudianteEstadisticaViewModel
    {
        public int EstudianteID { get; set; }
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }

        public decimal? Nota { get; set; }
        public string Estado { get; set; }
        public string TipoParticipacion { get; set; }
        public string Observaciones { get; set; }
        public System.DateTime? FechaEvaluacion { get; set; }

        public bool TieneEvaluacion { get; set; }
    }

    // Filtros de búsqueda
    public class FiltroEstadisticasViewModel
    {
        public int? CuatrimestreID { get; set; }
        public int? CursoID { get; set; }
    }

    // Resultado de consulta de estadísticas
    public class EstadisticasResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public EstadisticasViewModel Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    // Opciones para dropdowns
    public class CuatrimestreOpcionViewModel
    {
        public int CuatrimestreID { get; set; }
        public string Nombre { get; set; }
        public int Anio { get; set; }
        public int Numero { get; set; }
        public string Ind_Estado { get; set; }
        public int TotalCursos { get; set; }
        public int TotalEstudiantes { get; set; }
    }

    public class CursoOpcionViewModel
    {
        public int CursoID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Creditos { get; set; }
        public int TotalEstudiantes { get; set; }
        public int TotalEvaluaciones { get; set; }
    }

    // Comparativa entre cursos
    public class ComparativaCursosViewModel
    {
        public List<CursoComparativoViewModel> Cursos { get; set; }
    }

    public class CursoComparativoViewModel
    {
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public int TotalEstudiantes { get; set; }
        public decimal PromedioNota { get; set; }
        public decimal PorcentajeAprobacion { get; set; }
        public int Aprobados { get; set; }
        public int Reprobados { get; set; }
    }
}