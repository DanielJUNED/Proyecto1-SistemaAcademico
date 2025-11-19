// Web/Controllers/CursoCuatrimestreController.cs
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico._Web.Controllers;
using SistemaAcademico._Web.Models.ViewModels;
using SistemaAcademico.API.DTOs;
using SistemaAcademico.Data.Repositories;
using System.Text;
using System.Text.Json;

namespace AcademicSystem.Web.Controllers
{
    public class CursoCuatrimestreController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CursoCuatrimestreController> _logger;

        public CursoCuatrimestreController(IHttpClientFactory httpClientFactory, ILogger<CursoCuatrimestreController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET: CursoCuatrimestre
        public async Task<IActionResult> Index(int? cuatrimestreId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Obtener cuatrimestres activos para el filtro
                var cuatrimestresResponse = await client.GetAsync("CursoCuatrimestre/cuatrimestres");
                List<CuatrimestreViewModel> cuatrimestres = new List<CuatrimestreViewModel>();

                if (cuatrimestresResponse.IsSuccessStatusCode)
                {
                    var cuatrimestresContent = await cuatrimestresResponse.Content.ReadAsStringAsync();
                    var cuatrimestresData = JsonSerializer.Deserialize<List<CuatrimestreDTO>>(cuatrimestresContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Mapear a ViewModel
                    cuatrimestres = cuatrimestresData?.Select(c => new CuatrimestreViewModel
                    {
                        CuatrimestreId = c.CuatrimestreId,
                        Nombre = c.Nombre,
                        Anio = c.Anio,
                        Numero = c.Numero,
                        Fec_Inicio = c.Fec_Inicio,
                        Fec_Fin =c.Fec_Fin
                    }).ToList() ?? new List<CuatrimestreViewModel>();
                }

                // Obtener cursos-cuatrimestre
                var url = cuatrimestreId.HasValue
                    ? $"CursoCuatrimestre?cuatrimestreId={cuatrimestreId.Value}"
                    : "CursoCuatrimestre";

                var response = await client.GetAsync(url);

                List<CursoCuatrimestreListViewModel> cursos = new List<CursoCuatrimestreListViewModel>();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var cursosDTO = JsonSerializer.Deserialize<List<CursoCuatrimestreDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    cursos = cursosDTO?.Select(dto => new CursoCuatrimestreListViewModel
                    {
                        CursoCuatrimestreId = dto.CursoCuatrimestreId,
                        CodigoCurso = dto.CodigoCurso,
                        NombreCurso = dto.NombreCurso,
                        NombreCuatrimestre = dto.NombreCuatrimestre,
                        TotalDocentes = dto.TotalDocentes,
                        TotalEstudiantes = dto.TotalEstudiantes,
                        PuedeEliminar = !dto.TieneEvaluaciones && dto.TotalEstudiantes == 0 //&& dto.TotalDocentes == 0
                    }).ToList() ?? new List<CursoCuatrimestreListViewModel>();
                }

                var model = new CursoCuatrimestreIndexViewModel
                {
                    CuatrimestreIdFiltro = cuatrimestreId,
                    Cuatrimestres = cuatrimestres,
                    CursosCuatrimestre = cursos
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Index: {ex.Message}");
                ViewBag.Error = $"Error al cargar los datos: {ex.Message}";
                return View(new CursoCuatrimestreIndexViewModel
                {
                    Cuatrimestres = new List<CuatrimestreViewModel>(),
                    CursosCuatrimestre = new List<CursoCuatrimestreListViewModel>()
                });
            }
        }

        // GET: CursoCuatrimestre/Create
        public async Task<IActionResult> Crear(int? cuatrimestreId)
        {
            if (!cuatrimestreId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar un cuatrimestre";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Obtener cuatrimestre
                var cuatrimestreResponse = await client.GetAsync($"CursoCuatrimestre/cuatrimestres/{cuatrimestreId.Value}");
                if (!cuatrimestreResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Cuatrimestre no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var cuatrimestreContent = await cuatrimestreResponse.Content.ReadAsStringAsync();
                var cuatrimestreData = JsonSerializer.Deserialize<CuatrimestreDTO>(cuatrimestreContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var cuatrimestre = new CuatrimestreViewModel
                {
                    CuatrimestreId = cuatrimestreData.CuatrimestreId,
                    Nombre = cuatrimestreData.Nombre,
                    Anio = cuatrimestreData.Anio,
                    Numero = cuatrimestreData.Numero,
                    Fec_Inicio = cuatrimestreData.Fec_Inicio,
                    Fec_Fin = cuatrimestreData.Fec_Fin
                };

                // Obtener cursos
                var cursosResponse = await client.GetAsync($"CursoCuatrimestre/CursoNoEnCuatrimestre/{cuatrimestreId.Value}");
                List<CursoViewModel> cursos = new List<CursoViewModel>();

                if (cursosResponse.IsSuccessStatusCode)
                {
                    var cursosContent = await cursosResponse.Content.ReadAsStringAsync();
                    var cursosData = JsonSerializer.Deserialize<List<CursoDTO>>(cursosContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    cursos = cursosData?.Select(c => new CursoViewModel
                    {
                        CursoId = c.CursoId,
                        Codigo = c.Codigo,
                        Nom_Curso = c.Nom_Curso,
                        Desc_Curso = c.Desc_Curso,
                        Num_Creditos = c.Num_Creditos
                    }).ToList() ?? new List<CursoViewModel>();
                }

                // Obtener docentes
                var docentesResponse = await client.GetAsync("CursoCuatrimestre/docentes");
                List<DocenteViewModel> docentes = new List<DocenteViewModel>();

                if (docentesResponse.IsSuccessStatusCode)
                {
                    var docentesContent = await docentesResponse.Content.ReadAsStringAsync();
                    var docentesData = JsonSerializer.Deserialize<List<DocenteDTO>>(docentesContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    docentes = docentesData?.Select(d => new DocenteViewModel
                    {
                        DocenteId = d.DocenteId,
                        NombreCompleto = $"{d.Nombre} {d.Apellidos}",
                        Email = d.Email
                    }).ToList() ?? new List<DocenteViewModel>();
                }

                var model = new CursoCuatrimestreCreateViewModel
                {
                    CuatrimestreId = cuatrimestreId.Value,
                    Cuatrimestre = cuatrimestre,
                    Cursos = cursos,
                    DocentesDisponibles = docentes,
                    DocentesSeleccionados = new List<DocenteSeleccionadoViewModel>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Create GET: {ex.Message}");
                TempData["Error"] = "Error al cargar los datos";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CursoCuatrimestre/Edit/5
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Obtener el curso-cuatrimestre
                var response = await client.GetAsync($"CursoCuatrimestre/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Curso-Cuatrimestre no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var content = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<CursoCuatrimestreDTO>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Validar permisos de edición
                var validacionResponse = await client.GetAsync($"CursoCuatrimestre/ValidarEdicion/{id}");
                bool puedeEditar = true;
                bool tieneEstudiantes = false;
                bool tieneEvaluaciones = false;

                if (validacionResponse.IsSuccessStatusCode)
                {
                    var validacionContent = await validacionResponse.Content.ReadAsStringAsync();
                    var validacion = JsonSerializer.Deserialize<JsonElement>(validacionContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    puedeEditar = validacion.GetProperty("puedeEditar").GetBoolean();
                    tieneEstudiantes = validacion.GetProperty("tieneEstudiantes").GetBoolean();
                    tieneEvaluaciones = validacion.GetProperty("tieneEvaluaciones").GetBoolean();
                }

                // Obtener cuatrimestre
                var cuatrimestreResponse = await client.GetAsync($"CursoCuatrimestre/cuatrimestres/{dto.CuatrimestreId}");
                var cuatrimestreContent = await cuatrimestreResponse.Content.ReadAsStringAsync();
                var cuatrimestreData = JsonSerializer.Deserialize<CuatrimestreDTO>(cuatrimestreContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var cuatrimestre = new CuatrimestreViewModel
                {
                    CuatrimestreId = cuatrimestreData.CuatrimestreId,
                    Nombre = cuatrimestreData.Nombre,
                    Anio = cuatrimestreData.Anio,
                    Numero = cuatrimestreData.Numero,
                    Fec_Inicio = cuatrimestreData.Fec_Inicio,
                    Fec_Fin = cuatrimestreData.Fec_Fin
                };

                // Obtener curso
                var cursoResponse = await client.GetAsync($"CursoCuatrimestre/cursos/{dto.CursoId}");
                var cursoContent = await cursoResponse.Content.ReadAsStringAsync();
                var cursoData = JsonSerializer.Deserialize<CursoDTO>(cursoContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var curso = new CursoViewModel
                {
                    CursoId = cursoData.CursoId,
                    Codigo = cursoData.Codigo,
                    Nom_Curso = cursoData.Nom_Curso,
                    Desc_Curso = cursoData.Desc_Curso,
                    Num_Creditos = cursoData.Num_Creditos
                };

                // Obtener todos los docentes disponibles
                var docentesResponse = await client.GetAsync("CursoCuatrimestre/docentes");
                List<DocenteViewModel> todosDocentes = new List<DocenteViewModel>();

                if (docentesResponse.IsSuccessStatusCode)
                {
                    var docentesContent = await docentesResponse.Content.ReadAsStringAsync();
                    var docentesData = JsonSerializer.Deserialize<List<DocenteDTO>>(docentesContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    todosDocentes = docentesData?.Select(d => new DocenteViewModel
                    {
                        DocenteId = d.DocenteId,
                        NombreCompleto = $"{d.Nombre} {d.Apellidos}",
                        Email = d.Email
                    }).ToList() ?? new List<DocenteViewModel>();
                }

                // Mapear docentes asignados
                var docentesAsignados = dto.Docentes?.Select(d => new DocenteAsignadoViewModel
                {
                    CursoCuatriDocenteId = d.CursoCuatriDocenteId,
                    DocenteId = d.DocenteId,
                    NombreCompleto = d.NombreCompleto,
                    Email = d.Email
                }).ToList() ?? new List<DocenteAsignadoViewModel>();

                var model = new CursoCuatrimestreEditViewModel
                {
                    CursoCuatrimestreId = dto.CursoCuatrimestreId,
                    CursoId = dto.CursoId,
                    CuatrimestreId = dto.CuatrimestreId,
                    Cuatrimestre = cuatrimestre,
                    Curso = curso,
                    DocentesDisponibles = todosDocentes,
                    DocentesAsignados = docentesAsignados,
                    TieneEstudiantes = tieneEstudiantes,
                    TieneEvaluaciones = tieneEvaluaciones,
                    PuedeEditarDocentes = puedeEditar
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Edit GET: {ex.Message}");
                TempData["Error"] = "Error al cargar los datos";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CursoCuatrimestre/Details/5
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Obtener el curso-cuatrimestre con detalles
                var response = await client.GetAsync($"CursoCuatrimestre/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Curso-Cuatrimestre no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var content = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<CursoCuatrimestreDTO>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Obtener cuatrimestre
                var cuatrimestreResponse = await client.GetAsync($"CursoCuatrimestre/cuatrimestres/{dto.CuatrimestreId}");
                var cuatrimestreContent = await cuatrimestreResponse.Content.ReadAsStringAsync();
                var cuatrimestreData = JsonSerializer.Deserialize<CuatrimestreDTO>(cuatrimestreContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var cuatrimestre = new CuatrimestreViewModel
                {
                    CuatrimestreId = cuatrimestreData.CuatrimestreId,
                    Nombre = cuatrimestreData.Nombre,
                    Anio = cuatrimestreData.Anio,
                    Numero = cuatrimestreData.Numero,
                    Fec_Inicio = cuatrimestreData.Fec_Inicio,
                    Fec_Fin = cuatrimestreData.Fec_Fin
                };

                // Obtener curso
                var cursoResponse = await client.GetAsync($"CursoCuatrimestre/cursos/{dto.CursoId}");
                var cursoContent = await cursoResponse.Content.ReadAsStringAsync();
                var cursoData = JsonSerializer.Deserialize<CursoDTO>(cursoContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var curso = new CursoViewModel
                {
                    CursoId = cursoData.CursoId,
                    Codigo = cursoData.Codigo,
                    Nom_Curso = cursoData.Nom_Curso,
                    Desc_Curso = cursoData.Desc_Curso ,
                    Num_Creditos = cursoData.Num_Creditos
                };

                // Mapear docentes
                var docentes = dto.Docentes?.Select(d => new DocenteAsignadoViewModel
                {
                    CursoCuatriDocenteId = d.CursoCuatriDocenteId,
                    DocenteId = d.DocenteId,
                    NombreCompleto = d.NombreCompleto,
                    Email = d.Email
                }).ToList() ?? new List<DocenteAsignadoViewModel>();

                var model = new CursoCuatrimestreDetailViewModel
                {
                    CursoCuatrimestreId = dto.CursoCuatrimestreId,
                    Cuatrimestre = cuatrimestre,
                    Curso = curso,
                    Docentes = docentes,
                    TotalEstudiantes = dto.TotalEstudiantes,
                    TieneEvaluaciones = dto.TieneEvaluaciones
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Details: {ex.Message}");
                TempData["Error"] = "Error al cargar los datos";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: CursoCuatrimestre/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] CreateCursoCuatrimestreDTO dto)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var jsonContent = JsonSerializer.Serialize(dto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("CursoCuatrimestre", content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Curso-Cuatrimestre creado exitosamente" });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest(new { success = false, message = errorContent });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Create POST: {ex.Message}");
                return BadRequest(new { success = false, message = "Error al crear el registro" });
            }
        }

        // POST: CursoCuatrimestre/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.DeleteAsync($"CursoCuatrimestre/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Eliminado exitosamente" });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorData = JsonSerializer.Deserialize<JsonElement>(errorContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var message = errorData.TryGetProperty("message", out var msg)
                    ? msg.GetString()
                    : "Error al eliminar";

                return BadRequest(new { success = false, message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Delete: {ex.Message}");
                return BadRequest(new { success = false, message = "Error al eliminar el registro" });
            }
        }

        // POST: CursoCuatrimestre/AsignarDocente
        [HttpPost] 
        public async Task<IActionResult> RegistrarDocente([FromBody] AsignarDocenteGeneralDTO dto)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var jsonContent = JsonSerializer.Serialize(dto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("CursoCuatrimestre/AsignarDocente", content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Docente asignado exitosamente" });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorData = JsonSerializer.Deserialize<JsonElement>(errorContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var message = errorData.TryGetProperty("message", out var msg)
                    ? msg.GetString()
                    : "Error al asignar docente";

                return BadRequest(new { success = false, message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en AsignarDocente: {ex.Message}");
                return BadRequest(new { success = false, message = "Error al asignar docente" });
            }
        }

        // POST: CursoCuatrimestre/RemoverDocente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverDocente(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.DeleteAsync($"CursoCuatrimestre/RemoverDocente/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Docente removido exitosamente" });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                var errorData = JsonSerializer.Deserialize<JsonElement>(errorContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var message = errorData.TryGetProperty("message", out var msg)
                    ? msg.GetString()
                    : "Error al remover docente";

                return BadRequest(new { success = false, message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en RemoverDocente: {ex.Message}");
                return BadRequest(new { success = false, message = "Error al remover docente" });
            }
        }
    }   
}