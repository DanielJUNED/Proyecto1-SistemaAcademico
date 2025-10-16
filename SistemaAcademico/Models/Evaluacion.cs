namespace SistemaAcademico.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Evaluacion")]
    public partial class Evaluacion
    {
        public int EvaluacionId { get; set; }

        public int EstudianteCursoId { get; set; }

        public int DocenteId { get; set; }

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
