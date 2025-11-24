// Api/Controllers/CursoCuatrimestreApiController.cs
using SistemaAcademico.API.DTOs;
using SistemaAcademico.Data.Entities;
using SistemaAcademico.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico.Data.Constantes;

namespace SistemaAcademico.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursoCuatrimestreController : ControllerBase
    {
        private readonly CursoDB _cursodb;
        private readonly CursoCuatrimestreDB _cursoCuatrimestredb;
        private readonly CursoCuatrimestreDocenteDB _cursoCuatrDocentedb;
        private readonly BitacoraDB _bitacoradb;

        public CursoCuatrimestreController(CursoDB cursodb,CursoCuatrimestreDB cursoCuatrimestredb, CursoCuatrimestreDocenteDB cursoCuatrDocentedb, BitacoraDB bitacoradb)
        {
            _cursodb = cursodb;
            _cursoCuatrimestredb = cursoCuatrimestredb;
            _cursoCuatrDocentedb = cursoCuatrDocentedb;
            _bitacoradb = bitacoradb;
        } 
         
        // GET: api/CursoCuatrimestre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CursoCuatrimestreDTO>>> GetAll([FromQuery] int? cuatrimestreId = null)
        {
            try
            {
                var entities = await _cursoCuatrimestredb.ObtenerTodos(cuatrimestreId);
                var dtos = new List<CursoCuatrimestreDTO>();

                foreach (var entity in entities)
                {
                    var tieneEstudiantes  = await _cursoCuatrimestredb.TieneEstudiantes(entity.CursoCuatrimestreId);
                    var tieneEvaluaciones = await _cursoCuatrimestredb.TieneEvaluaciones(entity.CursoCuatrimestreId);
                    var docentes          = await _cursoCuatrimestredb.ObtenerDocentePorCurso(entity.CursoCuatrimestreId);
                    var estudiante        = await _cursoCuatrimestredb.ObtenerEstudientePorCurso(entity.CursoCuatrimestreId);

                    dtos.Add(new CursoCuatrimestreDTO
                    {
                        CursoCuatrimestreId = entity.CursoCuatrimestreId,
                        CursoId = entity.CursoId,
                        CuatrimestreId = entity.CuatrimestreId,
                        Ind_Estado = entity.Ind_Estado,
                        CodigoCurso = entity.Curso?.Codigo,
                        NombreCurso = entity.Curso?.Nom_Curso,
                        NombreCuatrimestre = entity.Cuatrimestre?.Nombre,
                        TotalDocentes = docentes.Count(),
                        TotalEstudiantes= estudiante.Count(),
                        TieneEvaluaciones = tieneEvaluaciones
                    });
                }

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los datos", error = ex.Message });
            }
        }

        // GET: api/CursoCuatrimestreApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoCuatrimestreDTO>> GetById(int id)
        {
            try
            {
                var entity = await _cursoCuatrimestredb.ObtenerPorId(id);
                if (entity == null)
                    return NotFound(new { message = "Curso-Cuatrimestre no encontrado" });

                var docentes = await _cursoCuatrimestredb.ObtenerDocentePorCurso(id);
                var tieneEstudiantes = await _cursoCuatrimestredb.TieneEstudiantes(id);
                var tieneEvaluaciones = await _cursoCuatrimestredb.TieneEvaluaciones(id);

                var dto = new CursoCuatrimestreDTO
                {
                    CursoCuatrimestreId = entity.CursoCuatrimestreId,
                    CursoId = entity.CursoId,
                    CuatrimestreId = entity.CuatrimestreId,
                    Ind_Estado = entity.Ind_Estado,
                    CodigoCurso = entity.Curso?.Codigo,
                    NombreCurso = entity.Curso?.Nom_Curso,
                    NombreCuatrimestre = entity.Cuatrimestre?.Nombre,
                    TotalDocentes = docentes.Count(),
                    TieneEvaluaciones = tieneEvaluaciones,
                    Docentes = docentes.Select(d => new DocenteAsignadoDTO
                    {
                        CursoCuatriDocenteId = d.CursoCuatriDocenteId,
                        DocenteId = d.DocenteId,
                        NombreCompleto = $"{d.Docente.Nombre} {d.Docente.Apellidos}",
                        Email = d.Docente.Email
                    }).ToList()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el dato", error = ex.Message });
            }
        }

        // POST: api/CursoCuatrimestreApi
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateCursoCuatrimestreDTO dto)
        {
            try
            {
                // Validar que no exista ya
                if (await _cursoCuatrimestredb.Existe(dto.CursoId, dto.CuatrimestreId))
                {
                    return BadRequest(new { message = "Este curso ya existe en el cuatrimestre seleccionado" });
                }

                var entity = new CursoCuatrimestre
                {
                    CursoId = dto.CursoId,
                    CuatrimestreId = dto.CuatrimestreId,
                    Ind_Estado = "A"
                };

                var id = await _cursoCuatrimestredb.CreateAsync(entity);

                 if (id != null)
                { 
                    var curso = await _cursodb.ObtenerPorId(dto.CursoId); 
                    var cuatrimestre = await _cursoCuatrimestredb.ObtenerCuatrimestrePorId(dto.CuatrimestreId);
                    // Registrar en bitácora
                    await _bitacoradb.Registrar(new Bitacora
                    {
                        UserId = dto.Bitacora.UserId,// "e64be105-16a8-4830-b6c7-0a71df6f0ef7", // Obtener del contexto
                        Accion = dto.Bitacora.Accion,
                        Modulo = dto.Bitacora.Modulo,
                        Descripcion = $"Se registro Curso {curso.Nom_Curso} en el cuatrimestre {cuatrimestre.Nombre}",
                        DireccionIP = dto.Bitacora.DireccionIP,
                    }); 
                }

                // Asignar docentes
                if (dto.DocenteIds != null && dto.DocenteIds.Any())
                {
                    var listDocentes = "";
                    foreach (var docenteId in dto.DocenteIds)
                    {
                        var seAsigno  = await _cursoCuatrimestredb.AsignarDocente(id, docenteId);
                        var docente   = await _cursoCuatrimestredb.ObtenerDocentePorId(docenteId);
                        listDocentes += listDocentes + ", Cod."+docenteId+" - " +docente.Nombre+" "+docente.Apellidos;
                    }
                    if (listDocentes != null)
                    { 
                        // Registrar en bitácora
                        await _bitacoradb.Registrar(new Bitacora
                        {
                            UserId = dto.Bitacora.UserId, // Obtener del contexto
                            Accion = AccionesBitacora.Crear,
                            Modulo = ModulosBitacora.CursoCuatrimestreDocente,
                            Descripcion = listDocentes,
                            DireccionIP = dto.Bitacora.DireccionIP,
                        });
                    }
                }

                return CreatedAtAction(nameof(GetById), new { id }, new { id, message = "Curso-Cuatrimestre creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro", error = ex.Message });
            }
        }

        // PUT: api/CursoCuatrimestreApi/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateCursoCuatrimestreDTO dto)
        {
            try
            {
                if (id != dto.CursoCuatrimestreId)
                    return BadRequest(new { message = "El ID no coincide" });

                var exists = await _cursoCuatrimestredb.ObtenerPorId(id);
                if (exists == null)
                    return NotFound(new { message = "Curso-Cuatrimestre no encontrado" });

                // Validar si tiene estudiantes o evaluaciones
                var tieneEstudiantes = await _cursoCuatrimestredb.TieneEstudiantes(id);
                var tieneEvaluaciones = await _cursoCuatrimestredb.TieneEvaluaciones(id);

                if (tieneEstudiantes || tieneEvaluaciones)
                {
                    return BadRequest(new { message = "No se puede editar. El curso tiene estudiantes matriculados o evaluaciones registradas." });
                }

                var entity = new CursoCuatrimestre
                {
                    CursoCuatrimestreId = dto.CursoCuatrimestreId,
                    CursoId = dto.CursoId,
                    CuatrimestreId = dto.CuatrimestreId
                };

                var success = await _cursoCuatrimestredb.Actualizar(entity);
                if (!success)
                    return StatusCode(500, new { message = "Error al actualizar" });

                return Ok(new { message = "Actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar", error = ex.Message });
            }
        }

        // DELETE: api/CursoCuatrimestreApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var exists = await _cursoCuatrimestredb.ObtenerPorId(id);
                if (exists == null)
                    return NotFound(new { message = "Curso-Cuatrimestre no encontrado" });

                // Validar si puede eliminarse
                var puedeEliminar = await _cursoCuatrimestredb.PuedeEliminar(id);
                if (!puedeEliminar)
                {
                    return BadRequest(new { message = "No se puede eliminar. El curso tiene docentes asignados, estudiantes matriculados o evaluaciones registradas." });
                }

                var success = await _cursoCuatrimestredb.Eliminar(id);
                if (!success)
                    return StatusCode(500, new { message = "Error al eliminar" });

                return Ok(new { message = "Eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar", error = ex.Message });
            }
        }

        // POST: api/CursoCuatrimestreApi/AsignarDocente
        [HttpPost("AsignarDocente")]
        public async Task<ActionResult> AsignarDocente([FromBody] AsignarDocenteGeneralDTO dto)
        {
            try
            {
                // Validar que el curso-cuatrimestre existe
                var cursoCuatrimestre = await _cursoCuatrimestredb.ObtenerPorId(dto.CursoCuatrimestreId);
                if (cursoCuatrimestre == null)
                    return NotFound(new { message = "Curso-Cuatrimestre no encontrado" });

                // Validar si tiene estudiantes o evaluaciones
                var tieneEstudiantes = await _cursoCuatrimestredb.TieneEstudiantes(dto.CursoCuatrimestreId);
                var tieneEvaluaciones = await _cursoCuatrimestredb.TieneEvaluaciones(dto.CursoCuatrimestreId);

                if (tieneEstudiantes || tieneEvaluaciones)
                {
                    return BadRequest(new { message = "No se puede modificar docentes. El curso tiene estudiantes matriculados o evaluaciones registradas." });
                }

                // Validar que el docente no esté ya asignado
                var yaAsignado = await _cursoCuatrimestredb.DocenteYaAsignado(dto.CursoCuatrimestreId, dto.DocenteId);
                if (yaAsignado)
                    return BadRequest(new { message = "El docente ya está asignado a este curso" });

                var success = await _cursoCuatrimestredb.AsignarDocente(dto.CursoCuatrimestreId, dto.DocenteId);
                if (!success)
                    return StatusCode(500, new { message = "Error al asignar docente" });

                return Ok(new { message = "Docente asignado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al asignar docente", error = ex.Message });
            }
        }

        // DELETE: api/CursoCuatrimestreApi/RemoverDocente/5
        [HttpDelete("RemoverDocente/{cursoCuatriDocenteId}")]
        public async Task<ActionResult> RemoverDocente(int cursoCuatriDocenteId)
        {
            try
            {
                // Obtener información del docente asignado para validaciones
                var cursoCuatriDocente = await _cursoCuatrDocentedb.ObtenerPorId(cursoCuatriDocenteId);
                var docentes = await _cursoCuatrimestredb.ObtenerDocentePorCurso(cursoCuatriDocente.CursoCuatrimestreId); // Necesitarías ajustar esto
                var docenteAsignado = docentes.FirstOrDefault(d => d.CursoCuatriDocenteId == cursoCuatriDocenteId);

                if (docenteAsignado == null)
                    return NotFound(new { message = "Asignación no encontrada" });

                // Validar si tiene estudiantes o evaluaciones
                var tieneEstudiantes = await _cursoCuatrimestredb.TieneEstudiantes(docenteAsignado.CursoCuatrimestreId);
                var tieneEvaluaciones = await _cursoCuatrimestredb.TieneEvaluaciones(docenteAsignado.CursoCuatrimestreId);

                if (tieneEstudiantes || tieneEvaluaciones)
                {
                    return BadRequest(new { message = "No se puede remover el docente. El curso tiene estudiantes matriculados o evaluaciones registradas." });
                }

                var success = await _cursoCuatrimestredb.RemoverDocente(cursoCuatriDocenteId);
                if (!success)
                    return StatusCode(500, new { message = "Error al remover docente" });

                return Ok(new { message = "Docente removido exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al remover docente", error = ex.Message });
            }
        }

        // GET: api/CursoCuatrimestreApi/Docentes/5
        [HttpGet("Docentes/{cursoCuatrimestreId}")]
        public async Task<ActionResult<IEnumerable<DocenteAsignadoDTO>>> GetDocentes(int cursoCuatrimestreId)
        {
            try
            {
                var docentes = await _cursoCuatrimestredb.ObtenerDocentePorCurso(cursoCuatrimestreId);
                var dtos = docentes.Select(d => new DocenteAsignadoDTO
                {
                    CursoCuatriDocenteId = d.CursoCuatriDocenteId,
                    DocenteId = d.DocenteId,
                    NombreCompleto = $"{d.Docente.Nombre} {d.Docente.Apellidos}",
                    Email = d.Docente.Email
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener docentes", error = ex.Message });
            }
        }

        // GET: api/CursoCuatrimestreApi/ValidarEdicion/5
        [HttpGet("ValidarEdicion/{id}")]
        public async Task<ActionResult> ValidarEdicion(int id)
        {
            try
            {
                var tieneEstudiantes = await _cursoCuatrimestredb.TieneEstudiantes(id);
                var tieneEvaluaciones = await _cursoCuatrimestredb.TieneEvaluaciones(id);

                return Ok(new
                {
                    puedeEditar = !tieneEstudiantes && !tieneEvaluaciones,
                    tieneEstudiantes,
                    tieneEvaluaciones
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al validar", error = ex.Message });
            }
        }
        //=======================================================
        // GET: api/catalogo/cuatrimestres
        [HttpGet("cuatrimestres")]
        public async Task<ActionResult> GetCuatrimestres()
        {
            try
            {
                var cuatrimestres = await _cursoCuatrimestredb.ObtenerCuatrimestresActivos();
                return Ok(cuatrimestres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener cuatrimestres", error = ex.Message });
            }
        }

        // GET: api/catalogo/cuatrimestres/5
        [HttpGet("cuatrimestres/{id}")]
        public async Task<ActionResult> GetCuatrimestreById(int id)
        {
            try
            {
                var cuatrimestre = await _cursoCuatrimestredb.ObtenerCuatrimestrePorId(id);

                if (cuatrimestre == null)
                    return NotFound(new { message = "Cuatrimestre no encontrado" });
                // MODEL → DTO
                var cuatrimestreDTO = new CuatrimestreDTO
                {
                    CuatrimestreId = cuatrimestre.CuatrimestreId,
                    Nombre = cuatrimestre.Nombre,
                    Anio = cuatrimestre.Anio,
                    Numero = cuatrimestre.Numero,
                    Fec_Inicio = cuatrimestre.Fec_Inicio,
                    Fec_Fin = cuatrimestre.Fec_Fin
                };
                return Ok(cuatrimestreDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener cuatrimestre", error = ex.Message });
            }
        }

        // GET: api/catalogo/cursos
        [HttpGet("cursos")]
        public async Task<ActionResult> GetCursos()
        {
            try
            {
                var cursos = await _cursoCuatrimestredb.ObtenerCursosActivos();
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener cursos", error = ex.Message });
            }
        }

        // GET: api/catalogo/cursos/5
        [HttpGet("cursos/{id}")]
        public async Task<ActionResult> GetCursoById(int id)
        {
            try
            {
                var curso = await _cursoCuatrimestredb.ObtenerCursoPorId(id);
                if (curso == null)
                    return NotFound(new { message = "Curso no encontrado" });
                // MODEL → DTO
                var cursoDTO = new CursoDTO
                {
                    CursoId = curso.CursoId,
                    Codigo = curso.Codigo,
                    Nom_Curso = curso.Nom_Curso,
                    Desc_Curso = curso.Desc_Curso,
                    Num_Creditos = curso.Num_Creditos
                };

                return Ok(cursoDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener curso", error = ex.Message });
            }
        }
        [HttpGet("CursoNoEnCuatrimestre/{id}")]
        public async Task<ActionResult> GetCursosNoEnCuatrimestre(int id)
        {
            try
            {
                var cursos = await _cursoCuatrimestredb.ObtenerCursoNoEnCuartrimestre(id);
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener cursos", error = ex.Message });
            }
        }
        // GET: api/catalogo/docentes
        [HttpGet("docentes")]
        public async Task<ActionResult> GetDocentes()
        {
            try
            {
                var docentes = await _cursoCuatrimestredb.ObtenerDocentesActivos();
                return Ok(docentes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener docentes", error = ex.Message });
            }
        }

        // GET: api/catalogo/docentes/5
        [HttpGet("docentes/{id}")]
        public async Task<ActionResult> GetDocenteById(int id)
        {
            try
            {
                var docente = await _cursoCuatrimestredb.ObtenerDocentePorId(id);
                if (docente == null)
                    return NotFound(new { message = "Docente no encontrado" });
                // MODEL → DTO
                var docenteDTO = new DocenteDTO
                {
                    DocenteId = docente.DocenteId,
                    Nombre = docente.Nombre,
                    Apellidos = docente.Apellidos,
                    Email = docente.Email
                };
                return Ok(docente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener docente", error = ex.Message });
            }
        }
    }
}