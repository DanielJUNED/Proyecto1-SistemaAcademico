using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SistemaAcademico.App_Start;
using SistemaAcademico.Data;
using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using SistemaAcademico.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaAcademico.Controllers
{
    [Authorize(Roles = "Docente,Administrador")]
    public class EvaluacionController : Controller
    {
        private ApplicationDbContext _db;

        private EvaluacionDB _dbEvaluacion;
        public EvaluacionController()
        {
            _db = new ApplicationDbContext();
            _dbEvaluacion = new EvaluacionDB(_db);
        }
        // =============================================
        // GET: /Evaluacion/Index
        // =============================================
        public ActionResult Index()
        {
            ViewBag.TiposParticipacion = EvaluacionOptions.TiposParticipacion;
            ViewBag.Estados = EvaluacionOptions.Estados;
            return View();
        }
        // =============================================
        // GET: /Evaluaciones/BuscarEstudiantes (AJAX)
        // =============================================
        [HttpGet]
        public async Task<JsonResult> BuscarEstudiantes(string criterio)
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
                    }, JsonRequestBehavior.AllowGet);
                }

                var estudiantes = await _dbEvaluacion.BuscarEstudiantesAsync(criterio);

                return Json(new
                {
                    success = true,
                    data = estudiantes,
                    count = estudiantes.Count
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al buscar estudiantes",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // GET: /Evaluaciones/ObtenerEstudiante (AJAX)
        // =============================================
        [HttpGet]
        public async Task<JsonResult> ObtenerEstudiante(int estudianteId)
        {
            var PermiteEvaluar = false;
            try
            { 
                //Permiso para evaluar cursos(Habilitar btn evaluar)
                var userId      = User.Identity.GetUserId();
                var docenteId   = _db.Docente.AsNoTracking().FirstOrDefault(d => d.UserId == userId).DocenteId;

                if (User.IsInRole("Administrador"))
                {
                    PermiteEvaluar = true;
                } 
                var estudiante = await _dbEvaluacion.ObtenerEstudianteDetalleAsync(estudianteId, docenteId); 

                if (estudiante == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Estudiante no encontrado"
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    data = estudiante,
                    permiteEvaluar = PermiteEvaluar
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener estudiante",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // POST: /Evaluaciones/RegistrarEvaluacion (AJAX)
        // =============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegistrarEvaluacion(RegistrarEvaluacionViewModel modelo)
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

                var UserId = User.Identity.GetUserId();
                var docente = _db.Docente.AsNoTracking().FirstOrDefault(d => d.UserId == UserId);
                var resultado = await _dbEvaluacion.RegistrarEvaluacionAsync(modelo, docente.DocenteId);

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new EvaluacionResultViewModel
                {
                    Success = false,
                    Message = "Error inesperado al registrar la evaluación",
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }

        // =============================================
        // POST: /Evaluaciones/ActualizarEvaluacion (AJAX)
        // =============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ActualizarEvaluacion(ActualizarEvaluacionViewModel modelo)
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
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }

        // =============================================
        // GET: /Evaluaciones/ObtenerEvaluacion (AJAX)
        // =============================================
        [HttpGet]
        public async Task<JsonResult> ObtenerEvaluacion(int evaluacionId)
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
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    data = evaluacion
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener evaluación",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
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