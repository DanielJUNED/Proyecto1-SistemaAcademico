using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using SistemaAcademico._Web.Models;
using SistemaAcademico._Web.Models.ViewModels; 
using SistemaAcademico._Web.Repository; 
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SistemaAcademico._Web.Controllers
{
    [Authorize(Roles = "Docente,Administrador")]
    public class EstadisticaController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly EstadisticaDB _dbEstadistica;
        private readonly UserManager<ApplicationUser> _userManager;

        public EstadisticaController(
            ApplicationDbContext db,
            EstadisticaDB dbEstadistica,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _dbEstadistica = dbEstadistica;
            _userManager = userManager;
        }

        // =============================================
        // GET: /Estadistica/Index
        // =============================================
        public async Task<IActionResult> Index()
        {
            var (userId, docenteId, roles) = await ObtenerDatosUsuarioAsync();
            var listCuatri = new List<CuatrimestreOpcionViewModel>();

            if (roles.Contains("Administrador") && docenteId.HasValue)
                docenteId = null;

            if (roles.Contains("Administrador") || roles.Contains("Docente"))
                listCuatri = await _dbEstadistica.ObtenerCuatrimestresAsync(docenteId);

            ViewBag.Cuatrimestres = listCuatri;

            return View();
        }

        // =============================================
        // GET: /Estadistica/ObtenerCuatrimestres
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ObtenerCuatrimestres()
        {
            try
            {
                var cuatrimestres = await _dbEstadistica.ObtenerCuatrimestresAsync();
                return Json(new { success = true, data = cuatrimestres });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener cuatrimestres", error = ex.Message });
            }
        }

        // =============================================
        // GET: /Estadistica/ObtenerCursos
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ObtenerCursos(int cuatrimestreId)
        {
            try
            {
                var (userId, docenteId, roles) = await ObtenerDatosUsuarioAsync();
                var listCurso = new List<CursoOpcionViewModel>();

                if (roles.Contains("Administrador") && docenteId.HasValue)
                    docenteId = null;

                if (roles.Contains("Administrador") || roles.Contains("Docente"))
                    listCurso = await _dbEstadistica.ObtenerCursosPorCuatrimestreAsync(cuatrimestreId, docenteId);

                return Json(new { success = true, data = listCurso });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener cursos", error = ex.Message });
            }
        }

        // =============================================
        // GET: /Estadistica/ObtenerEstadisticas
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ObtenerEstadisticas(int? cuatrimestreId, int? cursoId)
        {
            try
            {
                if (!cuatrimestreId.HasValue)
                    return Json(new { success = false, message = "Debe seleccionar un cuatrimestre" });

                var (userId, docenteId, roles) = await ObtenerDatosUsuarioAsync();
                var estadisticas = new EstadisticasViewModel();

                if (roles.Contains("Administrador") && docenteId.HasValue)
                    docenteId = null;

                if (roles.Contains("Administrador") || roles.Contains("Docente"))
                    estadisticas = await _dbEstadistica.ObtenerEstadisticasAsync(cuatrimestreId, cursoId, docenteId);

                return Json(new { success = true, data = estadisticas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener estadísticas", error = ex.Message });
            }
        }

        // =============================================
        // GET: /Estadistica/ObtenerComparativa
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ObtenerComparativa(int cuatrimestreId)
        {
            try
            {
                var (userId, docenteId, roles) = await ObtenerDatosUsuarioAsync();
                var comparativa = new ComparativaCursosViewModel();

                if (roles.Contains("Administrador") && docenteId.HasValue)
                    docenteId = null;

                if (roles.Contains("Administrador") || roles.Contains("Docente"))
                    comparativa = await _dbEstadistica.ObtenerComparativaCursosAsync(cuatrimestreId, docenteId);

                return Json(new { success = true, data = comparativa });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener comparativa", error = ex.Message });
            }
        }

        // =============================================
        // MÉTODO PRIVADO
        // =============================================
        private async Task<(string UserId, int? DocenteId, List<string> Roles)> ObtenerDatosUsuarioAsync()
        {
            var userId = _userManager.GetUserId(User);
            var roles = (await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User))).ToList();

            var docenteId = await _db.Docente
                .Where(d => d.UserId == userId)
                .Select(d => (int?)d.DocenteId)
                .FirstOrDefaultAsync();

            return (userId, docenteId, roles);
        }
    }
}
