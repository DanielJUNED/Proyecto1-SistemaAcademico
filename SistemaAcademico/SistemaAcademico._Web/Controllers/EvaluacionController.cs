using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico._Web.Models;
using SistemaAcademico._Web.Models.ViewModels;
using SistemaAcademico._Web.Utils;
using SistemaAcademico._Web.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SistemaAcademico.Controllers
{
    [Authorize(Roles = "Docente,Administrador")]
    public class EvaluacionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly EvaluacionDB _dbEvaluacion;
        private readonly UserManager<IdentityUser> _userManager;

        public EvaluacionController(
            ApplicationDbContext db,
            EvaluacionDB dbEvaluacion,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _dbEvaluacion = dbEvaluacion;
            _userManager = userManager;
        }
        // =============================================
        // GET: /Evaluacion/Index
        // =============================================
        public IActionResult Index()
        {
            ViewBag.TiposParticipacion = EvaluacionOptions.TiposParticipacion;
            ViewBag.Estados = EvaluacionOptions.Estados;
            return View();
        }

        // =============================================
        // GET: /Evaluacion/BuscarEstudiantes (AJAX)
        // =============================================
        [HttpGet]
        public async Task<IActionResult> BuscarEstudiantes(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio) || criterio.Length < 2)
                {
                    return Json(new
                    {
                        success = true,
                        data = new List<BusquedaEstudianteViewModel>(),
                        message = "Ingrese al menos 2 caracteres para buscar"
                    });
                }

                var estudiantes = await _dbEvaluacion.BuscarEstudiantesAsync(criterio);

                return Json(new
                {
                    success = true,
                    data = estudiantes,
                    count = estudiantes.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al buscar estudiantes",
                    error = ex.Message
                });
            }
        }

        // =============================================
        // GET: /Evaluacion/ObtenerEstudiante (AJAX)
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ObtenerEstudiante(int estudianteId)
        {
            try
            {
                bool permiteEvaluar = false;
                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id;

                var docenteId = await _db.Docente
                    .AsNoTracking()
                    .Where(d => d.UserId == userId)
                    .Select(d => d.DocenteId)
                    .FirstOrDefaultAsync();

                if (User.IsInRole("Administrador"))
                {
                    permiteEvaluar = true;
                }

                var estudiante = await _dbEvaluacion.ObtenerEstudianteDetalleAsync(estudianteId, docenteId);

                if (estudiante == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Estudiante no encontrado"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = estudiante,
                    permiteEvaluar
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener estudiante",
                    error = ex.Message
                });
            }
        }

        // =============================================
        // POST: /Evaluacion/RegistrarEvaluacion (AJAX)
        // =============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarEvaluacion([FromBody] RegistrarEvaluacionViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new EvaluacionResultViewModel
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = errors
                    });
                }

                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id;

                var docente = await _db.Docente
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.UserId == userId);

                var resultado = await _dbEvaluacion.RegistrarEvaluacionAsync(modelo, docente?.DocenteId ?? 0);

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new EvaluacionResultViewModel
                {
                    Success = false,
                    Message = "Error inesperado al registrar la evaluación",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // =============================================
        // POST: /Evaluacion/ActualizarEvaluacion (AJAX)
        // =============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEvaluacion([FromBody] ActualizarEvaluacionViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new EvaluacionResultViewModel
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = errors
                    });
                }

                var resultado = await _dbEvaluacion.ActualizarEvaluacionAsync(modelo);
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new EvaluacionResultViewModel
                {
                    Success = false,
                    Message = "Error inesperado al actualizar la evaluación",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // =============================================
        // GET: /Evaluacion/ObtenerEvaluacion (AJAX)
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ObtenerEvaluacion(int evaluacionId)
        {
            try
            {
                var evaluacion = await _dbEvaluacion.ObtenerEvaluacionPorIdAsync(evaluacionId);

                if (evaluacion == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Evaluación no encontrada"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = evaluacion
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener evaluación",
                    error = ex.Message
                });
            }
        }

        // =============================================
        // DISPOSE
        // =============================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}