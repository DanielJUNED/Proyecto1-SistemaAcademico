namespace SistemaAcademico.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Cuatrimestre")]
    public partial class Cuatrimestre
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cuatrimestre()
        {
            CursoCuatrimestre = new HashSet<CursoCuatrimestre>();
        }
        [Key]
        public int CuatrimestreId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        public int Anio { get; set; }
        [Required]
        [Range(1, 3)]

        public int Numero { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Fec_Inicio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fec_Fin { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CursoCuatrimestre> CursoCuatrimestre { get; set; }
    }
}
