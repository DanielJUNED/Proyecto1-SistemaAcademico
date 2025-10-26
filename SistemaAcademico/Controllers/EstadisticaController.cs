using Microsoft.AspNet.Identity;
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
    public class EstadisticaController : Controller
    {
        private ApplicationDbContext _db;

        private EstadisticaDB _dbEstadistica;
        public EstadisticaController()
        {
            _db = new ApplicationDbContext();
            _dbEstadistica = new EstadisticaDB(_db);
        }
        // =============================================
        // GET: /Estadisticas/Index
        // =============================================
        public async Task<ActionResult> Index()
        {

            var userId = User.Identity.GetUserId(); 
            var roles = _db.Users.Where(u => u.Id == userId)
                            .SelectMany(u => u.Roles)
                            .ToList();
            var listCuatri = new List<CuatrimestreOpcionViewModel>();
            if (User.IsInRole("Docente"))
            {
                // lógica para Docente
                listCuatri = await _dbEstadistica.ObtenerCuatrimestresAsync();
            }
            if (User.IsInRole("Administrador"))
            {
                // lógica para administradores
                listCuatri = await _dbEstadistica.ObtenerCuatrimestresAsync();
            }
            // Cargar cuatrimestres para el dropdown
            ViewBag.Cuatrimestres = listCuatri;

            return View();
        }

        // =============================================
        // GET: /Estadisticas/ObtenerCuatrimestres (AJAX)
        // =============================================
        [HttpGet]
        public async Task<JsonResult> ObtenerCuatrimestres()
        {
            try
            {
                var cuatrimestres = await _dbEstadistica.ObtenerCuatrimestresAsync();

                return Json(new
                {
                    success = true,
                    data = cuatrimestres
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener cuatrimestres",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // GET: /Estadisticas/ObtenerCursos (AJAX)
        // =============================================
        [HttpGet]
        public async Task<JsonResult> ObtenerCursos(int cuatrimestreId)
        {
            try
            {
                var cursos = await _dbEstadistica.ObtenerCursosPorCuatrimestreAsync(cuatrimestreId);

                return Json(new
                {
                    success = true,
                    data = cursos
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener cursos",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // GET: /Estadisticas/ObtenerEstadisticas (AJAX)
        // =============================================
        [HttpGet]
        public async Task<JsonResult> ObtenerEstadisticas(int? cuatrimestreId, int? cursoId)
        {
            try
            {
                if (!cuatrimestreId.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Debe seleccionar un cuatrimestre"
                    }, JsonRequestBehavior.AllowGet);
                }

                var estadisticas = await _dbEstadistica.ObtenerEstadisticasAsync(cuatrimestreId, cursoId);

                return Json(new
                {
                    success = true,
                    data = estadisticas
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener estadísticas",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =============================================
        // GET: /Estadisticas/ObtenerComparativa (AJAX)
        // =============================================
        [HttpGet]
        public async Task<JsonResult> ObtenerComparativa(int cuatrimestreId)
        {
            try
            {
                var comparativa = await _dbEstadistica.ObtenerComparativaCursosAsync(cuatrimestreId);

                return Json(new
                {
                    success = true,
                    data = comparativa
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener comparativa",
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