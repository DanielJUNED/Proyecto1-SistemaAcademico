using Microsoft.AspNetCore.Mvc;
using SistemaAcademico._Web.Models.ViewModels;
using SistemaAcademico.API.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SistemaAcademico._Web.Controllers
{
    public class CursoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CursoController> _logger;
        public CursoController(IHttpClientFactory httpClientFactory, ILogger<CursoController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        // GET: Cursos
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Debería llamar a: http://localhost:5275/api/cursos
                var response = await client.GetAsync("cursos");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var cursosDTO = JsonSerializer.Deserialize<List<CursoDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    // DTO → ViewModel
                    var viewModels = cursosDTO.Select(dto => new CursoListViewModel
                    {
                        CursoId = dto.CursoId,
                        Codigo = dto.Codigo,
                        Nombre = dto.Nom_Curso,
                        Descripcion = dto.Desc_Curso,
                        Creditos = dto.Num_Creditos
                    }).ToList();


                    return View(viewModels);
                }

                ViewBag.Error = "No se pudieron cargar los cursos";
                return View(new List<CursoListViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<CursoListViewModel>());
            }
        }

        // GET: Curso/Crear
        public ActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] CursoFormViewModel viewModel)
        {

            // Validación del modelo
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    mensaje = "Datos inválidos",
                    errores = errores
                });
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                // ViewModel → DTO
                var dto = new CrearCursoDTO
                {
                    Codigo = viewModel.Codigo,
                    Nom_Curso = viewModel.Nombre,
                    Desc_Curso = viewModel.Descripcion,
                    Num_Creditos = viewModel.Creditos
                };

                var jsonContent = JsonSerializer.Serialize(dto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("cursos", content);

                if (response.IsSuccessStatusCode)
                { 
                    return Ok(new
                    {
                        success = true,
                        mensaje = "Curso creado exitosamente"
                    });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
               // ModelState.AddModelError("", $"Error al crear curso: {errorContent}");
                return BadRequest(new
                {
                    success = false,
                    mensaje = $"Error al crear curso: {errorContent}",
                    errores = new[] { errorContent }
                });
            }
            catch (Exception ex)
            { 
                //ModelState.AddModelError("", "Error al crear curso");
                return BadRequest(new
                {
                    success = false,
                    mensaje = "Error inesperado",
                    errores = new[] { ex.Message }
                });
            }
        }

        // GET: Curso/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.GetAsync($"cursos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var cursoDTO = JsonSerializer.Deserialize<CursoDTO>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // DTO → ViewModel
                    var viewModel = new CursoFormViewModel
                    {
                        CursoId = cursoDTO.CursoId,
                        Codigo = cursoDTO.Codigo,
                        Nombre = cursoDTO.Nom_Curso,
                        Descripcion = cursoDTO.Desc_Curso,
                        Creditos = cursoDTO.Num_Creditos
                    };

                    return View(viewModel);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return NotFound();
            }
        }

        // POST: Cursos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, CursoFormViewModel viewModel)

        {
            if (id != viewModel.CursoId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var dto = new CursoDTO
                {
                    CursoId = viewModel.CursoId,
                    Codigo = viewModel.Codigo,
                    Nom_Curso = viewModel.Nombre,
                    Desc_Curso = viewModel.Descripcion,
                    Num_Creditos = viewModel.Creditos
                };

                var jsonContent = JsonSerializer.Serialize(dto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"cursos/{id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                return View(viewModel);
            }
            catch
            {
                return View(viewModel);
            }
        }
    }
}