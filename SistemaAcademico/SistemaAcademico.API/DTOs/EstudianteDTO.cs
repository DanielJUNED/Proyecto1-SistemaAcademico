namespace SistemaAcademico.API.DTOs
{
    public class EstudianteDTO
    {
        public int EstudianteId { get; set; }
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public DateTime Fec_Nacimiento { get; set; }
        public int Edad { get; set; }
        public string Ubicacion { get; set; }
    }
    public class HistorialAcademicoCompletoDto
    {
        public EstudianteDTO Estudiante { get; set; }
        public ResumenAcademicoDto Resumen { get; set; }
        public List<HistorialPorCuatrimestreDto> HistorialPorCuatrimestre { get; set; }
        public List<GraficoNotasDto> DatosGraficoNotas { get; set; }
    }

    public class ResumenAcademicoDto
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

    public class HistorialPorCuatrimestreDto
    {
        public int CuatrimestreId { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int Anio { get; set; }
        public int Numero { get; set; }
        public List<CursoHistorialDto> Cursos { get; set; }
        public decimal PromedioDelCuatrimestre { get; set; }
        public int CursosAprobados { get; set; }
        public int CursosReprobados { get; set; }
    }

    public class CursoHistorialDto
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

    public class GraficoNotasDto
    {
        public string Etiqueta { get; set; }
        public decimal Promedio { get; set; }
        public int Aprobados { get; set; }
        public int Reprobados { get; set; }
    }

    public class BusquedaEstudianteDto
    {
        public string Termino { get; set; }
    }

    public class EstudianteBusquedaResultDto
    {
        public int EstudianteId { get; set; }
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public bool TieneHistorial { get; set; }
    }
}
