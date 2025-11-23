using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico.API.DTOs;
using SistemaAcademico.Data.Entities;   
using SistemaAcademico.Data.Repositories;
using System.Configuration;

namespace SistemaAcademico.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CursosController : ControllerBase
    {
        private readonly CursoDB _cursodb;
        private readonly BitacoraDB _bitacoradb; 

        public CursosController(CursoDB cursodb,BitacoraDB bitacoradb)
        {
            _cursodb = cursodb;
             
            _bitacoradb = bitacoradb;
        }

        // GET: api/cursos
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            try
            {
                var cursos = await _cursodb.ObtenerTodos();
                // MODEL → DTO
                var cursosDTO = cursos.Select(c => new CursoDTO
                {
                    CursoId = c.CursoId,
                    Codigo = c.Codigo,
                    Nom_Curso = c.Nom_Curso,
                    Desc_Curso = c.Desc_Curso,
                    Num_Creditos = c.Num_Creditos
                }).ToList();
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener cursos" });
            }
        }
        // GET: api/cursos
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurso(int id)
        {
            try
            {
                var curso = await _cursodb.ObtenerPorId(id);

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
                return StatusCode(500, new { error = "Error al obtener curso", detalle = ex.Message });
            }
        }


        // POST: api/cursos
        [HttpPost] 
        public async Task<IActionResult> Crear([FromBody] CrearCursoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // DTO → MODEL
                var curso = new Curso
                {
                    Codigo = dto.Codigo,
                    Nom_Curso = dto.Nom_Curso,
                    Desc_Curso = dto.Desc_Curso,
                    Num_Creditos = dto.Num_Creditos
                };

                bool resultado = await _cursodb.Crear(curso);

                if (resultado)
                {
                    // OBTENER ID DEL USUARIO LOGUEADO
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                    // Registrar en bitácora
                    await _bitacoradb.Registrar(new Bitacora
                    {
                        UserId = userId,// "e64be105-16a8-4830-b6c7-0a71df6f0ef7", // Obtener del contexto
                        Accion = "Crear Curso",
                        Modulo = "Cursos",
                        Descripcion = $"Curso creado: {curso.Codigo} - {curso.Nom_Curso}",
                        DireccionIP = GetClientIP()
                    });

                    return Ok(new { success = true, message = "Curso creado correctamente" });
                }

                return BadRequest("No se pudo crear el curso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/cursos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CursoDTO dto)

        {
            try
            {
                if (id != dto.CursoId)
                    return BadRequest(new { message = "ID no coincide" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // DTO → MODEL
                var curso = new Curso
                {
                    CursoId = dto.CursoId,
                    Codigo = dto.Codigo,
                    Nom_Curso = dto.Nom_Curso,
                    Desc_Curso = dto.Desc_Curso,
                    Num_Creditos = dto.Num_Creditos
                };

                curso.CursoId = id;
                bool resultado = await _cursodb.Actualizar(curso);

                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el curso");
                }
                // Registrar en bitácora
                /*_bitacoradb.Registrar(new Bitacora
                {
                    UserId = User.Identity.Name,
                    Accion = "Actualizar Curso",
                    Modulo = "Cursos",
                    Descripcion = $"Curso actualizado: {curso.Codigo}",
                    DireccionIP = GetClientIP()
                });*/

                return Ok(new { success = true, message = "Curso actualizado correctamente" });
                 }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/cursos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                bool resultado = await _cursodb.Eliminar(id);

                if (!resultado)
                {
                    return BadRequest("No se pudo eliminar el curso. Puede tener estudiantes inscritos.");
                }
                // Registrar en bitácora
                /*_bitacoradb.Registrar(new Bitacora
                {
                    UserId = User.Identity.Name,
                    Accion = "Eliminar Curso",
                    Modulo = "Cursos",
                    Descripcion = $"Curso eliminado: ID {id}",
                    DireccionIP = GetClientIP()
                });*/

                return Ok(new { success = true, message = "Curso eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /*// POST: api/cursos/asignardocente
        [HttpPost]
        [Route("api/cursos/asignardocente")]
        public async Task<IActionResult> AsignarDocente([FromBody] dynamic datos)
        {
            try
            {
                int cursoId = datos.cursoId;
                int cuatrimestreId = datos.cuatrimestreId;
                int docenteId = datos.docenteId;

                bool resultado = await _cursodb.AsignarDocente(cursoId, cuatrimestreId, docenteId);

                if (resultado)
                {
                    return BadRequest("No se pudo asignar el docente");
                }

               /* _bitacoradb.Registrar(new Bitacora
                {
                    UserId = User.Identity.Name,
                    Accion = "Asignar Docente",
                    Modulo = "Cursos",
                    Descripcion = $"Docente {docenteId} asignado al curso {cursoId}",
                    DireccionIP = GetClientIP()
                }); 

                return Ok(new { success = true, message = "Docente asignado correctamente" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }*/

        private string GetClientIP()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Si está detrás de un proxy o balanceador, podrías revisar el encabezado "X-Forwarded-For"
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedIp))
                    ip = forwardedIp;
            }

            return ip ?? "IP desconocida";
        }
    }
}
