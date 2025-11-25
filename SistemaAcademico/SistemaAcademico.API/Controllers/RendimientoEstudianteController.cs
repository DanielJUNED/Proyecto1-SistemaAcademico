using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico.API.DTOs;
using SistemaAcademico.Data.Entities;
using SistemaAcademico.Data.Repositories;

namespace SistemaAcademico.API.Controllers
{
    public class RendimientoEstudianteController : ControllerBase
    {
        private readonly RendimientoEstudianteDB _remdimientoEstdb;
        private readonly BitacoraDB _bitacoradb;


        public RendimientoEstudianteController(RendimientoEstudianteDB remdimientoEstdb)
        { 
            _remdimientoEstdb = remdimientoEstdb;
        }

        // GET: api/rendimiento/mi-rendimiento
        /// <summary>
        /// Obtiene el rendimiento completo del estudiante autenticado
        /// </summary>
        [HttpGet]
        [Route("mi-rendimiento")] 
        public IActionResult GetMiRendimiento()
        {
            try
            {
                // Obtener UserId del usuario autenticado
                var userId = User.Identity.Name;

                // Obtener EstudianteId asociado
                var estudianteId = _remdimientoEstdb.ObtenerEstudianteIdPorUserId(userId);

                if (!estudianteId.HasValue)
                {
                    return NotFound();
                }

                // Obtener datos de la capa DATA
                var rendimientoEntity = _remdimientoEstdb.ObtenerRendimientoCompleto(estudianteId.Value);
                var rendimientoDTO = new RendimientoCompletoDTO();
                rendimientoDTO.Estudiante = new EstudianteDTO {
                    EstudianteId = rendimientoEntity.Estudiante.EstudianteId,
                    Identificacion = rendimientoEntity.Estudiante.Identificacion,
                    NombreCompleto = rendimientoEntity.Estudiante.NombreCompleto,
                    Email = rendimientoEntity.Estudiante.Email,
                };
                rendimientoDTO.NotasPorCurso = rendimientoEntity.NotasCursos?.Select(nc => new NotaCursoDTO {
                    CursoId = nc.CursoId,
                    CodigoCurso = nc.CodigoCurso,
                    NombreCurso = nc.NombreCurso,
                    CuatrimestreId = nc.CuatrimestreId,
                    NombreCuatrimestre = nc.NombreCuatrimestre,
                    Nota = nc.Nota,
                    Estado = nc.Estado,
                    FechaEvaluacion = nc.FechaEvaluacion,
                    TipoParticipacion = nc.TipoParticipacion
                }).ToList() ?? new List<NotaCursoDTO>();
                rendimientoDTO.NotasPorCuatrimestre = rendimientoEntity.RendimientoPorCuatrimestre?.Select(rc => new RendimientoCuatrimestreDTO {
                    CuatrimestreId = rc.CuatrimestreId,
                    NombreCuatrimestre = rc.NombreCuatrimestre,
                    Anio = rc.Anio,
                    Numero = rc.Numero,
                    PromedioNotas = rc.PromedioNotas,
                    CursosAprobados = rc.CursosAprobados,
                    CursosReprobados = rc.CursosReprobados,
                    TotalCursos = rc.TotalCursos
                }).ToList() ?? new List<RendimientoCuatrimestreDTO>();
                rendimientoDTO.EstadisticasGenerales = new EstadisticasDTO {
                    PromedioGeneral = rendimientoEntity.Estadisticas.PromedioGeneral,
                    TotalCursosAprobados = rendimientoEntity.Estadisticas.TotalCursosAprobados,
                    TotalCursosReprobados = rendimientoEntity.Estadisticas.TotalCursosReprobados,
                    TotalCursosCursados = rendimientoEntity.Estadisticas.TotalCursosCursados,
                    PorcentajeAprobacion = rendimientoEntity.Estadisticas.PorcentajeAprobacion,
                    NotaMasAlta = rendimientoEntity.Estadisticas.NotaMasAlta,
                    NotaMasBaja = rendimientoEntity.Estadisticas.NotaMasBaja,
                    CursoMejorNota = rendimientoEntity.Estadisticas.CursoMejorNota,
                    CursoPeorNota = rendimientoEntity.Estadisticas.CursoPeorNota
                };  
                // Convertir a DTO
                //var rendimientoDTO = RendimientoMapper.ToDTO(rendimientoEntity);

                return Ok(rendimientoDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error mi rendimiento: ", error = ex.Message }); 
            }
        }

        // POST: api/rendimiento/mi-rendimiento-filtrado
        /// <summary>
        /// Obtiene el rendimiento con filtros aplicados
        /// </summary>
        [HttpPost]
        [Route("mi-rendimiento-filtrado")] 
        public IActionResult GetMiRendimientoFiltrado([FromBody] FiltrosRendimientoDTO filtrosDTO)
        {
            try
            {
                var userId = User.Identity.Name;
                var estudianteId = _remdimientoEstdb.ObtenerEstudianteIdPorUserId(userId);

                if (!estudianteId.HasValue)
                {
                    return NotFound();
                }

                // Convertir DTO a Entity de filtros
                var filtrosEntity = new FiltrosRendimiento { 
                    EstudianteId = estudianteId.Value,
                    CuatrimestresIds = filtrosDTO.CuatrimestresIds ?? new List<int>(),
                    CursosIds = filtrosDTO.CursosIds ?? new List<int>(),
                    FechaDesde = filtrosDTO.FechaDesde,
                    FechaHasta = filtrosDTO.FechaHasta
                };

               // var filtrosEntity = RendimientoMapper.ToEntity(filtrosDTO, estudianteId.Value);

                // Obtener datos filtrados
                var rendimientoEntity = _remdimientoEstdb.ObtenerRendimientoCompleto(estudianteId.Value, filtrosEntity);

                // Convertir a DTO
                //var rendimientoDTO = RendimientoMapper.ToDTO(rendimientoEntity);
                var rendimientoDTO = new RendimientoCompletoDTO();
                rendimientoDTO.Estudiante = new EstudianteDTO
                {
                    EstudianteId = rendimientoEntity.Estudiante.EstudianteId,
                    Identificacion = rendimientoEntity.Estudiante.Identificacion,
                    NombreCompleto = rendimientoEntity.Estudiante.NombreCompleto,
                    Email = rendimientoEntity.Estudiante.Email,
                };
                rendimientoDTO.NotasPorCurso = rendimientoEntity.NotasCursos?.Select(nc => new NotaCursoDTO
                {
                    CursoId = nc.CursoId,
                    CodigoCurso = nc.CodigoCurso,
                    NombreCurso = nc.NombreCurso,
                    CuatrimestreId = nc.CuatrimestreId,
                    NombreCuatrimestre = nc.NombreCuatrimestre,
                    Nota = nc.Nota,
                    Estado = nc.Estado,
                    FechaEvaluacion = nc.FechaEvaluacion,
                    TipoParticipacion = nc.TipoParticipacion
                }).ToList() ?? new List<NotaCursoDTO>();
                rendimientoDTO.NotasPorCuatrimestre = rendimientoEntity.RendimientoPorCuatrimestre?.Select(rc => new RendimientoCuatrimestreDTO
                {
                    CuatrimestreId = rc.CuatrimestreId,
                    NombreCuatrimestre = rc.NombreCuatrimestre,
                    Anio = rc.Anio,
                    Numero = rc.Numero,
                    PromedioNotas = rc.PromedioNotas,
                    CursosAprobados = rc.CursosAprobados,
                    CursosReprobados = rc.CursosReprobados,
                    TotalCursos = rc.TotalCursos
                }).ToList() ?? new List<RendimientoCuatrimestreDTO>();
                rendimientoDTO.EstadisticasGenerales = new EstadisticasDTO
                {
                    PromedioGeneral = rendimientoEntity.Estadisticas.PromedioGeneral,
                    TotalCursosAprobados = rendimientoEntity.Estadisticas.TotalCursosAprobados,
                    TotalCursosReprobados = rendimientoEntity.Estadisticas.TotalCursosReprobados,
                    TotalCursosCursados = rendimientoEntity.Estadisticas.TotalCursosCursados,
                    PorcentajeAprobacion = rendimientoEntity.Estadisticas.PorcentajeAprobacion,
                    NotaMasAlta = rendimientoEntity.Estadisticas.NotaMasAlta,
                    NotaMasBaja = rendimientoEntity.Estadisticas.NotaMasBaja,
                    CursoMejorNota = rendimientoEntity.Estadisticas.CursoMejorNota,
                    CursoPeorNota = rendimientoEntity.Estadisticas.CursoPeorNota
                };

                return Ok(rendimientoDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener mi rendimiento filtrado", error = ex.Message });
            }
        }

        // GET: api/rendimiento/mis-cuatrimestres
        /// <summary>
        /// Obtiene la lista de cuatrimestres del estudiante para los filtros
        /// </summary>
        [HttpGet]
        [Route("mis-cuatrimestres")] 
        public IActionResult GetMisCuatrimestres()
        {
            try
            {
                var userId = User.Identity.Name;
                var estudianteId = _remdimientoEstdb.ObtenerEstudianteIdPorUserId(userId);

                if (!estudianteId.HasValue)
                {
                    return NotFound();
                }

                // Obtener cuatrimestres
                var cuatrimestresEntity = _remdimientoEstdb.ObtenerCuatrimestresEstudiante(estudianteId.Value);

                // Convertir a DTOs
                var cuatrimestresDTO = cuatrimestresEntity.ToList();

                return Ok(cuatrimestresDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error al obtener mis cuatrimestres: ", error = ex.Message });
            }
        }

        // GET: api/rendimiento/mis-cursos
        /// <summary>
        /// Obtiene la lista de cursos del estudiante para los filtros
        /// </summary>
        [HttpGet]
        [Route("mis-cursos")] 
        public IActionResult GetMisCursos()
        {
            try
            {
                var userId = User.Identity.Name;
                var estudianteId = _remdimientoEstdb.ObtenerEstudianteIdPorUserId(userId);

                if (!estudianteId.HasValue)
                {
                    return NotFound();
                }

                // Obtener cursos
                var cursosEntity = _remdimientoEstdb.ObtenerCursosEstudiante(estudianteId.Value);

                // Convertir a DTOs
                var cursosDTO = cursosEntity
                    .Select(c => new CursoDTO
                    {
                        CursoId = c.CursoId,
                        Codigo = c.Codigo,
                        Nom_Curso =c.Nom_Curso
                    })
                    .ToList();

                return Ok(cursosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener mis cursos: ", error = ex.Message }); 
            }
        }

        // GET: api/rendimiento/verificar-actualizacion
        /// <summary>
        /// Verifica si hay nuevas evaluaciones desde la última consulta
        /// </summary>
        [HttpGet]
        [Route("verificar-actualizacion")] 
        public IActionResult VerificarActualizacion(DateTime? ultimaConsulta = null)
        {
            try
            {
                var userId = User.Identity.Name;
                var estudianteId = _remdimientoEstdb.ObtenerEstudianteIdPorUserId(userId);

                if (!estudianteId.HasValue)
                {
                    return NotFound();
                }

                // Si no se proporciona fecha, usar hace 5 minutos
                if (!ultimaConsulta.HasValue)
                {
                    ultimaConsulta = DateTime.Now.AddMinutes(-5);
                }

                // Verificar si hay actualizaciones
                bool hayActualizaciones = _remdimientoEstdb.VerificarNuevasEvaluaciones(
                    estudianteId.Value,
                    ultimaConsulta.Value);

                var actualizacionDTO = new ActualizacionDTO
                {
                    HayActualizaciones = hayActualizaciones,
                    FechaConsulta = DateTime.Now,
                    NumeroNuevasEvaluaciones = 0 // Opcional: implementar conteo
                };

                return Ok(actualizacionDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error en la veridicacion de actulización: ", error = ex.Message });
            }
        }
    }
}
