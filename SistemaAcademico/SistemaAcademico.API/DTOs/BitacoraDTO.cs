namespace SistemaAcademico.API.DTOs
{
    public class BitacoraBaseDTO
    {
        public int? BitacoraId { get; set; }
        public string? UserId { get; set; } 
        public string? Accion { get; set; } = string.Empty;
        public string? Modulo { get; set; }
        public string? Descripcion { get; set; }
        public string? DireccionIP { get; set; }
        public DateTime? Fec_Registro { get; set; }
    }
    public class BitacoraDto
    {
        public int BitacoraId { get; set; }
        public string UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public string Accion { get; set; }
        public string Modulo { get; set; }
        public string Descripcion { get; set; }
        public string DireccionIP { get; set; }
        public DateTime Fec_Registro { get; set; }
    }

    public class FiltrosBitacoraDto
    {
        public string? UsuarioId { get; set; }
        public string Accion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 50;
    }

    public class BitacoraPaginadaDto
    {
        public List<BitacoraDto> Registros { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    } 
}
