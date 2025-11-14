using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico._Web.Models;
using SistemaAcademico._Web.Models.ViewModels;
using SistemaAcademico._Web.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaAcademico._Web.Controllers
{
    [Authorize]
    public class EstudianteController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly EstudianteDB _repo;

        public EstudianteController(ApplicationDbContext db, EstudianteDB repo)
        {
            _db = db;
            _repo = repo;
        }

        // GET: Estudiantes/Registrar
        [HttpGet]
        public IActionResult Registrar()
        {
            ViewBag.Provincias = _repo.ObtProvincias();
            ViewBag.Cuatrimestres = _repo.ObtCuatrimestresActivos();

            return View(new EstudianteViewModel());
        }

        // POST: Estudiantes/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(EstudianteViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new ResultadoRegistro
                    {
                        Exitoso = false,
                        Mensaje = "Datos inválidos",
                        Errores = errores
                    });
                }

                // Validar edad
                var edad = DateTime.Now.Year - modelo.FechaNacimiento.Year;
                if (modelo.FechaNacimiento.Date > DateTime.Now.AddYears(-edad)) edad--;
                if (edad < 15)
                {
                    return Json(new ResultadoRegistro
                    {
                        Exitoso = false,
                        Mensaje = "El estudiante debe tener al menos 15 años"
                    });
                }

                // Validar cursos seleccionados
                if (modelo.CursosSeleccionados == null || !modelo.CursosSeleccionados.Any())
                {
                    return Json(new ResultadoRegistro
                    {
                        Exitoso = false,
                        Mensaje = "Debe seleccionar al menos un curso"
                    });
                }

                var resultado = _repo.RegistrarEstudiante(modelo);
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new ResultadoRegistro
                {
                    Exitoso = false,
                    Mensaje = "Error inesperado al procesar la solicitud",
                    Errores = new() { ex.Message }
                });
            }
        }

        // GET: API - Verificar Identificación
        [HttpGet]
        public IActionResult VerificarIdentificacion(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
                return Json(new { existe = false });

            var existe = _repo.ExisteIdentificacion(identificacion);
            return Json(new { existe });
        }

        // GET: API - Verificar Email
        [HttpGet]
        public IActionResult VerificarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { existe = false });

            var existe = _repo.ExisteEmail(email);
            return Json(new { existe });
        }

        // GET: API - Cantones
        [HttpGet]
        public IActionResult ObtenerCantones(int provinciaId)
        {
            var cantones = _repo.ObtCantonesPorProvincia(provinciaId);
            return Json(cantones.Select(c => new { value = c.CantonId, text = c.Nom_Canton }));
        }

        // GET: API - Distritos
        [HttpGet]
        public IActionResult ObtenerDistritos(int cantonId)
        {
            var distritos = _repo.ObtDistritosPorCanton(cantonId);
            return Json(distritos.Select(d => new { value = d.DistritoId, text = d.Nom_Distrito }));
        }

        // GET: API - Cursos
        [HttpGet]
        public IActionResult ObtenerCursos(int cuatrimestreId)
        {
            var cursos = _repo.ObtCursosPorCuatrimestre(cuatrimestreId);
            return Json(cursos.Select(c => new
            {
                value = c.CursoId,
                text = $"{c.Codigo} - {c.Nom_Curso}",
                codigo = c.Codigo,
                nombre = c.Nom_Curso,
                creditos = c.Num_Creditos
            }));
        }
    }
}
