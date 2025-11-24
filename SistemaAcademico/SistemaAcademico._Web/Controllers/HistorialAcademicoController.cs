using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico._Web.Models.ViewModels;
using SistemaAcademico.API.DTOs;
using System.Text.Json;

namespace SistemaAcademico._Web.Controllers
{

    [Authorize]
    public class HistorialAcademicoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HistorialAcademicoController> _logger;

        public HistorialAcademicoController(IHttpClientFactory httpClientFactory, ILogger<HistorialAcademicoController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET: HistorialAcademico
        public IActionResult Index()
        {
            var model = new HistorialAcademicoIndexViewModel();
            return View(model);
        }

        // GET: HistorialAcademico/Buscar?termino=juan
        [HttpGet]
        public async Task<IActionResult> Buscar(string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
                {
                    return Json(new { success = false, message = "El término de búsqueda debe tener al menos 2 caracteres" });
                }

                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"HistorialAcademico/buscar?termino={Uri.EscapeDataString(termino)}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var estudiantes = JsonSerializer.Deserialize<List<EstudianteBusquedaResultDto>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var results = estudiantes?.Select(e => new EstudianteBusquedaViewModel
                    {
                        EstudianteId = e.EstudianteId,
                        Identificacion = e.Identificacion,
                        NombreCompleto = e.NombreCompleto,
                        Email = e.Email,
                        TieneHistorial = e.TieneHistorial
                    }).ToList() ?? new List<EstudianteBusquedaViewModel>();

                    return Json(new { success = true, data = results });
                }

                return Json(new { success = false, message = "Error al buscar estudiantes" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Buscar: {ex.Message}");
                return Json(new { success = false, message = "Error al realizar la búsqueda" });
            }
        }

        // GET: HistorialAcademico/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"HistorialAcademico/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["Error"] = "Estudiante no encontrado";
                    }
                    else
                    {
                        TempData["Error"] = "Error al obtener el historial académico";
                    }
                    return RedirectToAction(nameof(Index));
                }

                var content = await response.Content.ReadAsStringAsync();
                var historialDto = JsonSerializer.Deserialize<HistorialAcademicoCompletoDto>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (historialDto == null)
                {
                    TempData["Error"] = "No se pudo cargar la información del estudiante";
                    return RedirectToAction(nameof(Index));
                }

                // Mapear DTO a ViewModel
                var model = new HistorialDetalleViewModel
                {
                    Estudiante = new EstudianteInfoViewModel
                    {
                        EstudianteId = historialDto.Estudiante.EstudianteId,
                        Identificacion = historialDto.Estudiante.Identificacion,
                        NombreCompleto = historialDto.Estudiante.NombreCompleto,
                        Email = historialDto.Estudiante.Email,
                        Fec_Nacimiento = historialDto.Estudiante.Fec_Nacimiento,
                        Edad = historialDto.Estudiante.Edad,
                        Ubicacion = historialDto.Estudiante.Ubicacion
                    },
                    Resumen = new ResumenViewModel
                    {
                        TotalCursos = historialDto.Resumen.TotalCursos,
                        CursosAprobados = historialDto.Resumen.CursosAprobados,
                        CursosReprobados = historialDto.Resumen.CursosReprobados,
                        CursosEnProceso = historialDto.Resumen.CursosEnProceso,
                        PromedioGeneral = historialDto.Resumen.PromedioGeneral,
                        NotaMasAlta = historialDto.Resumen.NotaMasAlta,
                        NotaMasBaja = historialDto.Resumen.NotaMasBaja,
                        TotalCreditos = historialDto.Resumen.TotalCreditos,
                        CreditosAprobados = historialDto.Resumen.CreditosAprobados,
                        PorcentajeAprobacion = historialDto.Resumen.PorcentajeAprobacion
                    },
                    HistorialPorCuatrimestre = historialDto.HistorialPorCuatrimestre?.Select(c => new CuatrimestreHistorialViewModel
                    {
                        CuatrimestreId = c.CuatrimestreId,
                        NombreCuatrimestre = c.NombreCuatrimestre,
                        Anio = c.Anio,
                        Numero = c.Numero,
                        Cursos = c.Cursos?.Select(curso => new CursoDetalleEvaViewModel
                        {
                            EvaluacionId = curso.EvaluacionId,
                            CodigoCurso = curso.CodigoCurso,
                            NombreCurso = curso.NombreCurso,
                            Creditos = curso.Creditos,
                            Nota = curso.Nota,
                            Estado = curso.Estado,
                            TipoParticipacion = curso.TipoParticipacion,
                            Observaciones = curso.Observaciones,
                            Fec_Evaluacion = curso.Fec_Evaluacion,
                            NombreDocente = curso.NombreDocente
                        }).ToList() ?? new List<CursoDetalleEvaViewModel>(),
                        PromedioDelCuatrimestre = c.PromedioDelCuatrimestre,
                        CursosAprobados = c.CursosAprobados,
                        CursosReprobados = c.CursosReprobados
                    }).ToList() ?? new List<CuatrimestreHistorialViewModel>(),
                    DatosGraficoJson = JsonSerializer.Serialize(historialDto.DatosGraficoNotas)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Detalle: {ex.Message}");
                TempData["Error"] = "Error al cargar el historial académico";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
