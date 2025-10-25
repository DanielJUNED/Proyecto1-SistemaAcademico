using Microsoft.AspNet.Identity;
using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaAcademico.Data
{
    public class EvaluacionDB 
    {
        private readonly ApplicationDbContext _db;

        public EvaluacionDB(ApplicationDbContext context)
        {
            _db = context;
        }

        // =============================================
        // BUSCAR ESTUDIANTES
        // =============================================
        public async Task<List<BusquedaEstudianteViewModel>> BuscarEstudiantesAsync(string criterio)
        {
            var criterioBusqueda = criterio.Trim().ToLower();

            var estudiantes = await _db.Estudiante
                .Where(e => e.Ind_Estado =="A" &&
                    (e.Nombre.ToLower().Contains(criterioBusqueda) ||
                        e.Apellidos.ToLower().Contains(criterioBusqueda) ||
                        e.Identificacion.Contains(criterioBusqueda)))
                .Include(e => e.Distrito.Canton.Provincia)
                .Include(e => e.EstudianteCurso.Select(ec => ec.CursoCuatrimestre.Curso))
                .Include(e => e.EstudianteCurso.Select(ec => ec.CursoCuatrimestre.Cuatrimestre))
                .OrderBy(e => e.Nombre)
                .ThenBy(e => e.Apellidos)
                .Take(10) // Limitar resultados
                .ToListAsync();

            var resultado = estudiantes.Select(e => new BusquedaEstudianteViewModel
            {
                EstudianteID = e.EstudianteId,
                Identificacion = e.Identificacion,
                NombreCompleto = $"{e.Nombre} {e.Apellidos}",
                Email = e.Email,
                DireccionCompleta = $"{e.Distrito.Nom_Distrito}, {e.Distrito.Canton.Nom_Canton}, {e.Distrito.Canton.Provincia.Nom_Provincia}",
                Fec_Nacimiento = e.Fec_Nacimiento,
                Edad = CalcularEdad(e.Fec_Nacimiento),
                CursosMatriculados = e.EstudianteCurso
                    .Where(ec => ec.Ind_Estado =="A")
                    .Select(ec => new CursoMatriculadoViewModel
                    {
                        EstudianteCursoID = ec.EstudianteCursoId,
                        CursoID = ec.CursoCuatrimestre.CursoId,
                        CodigoCurso = ec.CursoCuatrimestre.Curso.Codigo,
                        NombreCurso = ec.CursoCuatrimestre.Curso.Nom_Curso,
                        NombreCuatrimestre = ec.CursoCuatrimestre.Cuatrimestre.Nombre,
                        CuatrimestreID = ec.CursoCuatrimestre.CuatrimestreId,
                        Creditos = ec.CursoCuatrimestre.Curso.Num_Creditos,
                        TieneEvaluacion = _db.Evaluacion.Any(ev => ev.EstudianteCursoId == ec.EstudianteCursoId),
                        NotaActual = _db.Evaluacion
                            .Where(ev => ev.EstudianteCursoId == ec.EstudianteCursoId)
                            .Select(ev => (decimal?)ev.Nota)
                            .FirstOrDefault(),
                        EstadoActual = _db.Evaluacion
                            .Where(ev => ev.EstudianteCursoId == ec.EstudianteCursoId)
                            .Select(ev => ev.Estado)
                            .FirstOrDefault()
                    })
                    .ToList()
            }).ToList();

            return resultado;
        }

        // =============================================
        // OBTENER DETALLE DE ESTUDIANTE
        // =============================================
        public async Task<BusquedaEstudianteViewModel> ObtenerEstudianteDetalleAsync(int estudianteId)
        {
            var estudiante = await _db.Estudiante
                .Where(e => e.EstudianteId == estudianteId && e.Ind_Estado =="A")
                .Include(e => e.Distrito.Canton.Provincia)
                .Include(e => e.EstudianteCurso.Select(ec => ec.CursoCuatrimestre.Curso))
                .Include(e => e.EstudianteCurso.Select(ec => ec.CursoCuatrimestre.Cuatrimestre))
                .FirstOrDefaultAsync();

            if (estudiante == null) return null;

            return new BusquedaEstudianteViewModel
            {
                EstudianteID      = estudiante.EstudianteId,
                Identificacion    = estudiante.Identificacion,
                NombreCompleto    = $"{estudiante.Nombre} {estudiante.Apellidos}",
                Email             = estudiante.Email,
                DireccionCompleta = $"{estudiante.Distrito.Nom_Distrito}, {estudiante.Distrito.Canton.Nom_Canton}, {estudiante.Distrito.Canton.Provincia.Nom_Provincia}",
                Fec_Nacimiento    = estudiante.Fec_Nacimiento,
                Edad              = CalcularEdad(estudiante.Fec_Nacimiento),
                CursosMatriculados = estudiante.EstudianteCurso
                    .Where(ec => ec.Ind_Estado == "A")
                    .Select(ec => new
                    {
                        ec,
                        Evaluacion = _db.Evaluacion
                            .Where(ev => ev.EstudianteCursoId == ec.EstudianteCursoId)
                            .FirstOrDefault()
                    })
                   .Select(x => new CursoMatriculadoViewModel
                   {
                       EstudianteCursoID  = x.ec.EstudianteCursoId,
                       CursoID            = x.ec.CursoCuatrimestre.CursoId,
                       CodigoCurso        = x.ec.CursoCuatrimestre.Curso.Codigo,
                       NombreCurso        = x.ec.CursoCuatrimestre.Curso.Nom_Curso,
                       NombreCuatrimestre = x.ec.CursoCuatrimestre.Cuatrimestre.Nombre,
                       CuatrimestreID     = x.ec.CursoCuatrimestre.CuatrimestreId,
                       Creditos           = x.ec.CursoCuatrimestre.Curso.Num_Creditos,
                       TieneEvaluacion    = x.Evaluacion != null,
                       EvaluacionId       = x.Evaluacion != null ? x.Evaluacion.EvaluacionId :(int?) null,
                       TipoParticipacion  = x.Evaluacion != null ? x.Evaluacion.TipoParticipacion : null,
                       Observacion        = x.Evaluacion != null ? x.Evaluacion.Observaciones : null,
                       NotaActual         = x.Evaluacion != null ? (decimal?)x.Evaluacion.Nota : null,
                       EstadoActual       = x.Evaluacion != null ? x.Evaluacion.Estado : null
                   })
                    .ToList()
            };
        }

        // =============================================
        // REGISTRAR EVALUACIÓN
        // =============================================
        public async Task<EvaluacionResultViewModel> RegistrarEvaluacionAsync(
            RegistrarEvaluacionViewModel modelo, int docenteId)
        {
            var resultado = new EvaluacionResultViewModel();
            var pasoGuardado = "N";
            int evaluacionId; 
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // Verificar si ya existe evaluación
                    var evaluacionExistente = await _db.Evaluacion
                        .FirstOrDefaultAsync(e => e.EstudianteCursoId == modelo.EstudianteCursoID);

                    if (evaluacionExistente != null)
                    {
                        resultado.Success = false;
                        resultado.Message = "Ya existe una evaluación registrada para este curso. Use la opción de actualizar.";
                        return resultado;
                    }

                    // Crear nueva evaluación
                    var evaluacion = new Evaluacion
                    {
                        EstudianteCursoId = modelo.EstudianteCursoID,
                        DocenteId = docenteId,
                        Nota = modelo.Nota,
                        Observaciones = modelo.Observaciones?.Trim(),
                        TipoParticipacion = modelo.TipoParticipacion,
                        Estado = modelo.Estado,
                        Fec_Evaluacion = DateTime.Now,
                        Fec_Registro = DateTime.Now,
                        Ind_Estado = "A"
                    };

                    _db.Evaluacion.Add(evaluacion);
                    await _db.SaveChangesAsync();

                    transaction.Commit();
                    pasoGuardado = "S";
                    evaluacionId = evaluacion.EvaluacionId;
                }
                catch (Exception ex)
                {
                Console.WriteLine("Error principal: " + ex.Message);

                    var inner = ex.InnerException;
                    int nivel = 1;

                    while (inner != null)
                    {
                        Console.WriteLine($"Inner Exception {nivel++}: {inner.Message}");
                        inner = inner.InnerException;
                    }
                    transaction.Rollback();
                    resultado.Success = false;
                    resultado.Message = "Error al registrar la evaluación";

                    var detalle = ex.InnerException?.InnerException?.Message
                                    ?? ex.InnerException?.Message
                                    ?? ex.Message;
                    resultado.Errors.Add(detalle);
                    // resultado.Errors.Add(ex.Message);
                        

                    throw; // opcional, para depuración
                }
            }
            if (pasoGuardado == "S")
            {
                // Obtener detalle completo
                var detalle = await ObtenerEvaluacionPorIdAsync(evaluacionId);

                resultado.Success = true;
                resultado.Message = "Evaluación registrada exitosamente";
                resultado.EvaluacionID = evaluacionId;
                resultado.Evaluacion = detalle;
            }
            return resultado;
        }

        // =============================================
        // ACTUALIZAR EVALUACIÓN
        // =============================================
        public async Task<EvaluacionResultViewModel> ActualizarEvaluacionAsync(ActualizarEvaluacionViewModel modelo)
        {
            var resultado = new EvaluacionResultViewModel();

            try
            {
                var evaluacion = await _db.Evaluacion.FindAsync(modelo.EvaluacionID);

                if (evaluacion == null)
                {
                    resultado.Success = false;
                    resultado.Message = "Evaluación no encontrada";
                    return resultado;
                }

                // Actualizar datos
                evaluacion.Nota = modelo.Nota;
                evaluacion.Observaciones = modelo.Observaciones?.Trim();
                evaluacion.TipoParticipacion = modelo.TipoParticipacion;
                evaluacion.Estado = modelo.Estado;  
                //evaluacion.FechaModificacion = DateTime.Now;

                await _db.SaveChangesAsync();

                // Obtener detalle actualizado
                var detalle = await ObtenerEvaluacionPorIdAsync(evaluacion.EvaluacionId);

                resultado.Success = true;
                resultado.Message = "Evaluación actualizada exitosamente";
                resultado.EvaluacionID = evaluacion.EvaluacionId;
                resultado.Evaluacion = detalle;
            }
            catch (Exception ex)
            {
                resultado.Success = false;
                resultado.Message = "Error al actualizar la evaluación";
                resultado.Errors.Add(ex.Message);
            }

            return resultado;
        }

        // =============================================
        // OBTENER EVALUACIÓN POR ID
        // =============================================
        public async Task<EvaluacionDetalleViewModel> ObtenerEvaluacionPorIdAsync(int evaluacionId)
        {
            var evaluacion = new Evaluacion();
            using (var appdb = new ApplicationDbContext())
            {
                evaluacion = await appdb.Evaluacion
                                .Where(e => e.EvaluacionId == evaluacionId)
                                .Include(e => e.EstudianteCurso.Estudiante)
                                .Include(e => e.EstudianteCurso.CursoCuatrimestre.Curso)
                                .Include(e => e.EstudianteCurso.CursoCuatrimestre.Cuatrimestre)
                                .Include(e => e.Docente).AsNoTracking()
                                .FirstOrDefaultAsync();
            }
            

            if (evaluacion == null) return null;

            return new EvaluacionDetalleViewModel
            {
                EvaluacionID = evaluacion.EvaluacionId,
                NombreEstudiante = $"{evaluacion.EstudianteCurso.Estudiante.Nombre} {evaluacion.EstudianteCurso.Estudiante.Apellidos}",
                IdentificacionEstudiante = evaluacion.EstudianteCurso.Estudiante.Identificacion,
                CodigoCurso = evaluacion.EstudianteCurso.CursoCuatrimestre.Curso.Codigo,
                NombreCurso = evaluacion.EstudianteCurso.CursoCuatrimestre.Curso.Nom_Curso,
                NombreCuatrimestre = evaluacion.EstudianteCurso.CursoCuatrimestre.Cuatrimestre.Nombre,
                Nota = evaluacion.Nota,
                Observaciones = evaluacion.Observaciones,
                TipoParticipacion = evaluacion.TipoParticipacion,
                Estado = evaluacion.Estado,
                FechaEvaluacion = (DateTime)evaluacion.Fec_Evaluacion,
                NombreDocente = $"{evaluacion.Docente.Nombre} {evaluacion.Docente.Apellidos}"
            };
        }

        // =============================================
        // VERIFICAR SI EXISTE EVALUACIÓN
        // =============================================
        public async Task<bool> ExisteEvaluacionAsync(int estudianteCursoId)
        {
            return await _db.Evaluacion
                .AnyAsync(e => e.EstudianteCursoId == estudianteCursoId);
        }
    // =============================================
    // VERIFICAR SI EXISTE EVALUACIÓN
    // =============================================
    public async Task<bool> ObtEvaluacionPorAsync(int estudianteCursoId)
    {
        return await _db.Evaluacion
            .AnyAsync(e => e.EstudianteCursoId == estudianteCursoId);
    }
    // =============================================
    // HELPER: Calcular Edad
    // =============================================
    private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }
    }
}