namespace SistemaAcademico._Web.Models.ViewModels
{
    public class BitacoraIndexViewModel
    {
        // Filtros de búsqueda
        public string NombreUsuario { get; set; }
        public string Accion { get; set; }
        public string Modulo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 50;
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }

        // Ordenamiento
        public string OrdenarPor { get; set; } = "Fec_Registro";
        public string Direccion { get; set; } = "desc"; // asc o desc

        // Datos
        public List<BitacoraListViewModel> Registros { get; set; } = new List<BitacoraListViewModel>();

        // Listas para filtros
        public List<AccionViewModel> Acciones { get; set; } = new List<AccionViewModel>();
        public List<ModuloViewModel> Modulos { get; set; } = new List<ModuloViewModel>();
    }

    public class BitacoraListViewModel
    {
        public int BitacoraId { get; set; }
        public string UserId { get; set; }
        public string NombreUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public string Accion { get; set; }
        public string Modulo { get; set; }
        public string Descripcion { get; set; }
        public string DireccionIP { get; set; }
        public DateTime Fec_Registro { get; set; }
    }

    public class AccionViewModel
    {
        public string Valor { get; set; }
        public string Texto { get; set; }
    }

    public class ModuloViewModel
    {
        public string Valor { get; set; }
        public string Texto { get; set; }
    }
}
