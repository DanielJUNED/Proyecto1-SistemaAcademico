namespace SistemaAcademico._Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Distrito")]
    public partial class Distrito
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Distrito()
        {
            Estudiante = new HashSet<Estudiante>();
        }

        public int DistritoId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre Distrito")]
        public string Nom_Distrito { get; set; }

        public int CantonId { get; set; }

        [Required]
        [StringLength(2)]

        [Display(Name = "Estado")]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual Canton Canton { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Estudiante> Estudiante { get; set; }
    }
}
