using Microsoft.AspNetCore.Mvc;
using SistemaAcademico.API.DTOs;
using SistemaAcademico.Data.Repositories;

namespace SistemaAcademico.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialAcademicoController : ControllerBase
    {
        private readonly HistorialAcademicoDB _historialAcademicodb;

        public HistorialAcademicoController(HistorialAcademicoDB historialAcademico)
        {
            _historialAcademicodb = historialAcademico;
        }
        // GET: api/HistorialAcademico/buscar?termino=juan
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<EstudianteBusquedaResultDto>>> BuscarEstudiantes([FromQuery] string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
                {
                    return BadRequest(new { message = "El término de búsqueda debe tener al menos 2 caracteres" });
                }

                var estudiantes = await _historialAcademicodb.BuscarEstudiantesAsync(termino);
                var results = new List<EstudianteBusquedaResultDto>();

                foreach (var est in estudiantes)
                {
                    var tieneHistorial = await _historialAcademicodb.TieneHistorialAsync(est.EstudianteId);
                    results.Add(new EstudianteBusquedaResultDto
                    {
                        EstudianteId = est.EstudianteId,
                        Identificacion = est.Identificacion,
                        NombreCompleto = est.NombreCompleto,
                        Email = est.Email,
                        TieneHistorial = tieneHistorial
                    });
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al buscar estudiantes", error = ex.Message });
            }
        }
        // GET: api/HistorialAcademico/5
        [HttpGet("{estudianteId}")]
        public async Task<ActionResult<HistorialAcademicoCompletoDto>> GetHistorial(int estudianteId)
        {
            try
            {
                var estudiante = await _historialAcademicodb.GetEstudianteByIdAsync(estudianteId);
                if (estudiante == null)
                {
                    return NotFound(new { message = "Estudiante no encontrado" });
                }

                var historial = await _historialAcademicodb.GetHistorialCompletoAsync(estudianteId);
                var resumen = await _historialAcademicodb.GetResumenAcademicoAsync(estudianteId);

                // Agrupar por cuatrimestre
                var historialPorCuatrimestre = historial
                    .GroupBy(h => new { h.CuatrimestreId, h.NombreCuatrimestre, h.Anio, h.NumeroCuatrimestre })
                    .Select(g => new HistorialPorCuatrimestreDto
                    {
                        CuatrimestreId = g.Key.CuatrimestreId,
                        NombreCuatrimestre = g.Key.NombreCuatrimestre,
                        Anio = g.Key.Anio,
                        Numero = g.Key.NumeroCuatrimestre,
                        Cursos = g.Select(c => new CursoHistorialDto
                        {
                            EvaluacionId = c.EvaluacionId,
                            CodigoCurso = c.CodigoCurso,
                            NombreCurso = c.NombreCurso,
                            Creditos = c.Num_Creditos,
                            Nota = c.Nota,
                            Estado = c.Estado,
                            TipoParticipacion = c.TipoParticipacion,
                            Observaciones = c.Observaciones,
                            Fec_Evaluacion = c.Fec_Evaluacion,
                            NombreDocente = c.NombreDocente
                        }).ToList(),
                        PromedioDelCuatrimestre = g.Average(c => c.Nota),
                        CursosAprobados = g.Count(c => c.Estado == "Aprobado"),
                        CursosReprobados = g.Count(c => c.Estado == "Reprobado")
                    })
                    .OrderByDescending(g => g.Anio)
                    .ThenByDescending(g => g.Numero)
                    .ToList();

                // Datos para gráfico
                var datosGrafico = historialPorCuatrimestre.Select(h => new GraficoNotasDto
                {
                    Etiqueta = $"{h.NombreCuatrimestre}",
                    Promedio = h.PromedioDelCuatrimestre,
                    Aprobados = h.CursosAprobados,
                    Reprobados = h.CursosReprobados
                }).ToList();

                var edad = DateTime.Now.Year - estudiante.Fec_Nacimiento.Year;
                if (DateTime.Now < estudiante.Fec_Nacimiento.AddYears(edad)) edad--;

                var resultado = new HistorialAcademicoCompletoDto
                {
                    Estudiante = new EstudianteDTO
                    {
                        EstudianteId = estudiante.EstudianteId,
                        Identificacion = estudiante.Identificacion,
                        NombreCompleto = estudiante.NombreCompleto,
                        Email = estudiante.Email,
                        Fec_Nacimiento = estudiante.Fec_Nacimiento,
                        Edad = edad,
                        Ubicacion = $"{estudiante.Distrito}, {estudiante.Canton}, {estudiante.Provincia}"
                    },
                    Resumen = new ResumenAcademicoDto
                    {
                        TotalCursos = resumen.TotalCursos,
                        CursosAprobados = resumen.CursosAprobados,
                        CursosReprobados = resumen.CursosReprobados,
                        CursosEnProceso = resumen.CursosEnProceso,
                        PromedioGeneral = resumen.PromedioGeneral,
                        NotaMasAlta = resumen.NotaMasAlta,
                        NotaMasBaja = resumen.NotaMasBaja,
                        TotalCreditos = resumen.TotalCreditos,
                        CreditosAprobados = resumen.CreditosAprobados,
                        PorcentajeAprobacion = resumen.TotalCursos > 0
                            ? Math.Round((double)resumen.CursosAprobados / resumen.TotalCursos * 100, 2)
                            : 0
                    },
                    HistorialPorCuatrimestre = historialPorCuatrimestre,
                    DatosGraficoNotas = datosGrafico
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener historial", error = ex.Message });
            }
        }
    }
}
