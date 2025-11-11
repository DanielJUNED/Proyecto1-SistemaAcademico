using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using SistemaAcademico.Repository;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SistemaAcademico.Controllers
{
    [Authorize]
    public class EstudianteController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private EstudianteDB _dbEstudiante;

        // <summary>
        /// Obtiene  configuracion del repositorio actual
        /// </summary> 
        private EstudianteDB EstudianteRepo
        {
            get
            {
                if (_dbEstudiante == null)
                {
                    _dbEstudiante = new EstudianteDB();
                }

                return _dbEstudiante;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Estudiantes/Registrar
        [HttpGet]
        public ActionResult Registrar()
        {
            var repo = EstudianteRepo;
            // Cargar datos iniciales para los dropdowns
            ViewBag.Provincias = repo.ObtProvincias();
            ViewBag.Cuatrimestres = repo.ObtCuatrimestresActivos();

            return View(new EstudianteViewModel());
        }

        // POST: Estudiantes/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(EstudianteViewModel modelo)
        {
            try
            {
                // Validaciones del servidor
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
                    }, JsonRequestBehavior.AllowGet);
                }

                // Validar edad (debe ser mayor de 15 años)
                var edad = DateTime.Now.Year - modelo.FechaNacimiento.Year;
                if (modelo.FechaNacimiento.Date > DateTime.Now.AddYears(-edad)) edad--;

                if (edad < 15)
                {
                    return Json(new ResultadoRegistro
                    {
                        Exitoso = false,
                        Mensaje = "El estudiante debe tener al menos 15 años"
                    }, JsonRequestBehavior.AllowGet);
                }

                // Validar que se hayan seleccionado cursos
                if (modelo.CursosSeleccionados == null || !modelo.CursosSeleccionados.Any())
                {
                    return Json(new ResultadoRegistro
                    {
                        Exitoso = false,
                        Mensaje = "Debe seleccionar al menos un curso"
                    }, JsonRequestBehavior.AllowGet);
                }

                var repo = EstudianteRepo;
                // Registrar estudiante
                var resultado = repo.RegistrarEstudiante(modelo);

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResultadoRegistro
                {
                    Exitoso = false,
                    Mensaje = "Error inesperado al procesar la solicitud",
                    Errores = new System.Collections.Generic.List<string> { ex.Message }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Verificar si la identificación ya existe
        [HttpGet]
        public ActionResult VerificarIdentificacion(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return Json(new { existe = false });
            }
            var repo = EstudianteRepo;
            var existe = repo.ExisteIdentificacion(identificacion);
            return Json(new { existe }, JsonRequestBehavior.AllowGet);
        }

        // API: Verificar si el email ya existe
        [HttpGet]
        public ActionResult VerificarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { existe = false });
            }
            var repo = EstudianteRepo;
            var existe = repo.ExisteEmail(email);
            return Json(new { existe }, JsonRequestBehavior.AllowGet);
        }

        // API: Obtener cantones por provincia
        [HttpGet]
        public ActionResult ObtenerCantones(int provinciaId)
        {
            var repo = EstudianteRepo;
            var cantones = repo.ObtCantonesPorProvincia(provinciaId);
            return Json(cantones.Select(c => new { value = c.CantonId, text = c.Nom_Canton }), JsonRequestBehavior.AllowGet);
        }

        // API: Obtener distritos por cantón
        [HttpGet]
        public ActionResult ObtenerDistritos(int cantonId)
        {
            var repo = EstudianteRepo;
            var distritos = repo.ObtDistritosPorCanton(cantonId);
            return Json(distritos.Select(d => new { value = d.DistritoId, text = d.Nom_Distrito }), JsonRequestBehavior.AllowGet);
        }

        // API: Obtener cursos por cuatrimestre
        [HttpGet]
        public ActionResult ObtenerCursos(int cuatrimestreId)
        {
            var repo = EstudianteRepo;
            var cursos = repo.ObtCursosPorCuatrimestre(cuatrimestreId);
            return Json(cursos.Select(c => new
            {
                value = c.CursoId,
                text = $"{c.Codigo} - {c.Nom_Curso}",
                codigo = c.Codigo,
                nombre = c.Nom_Curso,
                creditos = c.Num_Creditos
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
