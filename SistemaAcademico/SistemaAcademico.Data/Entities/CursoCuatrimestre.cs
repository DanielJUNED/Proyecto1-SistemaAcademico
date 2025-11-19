using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademico.Data.Entities
{
    [Table("CursoCuatrimestre")]
    public partial class CursoCuatrimestre
    { 
        public CursoCuatrimestre()
        {
            EstudianteCurso = new HashSet<EstudianteCurso>(); 
            CursoCuatrimestreDocente = new HashSet<CursoCuatrimestreDocente>();

        }
        [Key]
        public int CursoCuatrimestreId { get; set; }
        [Required]
        public int CursoId { get; set; }
        [Required]
        public int CuatrimestreId { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual Cuatrimestre Cuatrimestre { get; set; }

        public virtual Curso Curso { get; set; }
        public virtual ICollection<EstudianteCurso> EstudianteCurso { get; set; }
        public virtual ICollection<CursoCuatrimestreDocente> CursoCuatrimestreDocente { get; set; }

    }
}
