using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SistemaAcademico._Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5275/");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("weatherforecast");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    // Puedes deserializar si el endpoint devuelve JSON
                    // var weather = JsonSerializer.Deserialize<List<WeatherForecast>>(data);
                    return View(new List<string> { data });
                }
                else
                {
                    ViewBag.Error = $"Error al obtener datos: {response.StatusCode}";
                    return View(new List<string>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error en la solicitud: {ex.Message}";
                return View(new List<string>());
            }
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
