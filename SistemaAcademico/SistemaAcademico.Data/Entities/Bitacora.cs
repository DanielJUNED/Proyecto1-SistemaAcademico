using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Entities
{
    public class Bitacora
    {
        public int BitacoraId { get; set; }
        public string UserId { get; set; }
        public string Accion { get; set; }
        public string Modulo { get; set; }
        public string Descripcion { get; set; }
        public string DireccionIP { get; set; }
        public DateTime Fec_Registro { get; set; }
        // Propiedades de navegación para consultas
        public string NombreUsuario { get; set; }
        public string EmailUsuario { get; set; }
    }
}
