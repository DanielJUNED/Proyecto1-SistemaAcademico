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
            Fec_Registro = dto.Fec_Registro
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
}
