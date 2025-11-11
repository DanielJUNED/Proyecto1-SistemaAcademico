using System.Collections.Generic;

namespace SistemaAcademico.App_Start
{
    // Opciones para dropdowns
    public static class EvaluacionOptions
    {
        public static List<string> TiposParticipacion => new List<string>
        {
            "Excelente",
            "Buena",
            "Regular",
            "Baja",
            "Ninguna"
        };

        public static List<string> Estados => new List<string>
        {
            "Aprobado",
            "Reprobado",
            "En Proceso"
        };

        public static string ObtenerEstadoPorNota(decimal nota)
        {
            if (nota >= 70) return "Aprobado";
            if (nota >= 60) return "En Proceso";
            return "Reprobado";
        }

        public static string ObtenerColorEstado(string estado)
        {
            switch (estado)
            {
                case "Aprobado": return "success";
                case "Reprobado": return "danger";
                case "En Proceso": return "warning";
                default: return "secondary";
            }
        }
    }
}