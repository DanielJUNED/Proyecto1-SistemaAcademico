using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Entities
{
    [Table("CursoCuatrimestreDocente")]
    public class CursoCuatrimestreDocente
    {
        [Key]
        public int CursoCuatriDocenteId { get; set; } 

        [Required]
        public int CursoCuatrimestreId { get; set; }

        [Required]
        public int DocenteId { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        // Navegación
        public Docente Docente { get; set; }

        public CursoCuatrimestre CursoCuatrimestre { get; set; }
    }

}
