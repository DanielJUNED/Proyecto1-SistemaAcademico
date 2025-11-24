
namespace SistemaAcademico._Web.Models.ViewModels
{
        public class HistorialAcademicoIndexViewModel
        {
            public string TerminoBusqueda { get; set; }
            public List<EstudianteBusquedaViewModel> ResultadosBusqueda { get; set; } = new List<EstudianteBusquedaViewModel>();
        }

        public class EstudianteBusquedaViewModel
        {
            public int EstudianteId { get; set; }
            public string Identificacion { get; set; }
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public bool TieneHistorial { get; set; }
        }

        public class HistorialDetalleViewModel
        {
            public EstudianteInfoViewModel Estudiante { get; set; }
            public ResumenViewModel Resumen { get; set; }
            public List<CuatrimestreHistorialViewModel> HistorialPorCuatrimestre { get; set; }
            public string DatosGraficoJson { get; set; }
        }

        public class EstudianteInfoViewModel
        {
            public int EstudianteId { get; set; }
            public string Identificacion { get; set; }
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public DateTime Fec_Nacimiento { get; set; }
            public int Edad { get; set; }
            public string Ubicacion { get; set; }
        }

        public class ResumenViewModel
        {
            public int TotalCursos { get; set; }
            public int CursosAprobados { get; set; }
            public int CursosReprobados { get; set; }
            public int CursosEnProceso { get; set; }
            public decimal PromedioGeneral { get; set; }
            public decimal NotaMasAlta { get; set; }
            public decimal NotaMasBaja { get; set; }
            public int TotalCreditos { get; set; }
            public int CreditosAprobados { get; set; }
            public double PorcentajeAprobacion { get; set; }
        }

        public class CuatrimestreHistorialViewModel
        {
            public int CuatrimestreId { get; set; }
            public string NombreCuatrimestre { get; set; }
            public int Anio { get; set; }
            public int Numero { get; set; }
            public List<CursoDetalleEvaViewModel> Cursos { get; set; }
            public decimal PromedioDelCuatrimestre { get; set; }
            public int CursosAprobados { get; set; }
            public int CursosReprobados { get; set; }
        }

        public class CursoDetalleEvaViewModel
        {
            public int EvaluacionId { get; set; }
            public string CodigoCurso { get; set; }
            public string NombreCurso { get; set; }
            public int Creditos { get; set; }
            public decimal Nota { get; set; }
            public string Estado { get; set; }
            public string TipoParticipacion { get; set; }
            public string Observaciones { get; set; }
            public DateTime Fec_Evaluacion { get; set; }
            public string NombreDocente { get; set; }
        }
}
