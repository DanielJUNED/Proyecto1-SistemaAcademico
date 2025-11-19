using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademico.Data.Entities
{

    [Table("Docente")]
    public partial class Docente
    {
        public Docente()
        {
            Evaluacion = new HashSet<Evaluacion>();
            CursoCuatrimestreDocente = new HashSet<CursoCuatrimestreDocente>();
        }
        [Key]
        public int DocenteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual ICollection<Evaluacion> Evaluacion { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<CursoCuatrimestreDocente> CursoCuatrimestreDocente { get; set;  }
    }
}
