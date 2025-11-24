using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico._Web.Models.ViewModels;
using SistemaAcademico.API.DTOs;
using System.Text.Json;

namespace SistemaAcademico._Web.Controllers
{

    [Authorize]
    public class BitacoraController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BitacoraController> _logger;

        public BitacoraController(IHttpClientFactory httpClientFactory, ILogger<BitacoraController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET: Bitacora
        public async Task<IActionResult> Index(
            string? nombreUsuario,
            string? accion,
            string? modulo,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string ordenarPor = "Fec_Registro",
            string direccion = "desc",
            int pagina = 1,
            int registrosPorPagina = 50)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Construir URL con parámetros
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(nombreUsuario))
                    queryParams.Add($"nombreUsuario={Uri.EscapeDataString(nombreUsuario)}");
                if (!string.IsNullOrEmpty(accion))
                    queryParams.Add($"accion={Uri.EscapeDataString(accion)}");
                if (!string.IsNullOrEmpty(modulo))
                    queryParams.Add($"modulo={Uri.EscapeDataString(modulo)}");
                if (fechaInicio.HasValue)
                    queryParams.Add($"fechaInicio={fechaInicio.Value:yyyy-MM-dd}");
                if (fechaFin.HasValue)
                    queryParams.Add($"fechaFin={fechaFin.Value:yyyy-MM-dd}");

                queryParams.Add($"ordenarPor={ordenarPor}");
                queryParams.Add($"direccion={direccion}");
                queryParams.Add($"pagina={pagina}");
                queryParams.Add($"registrosPorPagina={registrosPorPagina}");

                var url = $"Bitacora?{string.Join("&", queryParams)}";
                var response = await client.GetAsync(url);

                // Obtener listas para filtros
                var accionesResponse = await client.GetAsync("Bitacora/acciones");
                var modulosResponse = await client.GetAsync("Bitacora/modulos");

                var model = new BitacoraIndexViewModel
                {
                    NombreUsuario = nombreUsuario,
                    Accion = accion,
                    Modulo = modulo,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    OrdenarPor = ordenarPor,
                    Direccion = direccion,
                    PaginaActual = pagina,
                    RegistrosPorPagina = registrosPorPagina
                };

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<BitacoraPaginadaDto>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    model.Registros = resultado.Registros.Select(r => new BitacoraListViewModel
                    {
                        BitacoraId = r.BitacoraId,
                        UserId = r.UsuarioId,
                        NombreUsuario = r.NombreUsuario,
                        EmailUsuario = r.EmailUsuario,
                        Accion = r.Accion,
                        Modulo = r.Modulo,
                        Descripcion = r.Descripcion,
                        DireccionIP = r.DireccionIP,
                        Fec_Registro = r.Fec_Registro
                    }).ToList();

                    model.TotalRegistros = resultado.TotalRegistros;
                    model.TotalPaginas = resultado.TotalPaginas;
                }

                // Cargar listas de filtros
                if (accionesResponse.IsSuccessStatusCode)
                {
                    var accionesContent = await accionesResponse.Content.ReadAsStringAsync();
                    var acciones = JsonSerializer.Deserialize<List<string>>(accionesContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    model.Acciones = acciones?.Select(a => new AccionViewModel
                    {
                        Valor = a,
                        Texto = a
                    }).ToList() ?? new List<AccionViewModel>();
                }

                if (modulosResponse.IsSuccessStatusCode)
                {
                    var modulosContent = await modulosResponse.Content.ReadAsStringAsync();
                    var modulos = JsonSerializer.Deserialize<List<string>>(modulosContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    model.Modulos = modulos?.Select(m => new ModuloViewModel
                    {
                        Valor = m,
                        Texto = m
                    }).ToList() ?? new List<ModuloViewModel>();
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Index: {ex.Message}");
                ViewBag.Error = $"Error al cargar los registros de la bitácora {ex.Message}";
                return View(new BitacoraIndexViewModel());
            }
        }
    }
}
