using SistemaAcademico.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SistemaAcademico.Data.Configurations
{
    public class BitacoraServicio
    {
        private readonly BitacoraDB _bitacoradb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BitacoraServicio(BitacoraDB bitacoradb, IHttpContextAccessor httpContextAccessor)
        {
            _bitacoradb = bitacoradb;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegistrarAsync(int usuarioId, string accion, string modulo, string descripcion)
        {
            try
            {
                var ip = ObtenerDireccionIP();
                //await _bitacoradb.Registrar(usuarioId, accion, modulo, descripcion, ip);
            }
            catch (Exception ex)
            {
                // Log error pero no afectar operación principal
                Console.WriteLine($"Error al registrar bitácora: {ex.Message}");
            }
        }

        private string ObtenerDireccionIP()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return "Unknown";

            // Intenta obtener la IP real si está detrás de un proxy
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
