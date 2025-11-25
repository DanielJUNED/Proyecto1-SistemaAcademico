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
    //Rendimiento academico del estudiante
    public class EstudianteSimpleDTO
    {
        public int EstudianteId { get; set; }
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
    }

    // DTOs/NotaCursoDTO.cs
    public class NotaCursoDTO
    {
        public int CursoId { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public int CuatrimestreId { get; set; }
        public string NombreCuatrimestre { get; set; }
        public decimal Nota { get; set; }
        public string Estado { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public string TipoParticipacion { get; set; }
    }

    // DTOs/RendimientoCuatrimestreDTO.cs
    public class RendimientoCuatrimestreDTO
    {
        public int CuatrimestreId { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int Anio { get; set; }
        public int Numero { get; set; }
        public decimal PromedioNotas { get; set; }
        public int CursosAprobados { get; set; }
        public int CursosReprobados { get; set; }
        public int TotalCursos { get; set; }
    }

    // DTOs/EstadisticasDTO.cs
    public class EstadisticasDTO
    {
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

    // DTOs/RendimientoCompletoDTO.cs - DTO principal para la respuesta
    public class RendimientoCompletoDTO
    {
        public EstudianteDTO Estudiante { get; set; }
        public List<NotaCursoDTO> NotasPorCurso { get; set; }
        public List<RendimientoCuatrimestreDTO> NotasPorCuatrimestre { get; set; }
        public EstadisticasDTO EstadisticasGenerales { get; set; }
    }

    // DTOs/FiltrosRendimientoDTO.cs - DTO para recibir filtros
    public class FiltrosRendimientoDTO
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public List<int> CursosIds { get; set; }
        public List<int> CuatrimestresIds { get; set; }

        public FiltrosRendimientoDTO()
        {
            CursosIds = new List<int>();
            CuatrimestresIds = new List<int>();
        }
    }

    // DTOs/CuatrimestreDTO.cs - DTO simplificado para filtros
   /* public class CuatrimestreDTO
    {
        public int CuatrimestreId { get; set; }
        public string Nombre { get; set; }
        public int Anio { get; set; }
        public int Numero { get; set; }
    }*/
     
    // DTOs/ActualizacionDTO.cs - DTO para verificación de actualizaciones
    public class ActualizacionDTO
    {
        public bool HayActualizaciones { get; set; }
        public DateTime FechaConsulta { get; set; }
        public int NumeroNuevasEvaluaciones { get; set; }
    }
}
