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
        }
        [Key]
        public int CursoCuatrimestreId { get; set; }
        [Required]
        public int CursoId { get; set; }
        [Required]
        public int CuatrimestreId { get; set; }
        //[Required]
        public int DocenteId { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual Cuatrimestre Cuatrimestre { get; set; }

        public virtual Curso Curso { get; set; }

        public virtual Docente Docente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EstudianteCurso> EstudianteCurso { get; set; }
    }
}
