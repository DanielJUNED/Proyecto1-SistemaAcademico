namespace SistemaAcademico._Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Evaluacion")]
    public partial class Evaluacion
    {
        [Key]
        public int EvaluacionId { get; set; }
        [Required]
        public int EstudianteCursoId { get; set; }
        [Required]
        public int DocenteId { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100")]

        public decimal Nota { get; set; }

        [StringLength(1000)]
        public string Observaciones { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoParticipacion { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        public DateTime? Fec_Evaluacion { get; set; }

        [Required]
        [StringLength(2)]
        public string Ind_Estado { get; set; }

        public DateTime Fec_Registro { get; set; }

        public virtual Docente Docente { get; set; }

        public virtual EstudianteCurso EstudianteCurso { get; set; }
    }
}
