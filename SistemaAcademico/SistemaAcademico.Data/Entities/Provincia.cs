using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademico.Data.Entities
{

    [Table("Provincia")]
    public partial class Provincia
    {
        public Provincia()
        {
            Canton = new HashSet<Canton>();
        }
        [Key]
        public int ProvinciaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom_Provincia { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual ICollection<Canton> Canton { get; set; }
    }
}
