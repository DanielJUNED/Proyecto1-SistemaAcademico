using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademico.Data.Entities
{

    [Table("EstudianteCurso")]
    public partial class EstudianteCurso
    { 
        public EstudianteCurso()
        {
            Evaluacion = new HashSet<Evaluacion>();
        }
        [Key]
        public int EstudianteCursoId { get; set; }
        [Required]
        public int EstudianteId { get; set; }
        [Required]
        public int CursoCuatrimestreId { get; set; }

        public DateTime? Fec_Matricula { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual CursoCuatrimestre CursoCuatrimestre { get; set; }

        public virtual Estudiante Estudiante { get; set; }
         
        public virtual ICollection<Evaluacion> Evaluacion { get; set; }
    }
}
