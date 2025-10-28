using Microsoft.AspNet.Identity;
using SistemaAcademico.Data;
using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using SistemaAcademico.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            _dbEstadistica = new EstadisticaDB();
        }

        // =============================================
        // GET: /Estadisticas/Index
        // =============================================
        public async Task<ActionResult> Index()
        {

            var (userId, docenteId, roles) = ObtenerDatosUsuario();
            var listCuatri = new List<CuatrimestreOpcionViewModel>();
            if (roles.Contains("Administrador") && docenteId.HasValue)
            {
                docenteId = null;
            }
            if (roles.Contains("Administrador")|| roles.Contains("Docente"))
            {
                // lógica para Docente
                listCuatri = await _dbEstadistica.ObtenerCuatrimestresAsync(docenteId);
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
                var (userId, docenteId, roles) = ObtenerDatosUsuario();
                var listCurso = new List<CursoOpcionViewModel>();
                if (roles.Contains("Administrador") && docenteId.HasValue)
                {
                    docenteId = null;
                }
                if (roles.Contains("Administrador") || roles.Contains("Docente"))
                {
                    // lógica para Docente o admin(docente is null)
                    listCurso = await _dbEstadistica.ObtenerCursosPorCuatrimestreAsync(cuatrimestreId, docenteId);
                }  

                return Json(new
                {
                    success = true,
                    data = listCurso
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
                var (userId, docenteId, roles) = ObtenerDatosUsuario();
                var estadisticas = new EstadisticasViewModel();
                if (roles.Contains("Administrador") && docenteId.HasValue)
                {
                    docenteId = null;
                }
                if (roles.Contains("Administrador") || roles.Contains("Docente"))
                {
                    // lógica para Docente o admin(docente is null)
                    estadisticas = await _dbEstadistica.ObtenerEstadisticasAsync(cuatrimestreId, cursoId, docenteId);
                } 
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
                var (userId, docenteId, roles) = ObtenerDatosUsuario();
                var comparativa = new ComparativaCursosViewModel();
                if (roles.Contains("Administrador") && docenteId.HasValue)
                {
                    docenteId = null;
                }
                if (roles.Contains("Administrador") || roles.Contains("Docente"))
                {
                    // lógica para Docente o admin(docente is null)
                    comparativa = await _dbEstadistica.ObtenerComparativaCursosAsync(cuatrimestreId, docenteId); 
                } 
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
        private (string UserId, int? DocenteId, List<string> Roles) ObtenerDatosUsuario()
        {
            var userId = User.Identity.GetUserId();

            // Obtener roles
            var roleIds =  _db.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Roles.Select(r => r.RoleId))
                .ToList();

            var roles = _db.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToList();

            // Obtener docenteId (si aplica)
            var docenteId = _db.Docente
                .Where(d => d.UserId == userId)
                .Select(d => (int?)d.DocenteId)
                .FirstOrDefault();

            return (userId, docenteId, roles);
        }
    }
}