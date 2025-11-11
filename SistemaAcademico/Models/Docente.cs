namespace SistemaAcademico.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Docente")]
    public partial class Docente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Docente()
        {
            Evaluacion = new HashSet<Evaluacion>();
            CursoCuatrimestre = new HashSet<CursoCuatrimestre>();
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
        [ForeignKey("UserId")]
        public virtual ApplicationUser Usuario { get; set; }
        public virtual ICollection<CursoCuatrimestre> CursoCuatrimestre { get; set; }
    }
}
