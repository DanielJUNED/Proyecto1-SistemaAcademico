namespace SistemaAcademico.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EstudianteCurso")]
    public partial class EstudianteCurso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EstudianteCurso()
        {
            Evaluacion = new HashSet<Evaluacion>();
        }

        public int EstudianteCursoId { get; set; }

        public int EstudianteId { get; set; }

        public int CursoCuatrimestreId { get; set; }

        public DateTime? Fec_Matricula { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual CursoCuatrimestre CursoCuatrimestre { get; set; }

        public virtual Estudiante Estudiante { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evaluacion> Evaluacion { get; set; }
    }
}
