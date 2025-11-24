using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Entities
{
    public class EstudianteInfo
    {
        public int EstudianteId { get; set; }
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellidos}";
        public DateTime Fec_Nacimiento { get; set; }
        public string Email { get; set; }
        public string Distrito { get; set; }
        public string Canton { get; set; }
        public string Provincia { get; set; }
    }

    public class HistorialAcademicoItem
    {
        public int EvaluacionId { get; set; }
        public int EstudianteCursoId { get; set; }
        public int CuatrimestreId { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int Anio { get; set; }
        public int NumeroCuatrimestre { get; set; }
        public DateTime Fec_InicioCuatrimestre { get; set; }
        public DateTime Fec_FinCuatrimestre { get; set; }
        public int CursoId { get; set; }
        public string CodigoCurso { get; set; }
        public string NombreCurso { get; set; }
        public int Num_Creditos { get; set; }
        public decimal Nota { get; set; }
        public string Estado { get; set; }
        public string TipoParticipacion { get; set; }
        public string Observaciones { get; set; }
        public DateTime Fec_Evaluacion { get; set; }
        public DateTime Fec_Matricula { get; set; }
        public int DocenteId { get; set; }
        public string NombreDocente { get; set; }
    }

    public class ResumenAcademico
    {
        public int TotalCursos { get; set; }
        public int CursosAprobados { get; set; }
        public int CursosReprobados { get; set; }
        public int CursosEnProceso { get; set; }
        public decimal PromedioGeneral { get; set; }
        public decimal NotaMasAlta { get; set; }
        public decimal NotaMasBaja { get; set; }
        public int TotalCreditos { get; set; }
        public int CreditosAprobados { get; set; }
    }

    public class HistorialPorCuatrimestre
    {
        public int CuatrimestreId { get; set; }
        public string NombreCuatrimestre { get; set; }
        public int Anio { get; set; }
        public int Numero { get; set; }
        public List<HistorialAcademicoItem> Cursos { get; set; } = new List<HistorialAcademicoItem>();
        public decimal PromedioDelCuatrimestre { get; set; }
        public int CursosAprobados { get; set; }
        public int CursosReprobados { get; set; }
    }
}
