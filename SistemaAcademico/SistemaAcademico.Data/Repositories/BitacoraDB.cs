using Microsoft.Data.SqlClient;
using SistemaAcademico.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Repositories
{
    public class BitacoraDB
    {
        private readonly string _connectionString;

        public BitacoraDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task RegistrarAsync(int usuarioId, string accion, string modulo, string descripcion, string direccionIP)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Bitacora 
                    (UserId, Accion, Modulo, Descripcion, DireccionIP, Fec_Registro)
                    VALUES 
                    (@UsuarioId, @Accion, @Modulo, @Descripcion, @DireccionIP, GETDATE())";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@Accion", accion);
                    cmd.Parameters.AddWithValue("@Modulo", modulo);
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@DireccionIP", direccionIP ?? "Unknown");

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task Registrar(Bitacora bitacora)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Bitacora 
                    (UserId, Accion, Modulo, Descripcion, DireccionIP, Fec_Registro)
                    VALUES 
                    (@UserId, @Accion, @Modulo, @Descripcion, @DireccionIP, GETDATE())";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", bitacora.UserId);
                    cmd.Parameters.AddWithValue("@Accion", bitacora.Accion);
                    cmd.Parameters.AddWithValue("@Modulo", bitacora.Modulo);
                    cmd.Parameters.AddWithValue("@Descripcion", bitacora.Descripcion);
                    cmd.Parameters.AddWithValue("@DireccionIP", bitacora.DireccionIP ?? "Unknown");

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<IEnumerable<Bitacora>> GetAllAsync(
            int? usuarioId = null,
            string accion = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            int pagina = 1,
            int registrosPorPagina = 50)
        {
            var lista = new List<Bitacora>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = new StringBuilder(@"
                    SELECT * FROM vw_BitacoraConsulta 
                    WHERE 1=1");

                if (usuarioId.HasValue)
                    query.Append(" AND UsuarioId = @UsuarioId");
                if (!string.IsNullOrEmpty(accion))
                    query.Append(" AND Accion = @Accion");
                if (fechaInicio.HasValue)
                    query.Append(" AND Fec_Registro >= @FechaInicio");
                if (fechaFin.HasValue)
                    query.Append(" AND Fec_Registro <= @FechaFin");

                query.Append(" ORDER BY Fec_Registro DESC");
                query.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

                using (var cmd = new SqlCommand(query.ToString(), conn))
                {
                    if (usuarioId.HasValue)
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId.Value);
                    if (!string.IsNullOrEmpty(accion))
                        cmd.Parameters.AddWithValue("@Accion", accion);
                    if (fechaInicio.HasValue)
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);
                    if (fechaFin.HasValue)
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value.AddDays(1).AddSeconds(-1));

                    cmd.Parameters.AddWithValue("@Offset", (pagina - 1) * registrosPorPagina);
                    cmd.Parameters.AddWithValue("@PageSize", registrosPorPagina);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Bitacora
                            {
                                BitacoraId    = reader.GetInt32(0),
                                UserId        = reader.GetString(1),
                                NombreUsuario = reader.GetString(2),
                                EmailUsuario  = reader.GetString(3),
                                Accion        = reader.GetString(4),
                                Modulo        = reader.GetString(5),
                                Descripcion   = reader.GetString(6),
                                DireccionIP   = reader.GetString(7),
                                Fec_Registro  = reader.GetDateTime(8)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<int> GetTotalRegistrosAsync(
            int? usuarioId = null,
            string accion = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = new StringBuilder("SELECT COUNT(*) FROM Bitacora WHERE 1=1");

                if (usuarioId.HasValue)
                    query.Append(" AND UsuarioId = @UsuarioId");
                if (!string.IsNullOrEmpty(accion))
                    query.Append(" AND Accion = @Accion");
                if (fechaInicio.HasValue)
                    query.Append(" AND Fec_Registro >= @FechaInicio");
                if (fechaFin.HasValue)
                    query.Append(" AND Fec_Registro <= @FechaFin");

                using (var cmd = new SqlCommand(query.ToString(), conn))
                {
                    if (usuarioId.HasValue)
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId.Value);
                    if (!string.IsNullOrEmpty(accion))
                        cmd.Parameters.AddWithValue("@Accion", accion);
                    if (fechaInicio.HasValue)
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);
                    if (fechaFin.HasValue)
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value.AddDays(1).AddSeconds(-1));

                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            }
        }
    }
    /*public class BitacoraDB2
    {

        private readonly string _connectionString;
        BitacoraDB2(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task RegistrarAccionAsync(
            string accion,
            string modulo,
            string descripcion,
            int? usuarioId = null,
            int? ip=null)
        {
            try
            { 
                var bitacora = new Bitacora
                {
                    UserId = usuarioId ?? ObtenerUsuarioIdDeContexto(httpContext),
                    Accion = accion,
                    Modulo = modulo,
                    Descripcion = descripcion,
                    EntidadId = entidadId,
                    ValoresAnteriores = valoresAnteriores != null ? JsonSerializer.Serialize(valoresAnteriores) : null,
                    ValoresNuevos = valoresNuevos != null ? JsonSerializer.Serialize(valoresNuevos) : null,
                    DireccionIP = ip, 
                };

                await _bitacoraRepository.RegistrarAsync(bitacora);
            }
            catch (Exception ex)
            {
                // Log del error pero no fallar la operación principal
                Console.WriteLine($"Error al registrar bitácora: {ex.Message}");
            }
        }

        public async Task RegistrarCreacionAsync(string modulo, int entidadId, object valores, int? usuarioId = null)
        {
            var descripcion = $"Se creó un nuevo registro en {modulo}";
            await RegistrarAccionAsync(
                AccionesBitacora.Crear,
                modulo,
                descripcion,
                entidadId,
                null,
                valores,
                usuarioId
            );
        }

        public async Task RegistrarEdicionAsync(string modulo, int entidadId, object valoresAnteriores, object valoresNuevos, int? usuarioId = null)
        {
            var descripcion = $"Se editó el registro #{entidadId} en {modulo}";
            await RegistrarAccionAsync(
                AccionesBitacora.Editar,
                modulo,
                descripcion,
                entidadId,
                valoresAnteriores,
                valoresNuevos,
                usuarioId
            );
        }

        public async Task RegistrarEliminacionAsync(string modulo, int entidadId, object valores, int? usuarioId = null)
        {
            var descripcion = $"Se eliminó el registro #{entidadId} en {modulo}";
            await RegistrarAccionAsync(
                AccionesBitacora.Eliminar,
                modulo,
                descripcion,
                entidadId,
                valores,
                null,
                usuarioId
            );
        }

        public async Task RegistrarLoginAsync(int usuarioId, bool exitoso)
        {
            var accion = exitoso ? AccionesBitacora.Login : AccionesBitacora.LoginFallido;
            var descripcion = exitoso ? "Inicio de sesión exitoso" : "Intento de inicio de sesión fallido";

            await RegistrarAccionAsync(
                accion,
                ModulosBitacora.Autenticacion,
                descripcion,
                null,
                null,
                null,
                exitoso ? usuarioId : null
            );
        }

        public async Task RegistrarLogoutAsync(int usuarioId)
        {
            await RegistrarAccionAsync(
                AccionesBitacora.Logout,
                ModulosBitacora.Autenticacion,
                "Cierre de sesión",
                null,
                null,
                null,
                usuarioId
            );
        }

        private string ObtenerDireccionIP(HttpContext httpContext)
        {
            if (httpContext == null) return null;

            // Intenta obtener la IP real si está detrás de un proxy
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }

        private int? ObtenerUsuarioIdDeContexto(HttpContext httpContext)
        {
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst("UsuarioId")?.Value;
                if (int.TryParse(userIdClaim, out int usuarioId))
                {
                    return usuarioId;
                }
            }
            return null;
        }
    }*/
}
