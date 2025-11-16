using SistemaAcademico._Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaAcademico._Web.Repository
{
    public class EstadisticaDB
    {
        private readonly string _connectionString;

        public EstadisticaDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        /*
        private readonly string _connectionString;

        public EstadisticaDB()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public EstadisticaDB(string connectionString)
        {
            _connectionString = connectionString;
        }*/

        // =============================================
        // OBTENER CUATRIMESTRES DISPONIBLES
        // =============================================
        public async Task<List<CuatrimestreOpcionViewModel>> ObtenerCuatrimestresAsync(int? docenteId = null)
        {
            var resultado = new List<CuatrimestreOpcionViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT 
                        c.CuatrimestreId,
                        c.Nombre,
                        c.Anio,
                        c.Numero,
                        c.Ind_Estado
                    FROM Cuatrimestre c
                    WHERE c.Ind_Estado = 'A'
                    ORDER BY c.Anio DESC, c.Numero DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var cuatrimestre = new CuatrimestreOpcionViewModel
                            {
                                CuatrimestreID = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Anio = reader.GetInt32(2),
                                Numero = reader.GetInt32(3),
                                Ind_Estado = reader.GetString(4)
                            };
                            resultado.Add(cuatrimestre);
                        }
                    }
                }

                // Obtener totales de cursos y estudiantes para cada cuatrimestre
                foreach (var cuatrimestre in resultado)
                {
                    // Total de cursos 
                    var docentejoin = "";
                    var docenteParam = "";
                    string queryCursos = @"
                        SELECT COUNT(*)
                        FROM CursoCuatrimestre cc";

                    if (docenteId.HasValue) {
                        docentejoin += @" INNER JOIN CursoCuatrimestreDocente ccd ON cc.CursoCuatrimestreId = ccd.CursoCuatrimestreId";
                        docenteParam = @" AND ccd.DocenteId = @DocenteId";
                    }
                    queryCursos += docentejoin;
                    queryCursos += @" WHERE cc.CuatrimestreId = @CuatrimestreId 
                                      AND cc.Ind_Estado = 'A'" + docenteParam;

                    using (SqlCommand cmd = new SqlCommand(queryCursos, conn))
                    {
                        cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestre.CuatrimestreID);
                        if (docenteId.HasValue)
                            cmd.Parameters.AddWithValue("@DocenteId", docenteId.Value);

                        cuatrimestre.TotalCursos = (int)await cmd.ExecuteScalarAsync();
                    }

                    // Total de estudiantes
                    string queryEstudiantes = @"
                        SELECT COUNT(DISTINCT ec.EstudianteId)
                        FROM EstudianteCurso ec
                        INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId" +
                        docentejoin +
                        @" WHERE cc.CuatrimestreId = @CuatrimestreId 
                        AND ec.Ind_Estado = 'A'"+
                        docenteParam;
                     

                    using (SqlCommand cmd = new SqlCommand(queryEstudiantes, conn))
                    {
                        cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestre.CuatrimestreID);
                        if (docenteId.HasValue)
                            cmd.Parameters.AddWithValue("@DocenteId", docenteId.Value);

                        cuatrimestre.TotalEstudiantes = (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }

            return resultado;
        }

        // =============================================
        // OBTENER CURSOS POR CUATRIMESTRE
        // =============================================
        public async Task<List<CursoOpcionViewModel>> ObtenerCursosPorCuatrimestreAsync(int cuatrimestreId, int? docenteId = null)
        {
            var resultado = new List<CursoOpcionViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var docentejoin = "";
                var docenteParam = "";
                if (docenteId.HasValue)
                {
                    docentejoin += @" INNER JOIN CursoCuatrimestreDocente ccd ON cc.CursoCuatrimestreId = ccd.CursoCuatrimestreId";
                    docenteParam = @" AND ccd.DocenteId = @DocenteId";
                }
                string query = @"
                    SELECT DISTINCT
                        c.CursoId,
                        c.Codigo,
                        c.Nom_Curso,
                        c.Num_Creditos
                    FROM Curso c
                    INNER JOIN CursoCuatrimestre cc ON c.CursoId = cc.CursoId"+
                    docentejoin + @"
                    WHERE c.Ind_Estado = 'A'
                    AND cc.CuatrimestreId = @CuatrimestreId
                    AND cc.Ind_Estado = 'A'"+
                    docenteParam;

                query += " ORDER BY c.Codigo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId);
                    if (docenteId.HasValue)
                        cmd.Parameters.AddWithValue("@DocenteId", docenteId.Value);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var curso = new CursoOpcionViewModel
                            {
                                CursoID = reader.GetInt32(0),
                                Codigo = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Creditos = reader.GetInt32(3)
                            };
                            resultado.Add(curso);
                        }
                    }
                }

                // Obtener estadísticas para cada curso
                foreach (var curso in resultado)
                {
                    // Obtener CursoCuatrimestreId
                    string queryCursoCuatrimestre = @"
                        SELECT TOP 1 cc.CursoCuatrimestreId
                        FROM CursoCuatrimestre cc" +
                        docentejoin + @"
                        WHERE cc.CursoId = @CursoId
                        AND cc.CuatrimestreId = @CuatrimestreId
                        AND cc.Ind_Estado = 'A'"+ docenteParam;

                    int cursoCuatrimestreId = 0;
                    using (SqlCommand cmd = new SqlCommand(queryCursoCuatrimestre, conn))
                    {
                        cmd.Parameters.AddWithValue("@CursoId", curso.CursoID);
                        cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId);
                        if (docenteId.HasValue)
                            cmd.Parameters.AddWithValue("@DocenteId", docenteId.Value);

                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            cursoCuatrimestreId = Convert.ToInt32(result);
                    }

                    if (cursoCuatrimestreId == 0)
                        continue;

                    // Total estudiantes
                    string queryEstudiantes = @"
                        SELECT COUNT(*)
                        FROM EstudianteCurso
                        WHERE CursoCuatrimestreId = @CursoCuatrimestreId
                        AND Ind_Estado = 'A'";

                    using (SqlCommand cmd = new SqlCommand(queryEstudiantes, conn))
                    {
                        cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                        curso.TotalEstudiantes = (int)await cmd.ExecuteScalarAsync();
                    }

                    // Total evaluaciones
                    string queryEvaluaciones = @"
                        SELECT COUNT(*)
                        FROM Evaluacion e
                        INNER JOIN EstudianteCurso ec ON e.EstudianteCursoId = ec.EstudianteCursoId
                        WHERE ec.CursoCuatrimestreId = @CursoCuatrimestreId";

                    using (SqlCommand cmd = new SqlCommand(queryEvaluaciones, conn))
                    {
                        cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                        curso.TotalEvaluaciones = (int)await cmd.ExecuteScalarAsync();
                    }
                    // Total estudiantes
                    string queryDocente = @"
                        SELECT COUNT(*)
                        FROM CursoCuatrimestreDocente
                        WHERE CursoCuatrimestreId = @CursoCuatrimestreId
                        AND Ind_Estado = 'A'";

                    using (SqlCommand cmd = new SqlCommand(queryEstudiantes, conn))
                    {
                        cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                        curso.TotalDocentes = (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }

            return resultado;
        }

        // =============================================
        // OBTENER ESTADÍSTICAS
        // =============================================
        public async Task<EstadisticasViewModel> ObtenerEstadisticasAsync(int? cuatrimestreId, int? cursoId, int? docenteId = null)
        {
            var estadisticas = new EstadisticasViewModel
            {
                CuatrimestreID = cuatrimestreId,
                CursoID = cursoId,
                Generales = new EstadisticasGeneralesViewModel(),
                Graficos = new EstadisticasGraficosViewModel(),
                Estudiantes = new List<EstudianteEstadisticaViewModel>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Obtener nombres
                if (cuatrimestreId.HasValue)
                {
                    string query = "SELECT Nombre FROM Cuatrimestre WHERE CuatrimestreId = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", cuatrimestreId.Value);
                        var result = await cmd.ExecuteScalarAsync();
                        estadisticas.NombreCuatrimestre = result?.ToString();
                    }
                }

                if (cursoId.HasValue)
                {
                    string query = "SELECT Nom_Curso, Codigo FROM Curso WHERE CursoId = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", cursoId.Value);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                estadisticas.NombreCurso = reader.GetString(0);
                                estadisticas.CodigoCurso = reader.GetString(1);
                            }
                        }
                    }
                }

                // Obtener estadísticas generales
                await ObtenerEstadisticasGeneralesAsync(conn, estadisticas, cuatrimestreId, cursoId, docenteId);

                // Obtener evaluaciones
                var evaluaciones = await ObtenerEvaluacionesAsync(conn, cuatrimestreId, cursoId, docenteId);

                // Calcular estadísticas de evaluaciones
                CalcularEstadisticasEvaluaciones(estadisticas, evaluaciones);

                // Preparar datos para gráficos
                PrepararDatosGraficos(estadisticas, evaluaciones);

                // Lista de estudiantes
                estadisticas.Estudiantes = await ObtenerListaEstudiantesAsync(conn, cuatrimestreId, cursoId, docenteId);
            }

            return estadisticas;
        }

        // =============================================
        // OBTENER ESTADÍSTICAS GENERALES
        // =============================================
        private async Task ObtenerEstadisticasGeneralesAsync(
            SqlConnection conn,
            EstadisticasViewModel estadisticas,
            int? cuatrimestreId,
            int? cursoId,
            int? docenteId)
        {
            var docentejoin = "";
            var docenteParam = "";
            if (docenteId.HasValue)
            {
                docentejoin += @" INNER JOIN CursoCuatrimestreDocente ccd ON cc.CursoCuatrimestreId = ccd.CursoCuatrimestreId";
                docenteParam = @" AND ccd.DocenteId = @DocenteId";
            }
            string query = @"
                SELECT 
                    COUNT(*) as TotalMatriculados,
                    COUNT(DISTINCT ec.EstudianteId) as TotalEstudiantes
                FROM EstudianteCurso ec
                INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId"+
                docentejoin + @"
                WHERE ec.Ind_Estado = 'A'"+ docenteParam;

            var parameters = new List<SqlParameter>();

            if (cuatrimestreId.HasValue)
            {
                query += " AND cc.CuatrimestreId = @CuatrimestreId";
                parameters.Add(new SqlParameter("@CuatrimestreId", cuatrimestreId.Value));
            }

            if (cursoId.HasValue)
            {
                query += " AND cc.CursoId = @CursoId";
                parameters.Add(new SqlParameter("@CursoId", cursoId.Value));
            }

            if (docenteId.HasValue)
            {
                parameters.Add(new SqlParameter("@DocenteId", docenteId.Value));
            }

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        estadisticas.Generales.TotalMatriculados = reader.GetInt32(0);
                        estadisticas.Generales.TotalEstudiantes = reader.GetInt32(1);
                    }
                }
            }
        }

        // =============================================
        // OBTENER EVALUACIONES
        // =============================================
        private async Task<List<EvaluacionData>> ObtenerEvaluacionesAsync(
            SqlConnection conn,
            int? cuatrimestreId,
            int? cursoId,
            int? docenteId)
        {
            var evaluaciones = new List<EvaluacionData>();
            var docentejoin = "";
            var docenteParam = "";
            if (docenteId.HasValue)
            {
                docentejoin += @" INNER JOIN CursoCuatrimestreDocente ccd ON cc.CursoCuatrimestreId = ccd.CursoCuatrimestreId";
                docenteParam = @" AND ccd.DocenteId = @DocenteId";
            }
            string query = @"
                SELECT 
                    e.EvaluacionId,
                    e.EstudianteCursoId,
                    e.Nota,
                    e.Estado,
                    e.TipoParticipacion,
                    e.Observaciones,
                    e.Fec_Evaluacion
                FROM Evaluacion e
                INNER JOIN EstudianteCurso ec ON e.EstudianteCursoId = ec.EstudianteCursoId
                INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                "+docentejoin+ @"
                WHERE ec.Ind_Estado = 'A'"
                + docenteParam;


            var parameters = new List<SqlParameter>();

            if (cuatrimestreId.HasValue)
            {
                query += " AND cc.CuatrimestreId = @CuatrimestreId";
                parameters.Add(new SqlParameter("@CuatrimestreId", cuatrimestreId.Value));
            }

            if (cursoId.HasValue)
            {
                query += " AND cc.CursoId = @CursoId";
                parameters.Add(new SqlParameter("@CursoId", cursoId.Value));
            }

            if (docenteId.HasValue)
            {
                parameters.Add(new SqlParameter("@DocenteId", docenteId.Value));
            }

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        evaluaciones.Add(new EvaluacionData
                        {
                            EvaluacionId = reader.GetInt32(0),
                            EstudianteCursoId = reader.GetInt32(1),
                            Nota = reader.GetDecimal(2),
                            Estado = reader.GetString(3),
                            TipoParticipacion = reader.GetString(4),
                            Observaciones = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Fec_Evaluacion = reader.GetDateTime(6)
                        });
                    }
                }
            }

            return evaluaciones;
        }

        // =============================================
        // CALCULAR ESTADÍSTICAS DE EVALUACIONES
        // =============================================
        private void CalcularEstadisticasEvaluaciones(
            EstadisticasViewModel estadisticas,
            List<EvaluacionData> evaluaciones)
        {
            estadisticas.Generales.TotalEvaluaciones = evaluaciones.Count;

            // Porcentaje de participación
            if (estadisticas.Generales.TotalMatriculados > 0)
            {
                estadisticas.Generales.PorcentajeParticipacion =
                    (decimal)estadisticas.Generales.TotalEvaluaciones / estadisticas.Generales.TotalMatriculados * 100;
            }

            // Estadísticas de aprobación
            estadisticas.Generales.EstudiantesAprobados = evaluaciones.Count(e => e.Estado == "Aprobado");
            estadisticas.Generales.EstudiantesReprobados = evaluaciones.Count(e => e.Estado == "Reprobado");
            estadisticas.Generales.EstudiantesEnProceso = evaluaciones.Count(e => e.Estado == "En Proceso");

            if (estadisticas.Generales.TotalEvaluaciones > 0)
            {
                estadisticas.Generales.PorcentajeAprobacion =
                    (decimal)estadisticas.Generales.EstudiantesAprobados / estadisticas.Generales.TotalEvaluaciones * 100;
                estadisticas.Generales.PorcentajeReprobacion =
                    (decimal)estadisticas.Generales.EstudiantesReprobados / estadisticas.Generales.TotalEvaluaciones * 100;
                estadisticas.Generales.PorcentajeEnProceso =
                    (decimal)estadisticas.Generales.EstudiantesEnProceso / estadisticas.Generales.TotalEvaluaciones * 100;
            }

            // Promedios
            if (evaluaciones.Any())
            {
                estadisticas.Generales.PromedioGeneral = evaluaciones.Average(e => e.Nota);

                var aprobados = evaluaciones.Where(e => e.Estado == "Aprobado");
                if (aprobados.Any())
                {
                    estadisticas.Generales.PromedioAprobados = aprobados.Average(e => e.Nota);
                }

                var reprobados = evaluaciones.Where(e => e.Estado == "Reprobado");
                if (reprobados.Any())
                {
                    estadisticas.Generales.PromedioReprobados = reprobados.Average(e => e.Nota);
                }
            }

            // Estadísticas de participación
            estadisticas.Generales.ParticipacionExcelente = evaluaciones.Count(e => e.TipoParticipacion == "Excelente");
            estadisticas.Generales.ParticipacionBuena = evaluaciones.Count(e => e.TipoParticipacion == "Buena");
            estadisticas.Generales.ParticipacionRegular = evaluaciones.Count(e => e.TipoParticipacion == "Regular");
            estadisticas.Generales.ParticipacionBaja = evaluaciones.Count(e => e.TipoParticipacion == "Baja");
            estadisticas.Generales.ParticipacionNinguna = evaluaciones.Count(e => e.TipoParticipacion == "Ninguna");
        }

        // =============================================
        // PREPARAR DATOS PARA GRÁFICOS
        // =============================================
        private void PrepararDatosGraficos(EstadisticasViewModel estadisticas, List<EvaluacionData> evaluaciones)
        {
            // Gráfico de estados
            estadisticas.Graficos.EstadosLabels = new List<string> { "Aprobados", "Reprobados", "En Proceso" };
            estadisticas.Graficos.EstadosData = new List<int>
            {
                estadisticas.Generales.EstudiantesAprobados,
                estadisticas.Generales.EstudiantesReprobados,
                estadisticas.Generales.EstudiantesEnProceso
            };
            estadisticas.Graficos.EstadosColors = new List<string> { "#28a745", "#dc3545", "#ffc107" };

            // Gráfico de participación
            estadisticas.Graficos.ParticipacionLabels = new List<string>
            {
                "Excelente", "Buena", "Regular", "Baja", "Ninguna"
            };
            estadisticas.Graficos.ParticipacionData = new List<int>
            {
                estadisticas.Generales.ParticipacionExcelente,
                estadisticas.Generales.ParticipacionBuena,
                estadisticas.Generales.ParticipacionRegular,
                estadisticas.Generales.ParticipacionBaja,
                estadisticas.Generales.ParticipacionNinguna
            };
            estadisticas.Graficos.ParticipacionColors = new List<string>
            {
                "#28a745", "#17a2b8", "#ffc107", "#fd7e14", "#6c757d"
            };

            // Distribución de notas
            estadisticas.Graficos.NotasRangos = new List<string>
            {
                "0-59", "60-69", "70-79", "80-89", "90-100"
            };
            estadisticas.Graficos.NotasDistribucion = new List<int>
            {
                evaluaciones.Count(e => e.Nota < 60),
                evaluaciones.Count(e => e.Nota >= 60 && e.Nota < 70),
                evaluaciones.Count(e => e.Nota >= 70 && e.Nota < 80),
                evaluaciones.Count(e => e.Nota >= 80 && e.Nota < 90),
                evaluaciones.Count(e => e.Nota >= 90)
            };
        }

        // =============================================
        // OBTENER LISTA DE ESTUDIANTES
        // =============================================
        private async Task<List<EstudianteEstadisticaViewModel>> ObtenerListaEstudiantesAsync(
            SqlConnection conn,
            int? cuatrimestreId,
            int? cursoId,
            int? docenteId)
        {
            var resultado = new List<EstudianteEstadisticaViewModel>();
            var docentejoin = "";
            var docenteParam = "";
            if (docenteId.HasValue)
            {
                docentejoin += @" INNER JOIN CursoCuatrimestreDocente ccd ON cc.CursoCuatrimestreId = ccd.CursoCuatrimestreId";
                docenteParam = @" AND ccd.DocenteId = @DocenteId";
            }

            string query = @"
                SELECT 
                    e.EstudianteId,
                    e.Identificacion,
                    e.Nombre,
                    e.Apellidos,
                    e.Email,
                    c.Nom_Curso,
                    ec.EstudianteCursoId,
                    ev.Nota,
                    ev.Estado,
                    ev.TipoParticipacion,
                    ev.Observaciones,
                    ev.Fec_Evaluacion
                FROM EstudianteCurso ec
                INNER JOIN Estudiante e ON ec.EstudianteId = e.EstudianteId
                INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                "+docentejoin+ @"
                INNER JOIN Curso c ON cc.CursoId = c.CursoId
                LEFT JOIN Evaluacion ev ON ec.EstudianteCursoId = ev.EstudianteCursoId
                WHERE ec.Ind_Estado = 'A'"+
                docenteParam;

            var parameters = new List<SqlParameter>();

            if (cuatrimestreId.HasValue)
            {
                query += " AND cc.CuatrimestreId = @CuatrimestreId";
                parameters.Add(new SqlParameter("@CuatrimestreId", cuatrimestreId.Value));
            }

            if (cursoId.HasValue)
            {
                query += " AND cc.CursoId = @CursoId";
                parameters.Add(new SqlParameter("@CursoId", cursoId.Value));
            }

            if (docenteId.HasValue)
            { 
                parameters.Add(new SqlParameter("@DocenteId", docenteId.Value));
            }

            query += " ORDER BY e.Nombre, e.Apellidos";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        resultado.Add(new EstudianteEstadisticaViewModel
                        {
                            EstudianteID = reader.GetInt32(0),
                            Identificacion = reader.GetString(1),
                            NombreCompleto = $"{reader.GetString(2)} {reader.GetString(3)}",
                            Email = reader.GetString(4),
                            NombreCurso = reader.GetString(5),
                            TieneEvaluacion = !reader.IsDBNull(7),
                            Nota = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7),
                            Estado = reader.IsDBNull(8) ? null : reader.GetString(8),
                            TipoParticipacion = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Observaciones = reader.IsDBNull(10) ? null : reader.GetString(10),
                            FechaEvaluacion = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11)
                        });
                    }
                }
            }

            return resultado;
        }

        // =============================================
        // OBTENER COMPARATIVA DE CURSOS
        // =============================================
        public async Task<ComparativaCursosViewModel> ObtenerComparativaCursosAsync(int cuatrimestreId, int? docenteId = null)
        {
            var cursos = await ObtenerCursosPorCuatrimestreAsync(cuatrimestreId, docenteId);
            var comparativa = new ComparativaCursosViewModel
            {
                Cursos = new List<CursoComparativoViewModel>()
            };

            foreach (var curso in cursos)
            {
                var stats = await ObtenerEstadisticasAsync(cuatrimestreId, curso.CursoID, docenteId);

                comparativa.Cursos.Add(new CursoComparativoViewModel
                {
                    CodigoCurso = curso.Codigo,
                    NombreCurso = curso.Nombre,
                    TotalEstudiantes = curso.TotalEstudiantes,
                    PromedioNota = stats.Generales.PromedioGeneral,
                    PorcentajeAprobacion = stats.Generales.PorcentajeAprobacion,
                    Aprobados = stats.Generales.EstudiantesAprobados,
                    Reprobados = stats.Generales.EstudiantesReprobados
                });
            }

            return comparativa;
        }

        // =============================================
        // CLASE AUXILIAR PARA DATOS DE EVALUACIÓN
        // =============================================
        private class EvaluacionData
        {
            public int EvaluacionId { get; set; }
            public int EstudianteCursoId { get; set; }
            public decimal Nota { get; set; }
            public string Estado { get; set; }
            public string TipoParticipacion { get; set; }
            public string Observaciones { get; set; }
            public DateTime Fec_Evaluacion { get; set; }
        }
    }
}