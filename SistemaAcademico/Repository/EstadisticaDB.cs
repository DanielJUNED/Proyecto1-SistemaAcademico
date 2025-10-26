using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace SistemaAcademico.Repository
{
    public class EstadisticaDB
    {
        private readonly ApplicationDbContext _db;

        public EstadisticaDB(ApplicationDbContext context)
        {
            _db = context;
        }

        // =============================================
        // OBTENER CUATRIMESTRES DISPONIBLES
        // =============================================
        public async Task<List<CuatrimestreOpcionViewModel>> ObtenerCuatrimestresAsync()
        { 
            var cuatrimestres = await _db.Cuatrimestre
                .Where(c => c.Ind_Estado == "A")
                .OrderByDescending(c => c.Anio)
                .ThenByDescending(c => c.Numero)
                .ToListAsync();

            var resultado = new List<CuatrimestreOpcionViewModel>();

            foreach (var cuatrimestre in cuatrimestres)
            {
                var totalCursos = await _db.CursoCuatrimestre
                    .Where(cc => cc.CuatrimestreId == cuatrimestre.CuatrimestreId && cc.Ind_Estado == "A")
                    .CountAsync();

                var totalEstudiantes = await _db.EstudianteCurso
                    .Where(ec => ec.CursoCuatrimestre.CuatrimestreId == cuatrimestre.CuatrimestreId && ec.Ind_Estado == "A")
                    .Select(ec => ec.EstudianteId)
                    .Distinct()
                    .CountAsync();

                resultado.Add(new CuatrimestreOpcionViewModel
                {
                    CuatrimestreID = cuatrimestre.CuatrimestreId,
                    Nombre = cuatrimestre.Nombre,
                    Anio = cuatrimestre.Anio,
                    Numero = cuatrimestre.Numero,
                    Ind_Estado = cuatrimestre.Ind_Estado,
                    TotalCursos = totalCursos,
                    TotalEstudiantes = totalEstudiantes
                });
            }

            return resultado;
        }

        // =============================================
        // OBTENER CURSOS POR CUATRIMESTRE
        // =============================================
        public async Task<List<CursoOpcionViewModel>> ObtenerCursosPorCuatrimestreAsync(int cuatrimestreId)
        {
            var cursos = await _db.Curso
                .Where(c => c.Ind_Estado == "A" &&
                    c.CursoCuatrimestre.Any(cc => cc.CuatrimestreId == cuatrimestreId && cc.Ind_Estado =="A"))
                .OrderBy(c => c.Codigo)
                .ToListAsync();

            var resultado = new List<CursoOpcionViewModel>();

            foreach (var curso in cursos)
            {
                var cursoCuatrimestre = await _db.CursoCuatrimestre
                    .FirstOrDefaultAsync(cc => cc.CursoId == curso.CursoId &&
                                              cc.CuatrimestreId == cuatrimestreId);

                var totalEstudiantes = await _db.EstudianteCurso
                    .Where(ec => ec.CursoCuatrimestreId == cursoCuatrimestre.CursoCuatrimestreId && ec.Ind_Estado == "A")
                    .CountAsync();

                var totalEvaluaciones = await _db.Evaluacion
                    .Where(e => e.EstudianteCurso.CursoCuatrimestreId == cursoCuatrimestre.CursoCuatrimestreId)
                    .CountAsync();

                resultado.Add(new CursoOpcionViewModel
                {
                    CursoID = curso.CursoId,
                    Codigo = curso.Codigo,
                    Nombre = curso.Nom_Curso,
                    Creditos = curso.Num_Creditos,
                    TotalEstudiantes = totalEstudiantes,
                    TotalEvaluaciones = totalEvaluaciones
                });
            }

            return resultado;
        }

        // =============================================
        // OBTENER ESTADÍSTICAS
        // =============================================
        public async Task<EstadisticasViewModel> ObtenerEstadisticasAsync(int? cuatrimestreId, int? cursoId)
        {
            var estadisticas = new EstadisticasViewModel
            {
                CuatrimestreID = cuatrimestreId,
                CursoID        = cursoId,
                Generales      = new EstadisticasGeneralesViewModel(),
                Graficos       = new EstadisticasGraficosViewModel(),
                Estudiantes    = new List<EstudianteEstadisticaViewModel>()
            };

            // Obtener nombres
            if (cuatrimestreId.HasValue)
            {
                var cuatrimestre = await _db.Cuatrimestre.FindAsync(cuatrimestreId.Value);
                estadisticas.NombreCuatrimestre = cuatrimestre?.Nombre;
            }

            if (cursoId.HasValue)
            {
                var curso = await _db.Curso.FindAsync(cursoId.Value);
                estadisticas.NombreCurso = curso?.Nom_Curso;
                estadisticas.CodigoCurso = curso?.Codigo;
            }

            // Query base
            IQueryable<EstudianteCurso> query = _db.EstudianteCurso
                .Include(ec => ec.Estudiante)
                .Include(ec => ec.CursoCuatrimestre.Curso)
                .Include(ec => ec.CursoCuatrimestre.Cuatrimestre)
                .Where(ec => ec.Ind_Estado == "A");

            // Aplicar filtros
            if (cuatrimestreId.HasValue)
            {
                query = query.Where(ec => ec.CursoCuatrimestre.CuatrimestreId == cuatrimestreId.Value);
            }

            if (cursoId.HasValue)
            {
                query = query.Where(ec => ec.CursoCuatrimestre.CursoId == cursoId.Value);
            }

            var estudiantesCursos = await query.ToListAsync();

            // Calcular estadísticas generales
            estadisticas.Generales.TotalMatriculados = estudiantesCursos.Count;
            estadisticas.Generales.TotalEstudiantes = estudiantesCursos
                .Select(ec => ec.EstudianteId)
                .Distinct()
                .Count();

            // Obtener evaluaciones
            var evaluaciones = await _db.Evaluacion
                .Where(e => _db.EstudianteCurso
                    .Where(ec => ec.Ind_Estado == "A" &&
                                 (!cuatrimestreId.HasValue || ec.CursoCuatrimestre.CuatrimestreId == cuatrimestreId.Value) &&
                                 (!cursoId.HasValue || ec.CursoCuatrimestre.CursoId == cursoId.Value))
                    .Select(ec => ec.EstudianteCursoId)
                    .Contains(e.EstudianteCursoId))
                .ToListAsync();

            estadisticas.Generales.TotalEvaluaciones = evaluaciones.Count;

            // Porcentaje de participación
            if (estadisticas.Generales.TotalMatriculados > 0)
            {
                estadisticas.Generales.PorcentajeParticipacion =
                    (decimal)estadisticas.Generales.TotalEvaluaciones / estadisticas.Generales.TotalMatriculados * 100;
            }

            // Estadísticas de aprobación
            estadisticas.Generales.EstudiantesAprobados = evaluaciones.Count(e => e.Estado == "Aprobado");
            estadisticas.Generales.EstudiantesReprobados = evaluaciones.Count(e => e.Estado == "Reprobado");
            estadisticas.Generales.EstudiantesEnProceso = evaluaciones.Count(e => e.Estado == "En Proceso");

            if (estadisticas.Generales.TotalEvaluaciones > 0)
            {
                estadisticas.Generales.PorcentajeAprobacion =
                    (decimal)estadisticas.Generales.EstudiantesAprobados / estadisticas.Generales.TotalEvaluaciones * 100;
                estadisticas.Generales.PorcentajeReprobacion =
                    (decimal)estadisticas.Generales.EstudiantesReprobados / estadisticas.Generales.TotalEvaluaciones * 100;
                estadisticas.Generales.PorcentajeEnProceso =
                    (decimal)estadisticas.Generales.EstudiantesEnProceso / estadisticas.Generales.TotalEvaluaciones * 100;
            }

            // Promedios
            if (evaluaciones.Any())
            {
                estadisticas.Generales.PromedioGeneral = evaluaciones.Average(e => e.Nota);

                var aprobados = evaluaciones.Where(e => e.Estado == "Aprobado");
                if (aprobados.Any())
                {
                    estadisticas.Generales.PromedioAprobados = aprobados.Average(e => e.Nota);
                }

                var reprobados = evaluaciones.Where(e => e.Estado == "Reprobado");
                if (reprobados.Any())
                {
                    estadisticas.Generales.PromedioReprobados = reprobados.Average(e => e.Nota);
                }
            }

            // Estadísticas de participación
            estadisticas.Generales.ParticipacionExcelente = evaluaciones.Count(e => e.TipoParticipacion == "Excelente");
            estadisticas.Generales.ParticipacionBuena = evaluaciones.Count(e => e.TipoParticipacion == "Buena");
            estadisticas.Generales.ParticipacionRegular = evaluaciones.Count(e => e.TipoParticipacion == "Regular");
            estadisticas.Generales.ParticipacionBaja = evaluaciones.Count(e => e.TipoParticipacion == "Baja");
            estadisticas.Generales.ParticipacionNinguna = evaluaciones.Count(e => e.TipoParticipacion == "Ninguna");

            // Preparar datos para gráficos
            PrepararDatosGraficos(estadisticas, evaluaciones);

            // Lista de estudiantes
            estadisticas.Estudiantes = await ObtenerListaEstudiantesAsync(estudiantesCursos, evaluaciones);

            return estadisticas;
        }

        // =============================================
        // PREPARAR DATOS PARA GRÁFICOS
        // =============================================
        private void PrepararDatosGraficos(EstadisticasViewModel estadisticas, List<Evaluacion> evaluaciones)
        {
            // Gráfico de estados
            estadisticas.Graficos.EstadosLabels = new List<string> { "Aprobados", "Reprobados", "En Proceso" };
            estadisticas.Graficos.EstadosData = new List<int>
            {
                estadisticas.Generales.EstudiantesAprobados,
                estadisticas.Generales.EstudiantesReprobados,
                estadisticas.Generales.EstudiantesEnProceso
            };
            estadisticas.Graficos.EstadosColors = new List<string> { "#28a745", "#dc3545", "#ffc107" };

            // Gráfico de participación
            estadisticas.Graficos.ParticipacionLabels = new List<string>
            {
                "Excelente", "Buena", "Regular", "Baja", "Ninguna"
            };
            estadisticas.Graficos.ParticipacionData = new List<int>
            {
                estadisticas.Generales.ParticipacionExcelente,
                estadisticas.Generales.ParticipacionBuena,
                estadisticas.Generales.ParticipacionRegular,
                estadisticas.Generales.ParticipacionBaja,
                estadisticas.Generales.ParticipacionNinguna
            };
            estadisticas.Graficos.ParticipacionColors = new List<string>
            {
                "#28a745", "#17a2b8", "#ffc107", "#fd7e14", "#6c757d"
            };

            // Distribución de notas
            estadisticas.Graficos.NotasRangos = new List<string>
            {
                "0-59", "60-69", "70-79", "80-89", "90-100"
            };
            estadisticas.Graficos.NotasDistribucion = new List<int>
            {
                evaluaciones.Count(e => e.Nota < 60),
                evaluaciones.Count(e => e.Nota >= 60 && e.Nota < 70),
                evaluaciones.Count(e => e.Nota >= 70 && e.Nota < 80),
                evaluaciones.Count(e => e.Nota >= 80 && e.Nota < 90),
                evaluaciones.Count(e => e.Nota >= 90)
            };
        }

        // =============================================
        // OBTENER LISTA DE ESTUDIANTES
        // =============================================
        private async Task<List<EstudianteEstadisticaViewModel>> ObtenerListaEstudiantesAsync(
            List<EstudianteCurso> estudiantesCursos, List<Evaluacion> evaluaciones)
        {
            var resultado = new List<EstudianteEstadisticaViewModel>();

            foreach (var ec in estudiantesCursos)
            {
                var evaluacion = evaluaciones.FirstOrDefault(e => e.EstudianteCursoId == ec.EstudianteCursoId);

                resultado.Add(new EstudianteEstadisticaViewModel
                {
                    EstudianteID = ec.Estudiante.EstudianteId,
                    Identificacion = ec.Estudiante.Identificacion,
                    NombreCompleto = $"{ec.Estudiante.Nombre} {ec.Estudiante.Apellidos}",
                    Email = ec.Estudiante.Email,
                    TieneEvaluacion = evaluacion != null,
                    Nota = evaluacion?.Nota,
                    Estado = evaluacion?.Estado,
                    TipoParticipacion = evaluacion?.TipoParticipacion,
                    Observaciones = evaluacion?.Observaciones,
                    FechaEvaluacion = evaluacion?.Fec_Evaluacion
                });
            }

            return resultado.OrderBy(e => e.NombreCompleto).ToList();
        }

        // =============================================
        // OBTENER COMPARATIVA DE CURSOS
        // =============================================
        public async Task<ComparativaCursosViewModel> ObtenerComparativaCursosAsync(int cuatrimestreId)
        {
            var cursos = await ObtenerCursosPorCuatrimestreAsync(cuatrimestreId);
            var comparativa = new ComparativaCursosViewModel
            {
                Cursos = new List<CursoComparativoViewModel>()
            };

            foreach (var curso in cursos)
            {
                var stats = await ObtenerEstadisticasAsync(cuatrimestreId, curso.CursoID);

                comparativa.Cursos.Add(new CursoComparativoViewModel
                {
                    CodigoCurso = curso.Codigo,
                    NombreCurso = curso.Nombre,
                    TotalEstudiantes = curso.TotalEstudiantes,
                    PromedioNota = stats.Generales.PromedioGeneral,
                    PorcentajeAprobacion = stats.Generales.PorcentajeAprobacion,
                    Aprobados = stats.Generales.EstudiantesAprobados,
                    Reprobados = stats.Generales.EstudiantesReprobados
                });
            }

            return comparativa;
        }
    }
}