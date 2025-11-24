using Microsoft.AspNetCore.Mvc;
using SistemaAcademico.API.DTOs;
using SistemaAcademico.Data.Entities;
using SistemaAcademico.Data.Repositories;

[Route("api/[controller]")]
[ApiController]
public class BitacoraController : ControllerBase
{
    private readonly BitacoraDB _bitacoradb;

    public BitacoraController(BitacoraDB bitacoradb)
    {
        _bitacoradb = bitacoradb;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] BitacoraBaseDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entrada = new Bitacora
        {
            UserId = dto.UserId,
            Accion = dto.Accion,
            Modulo = dto.Modulo,
            DireccionIP = dto.DireccionIP,
            Descripcion = dto.Descripcion,
            Fec_Registro = dto.Fec_Registro ?? DateTime.UtcNow
        };

        try
        {
            await _bitacoradb.Registrar(entrada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error registrando la bitácora", detalle = ex.Message });
        }

        return Ok(new { mensaje = "Bitácora registrada correctamente" });
    }
    // GET: api/BitacoraApi?nombreUsuario=juan&accion=Crear&modulo=Curso&fechaInicio=2025-01-01
    [HttpGet]
    // [Authorize(Roles = "Administrador")] // TODO: Descomentar cuando tengas autenticación
    public async Task<ActionResult<BitacoraPaginadaDto>> GetAll(
        [FromQuery] string? nombreUsuario,
        [FromQuery] string? accion,
        [FromQuery] string? modulo,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] string ordenarPor = "Fec_Registro",
        [FromQuery] string direccion = "desc",
        [FromQuery] int pagina = 1,
        [FromQuery] int registrosPorPagina = 50)
    {
        try
        {

            var registros = await _bitacoradb.GetAllAsync(
                nombreUsuario,
                accion,
                modulo,
                fechaInicio,
                fechaFin,
                ordenarPor,
                direccion,
                pagina,
                registrosPorPagina
            );

            var total = await _bitacoradb.GetTotalRegistrosAsync(
                nombreUsuario,
                accion,
                modulo,
                fechaInicio,
                fechaFin
            );

            var result = new BitacoraPaginadaDto
            {
                Registros = registros.Select(b => new BitacoraDto
                {
                    BitacoraId = b.BitacoraId,
                    UsuarioId = b.UserId,
                    NombreUsuario = b.NombreUsuario,
                    EmailUsuario = b.EmailUsuario,
                    Accion = b.Accion,
                    Modulo = b.Modulo,
                    Descripcion = b.Descripcion,
                    DireccionIP = b.DireccionIP,
                    Fec_Registro = b.Fec_Registro
                }).ToList(),
                TotalRegistros = total,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(total / (double)registrosPorPagina)
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener bitácora", error = ex.Message });
        }
    }

    // GET: api/BitacoraApi/acciones
    [HttpGet("acciones")]
    public async Task<ActionResult<IEnumerable<string>>> GetAcciones()
    {
        try
        {
            var acciones = await _bitacoradb.GetAccionesAsync();
            return Ok(acciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener acciones", error = ex.Message });
        }
    }

    // GET: api/BitacoraApi/modulos
    [HttpGet("modulos")]
    public async Task<ActionResult<IEnumerable<string>>> GetModulos()
    {
        try
        {
            var modulos = await _bitacoradb.GetModulosAsync();
            return Ok(modulos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener módulos", error = ex.Message });
        }
    }
}
