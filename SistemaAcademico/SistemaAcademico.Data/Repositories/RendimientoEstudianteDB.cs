using Microsoft.Data.SqlClient; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaAcademico.Data.Entities;

namespace SistemaAcademico.Data.Repositories
{
    public class RendimientoEstudianteDB
    {
        private readonly string _connectionString;

        public RendimientoEstudianteDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Obtener rendimiento completo del estudiante
        public RendimientoEstudiante ObtenerRendimientoCompleto(int estudianteId, FiltrosRendimiento filtros = null)
        {
            var rendimiento = new RendimientoEstudiante
            {
                NotasCursos = new List<NotaCurso>(),
                RendimientoPorCuatrimestre = new List<RendimientoCuatrimestre>()
            };

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Obtener datos del estudiante
                rendimiento.Estudiante = ObtenerEstudiante(conn, estudianteId);

                // Obtener notas por curso
                rendimiento.NotasCursos = ObtenerNotasCursos(conn, estudianteId, filtros);

                // Obtener rendimiento por cuatrimestre
                rendimiento.RendimientoPorCuatrimestre = ObtenerRendimientoPorCuatrimestre(conn, estudianteId, filtros);

                // Calcular estadísticas
                rendimiento.Estadisticas = CalcularEstadisticas(estudianteId, rendimiento.NotasCursos);
            }

            return rendimiento;
        }

        // Obtener información del estudiante
        private Estudiante ObtenerEstudiante(SqlConnection conn, int estudianteId)
        {
            var cmd = new SqlCommand(@"
                SELECT EstudianteId, Identificacion, Nombre, Apellidos, Fec_Nacimiento, 
                       Email, DistritoId, Ind_Estado, Fec_Registro, UserId
                FROM Estudiante 
                WHERE EstudianteId = @EstudianteId", conn);

            cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);

            var reader = cmd.ExecuteReader();
            Estudiante estudiante = null;

            if (reader.Read())
            {
                estudiante = new Estudiante
                {
                    EstudianteId = reader.GetInt32(0),
                    Identificacion = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Apellidos = reader.GetString(3),
                    Fec_Nacimiento = reader.GetDateTime(4),
                    Email = reader.GetString(5),
                    DistritoId = reader.GetInt32(6),
                    Ind_Estado = reader.GetString(7),
                    Fec_Registro = reader.GetDateTime(8),
                    UserId = reader.IsDBNull(9) ? null : reader.GetString(9)
                };
            }
            reader.Close();

            return estudiante;
        }

        // Obtener notas por curso con filtros
        private List<NotaCurso> ObtenerNotasCursos(SqlConnection conn, int estudianteId, FiltrosRendimiento filtros)
        {
            var notas = new List<NotaCurso>();

            var query = @"
                SELECT 
                    e.EvaluacionId,
                    c.CursoId,
                    c.Codigo AS CodigoCurso,
                    c.Nom_Curso AS NombreCurso,
                    cu.CuatrimestreId,
                    cu.Nombre AS NombreCuatrimestre,
                    cu.Anio,
                    cu.Numero,
                    e.Nota,
                    e.Estado,
                    e.Fec_Evaluacion,
                    e.TipoParticipacion
                FROM Evaluacion e
                INNER JOIN EstudianteCurso ec ON e.EstudianteCursoId = ec.EstudianteCursoId
                INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                INNER JOIN Curso c ON cc.CursoId = c.CursoId
                INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                WHERE ec.EstudianteId = @EstudianteId
                  AND e.Estado IN ('Aprobado', 'Reprobado')";

            // Aplicar filtros
            if (filtros != null)
            {
                if (filtros.FechaDesde.HasValue)
                    query += " AND e.Fec_Evaluacion >= @FechaDesde";
                if (filtros.FechaHasta.HasValue)
                    query += " AND e.Fec_Evaluacion <= @FechaHasta";
                if (filtros.CursosIds != null && filtros.CursosIds.Count > 0)
                    query += " AND c.CursoId IN (" + string.Join(",", filtros.CursosIds) + ")";
                if (filtros.CuatrimestresIds != null && filtros.CuatrimestresIds.Count > 0)
                    query += " AND cu.CuatrimestreId IN (" + string.Join(",", filtros.CuatrimestresIds) + ")";
            }

            query += " ORDER BY cu.Anio, cu.Numero, c.Codigo";

            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);

            if (filtros != null)
            {
                if (filtros.FechaDesde.HasValue)
                    cmd.Parameters.AddWithValue("@FechaDesde", filtros.FechaDesde.Value);
                if (filtros.FechaHasta.HasValue)
                    cmd.Parameters.AddWithValue("@FechaHasta", filtros.FechaHasta.Value);
            }

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                notas.Add(new NotaCurso
                {
                    EvaluacionId = reader.GetInt32(0),
                    CursoId = reader.GetInt32(1),
                    CodigoCurso = reader.GetString(2),
                    NombreCurso = reader.GetString(3),
                    CuatrimestreId = reader.GetInt32(4),
                    NombreCuatrimestre = reader.GetString(5),
                    AnioCuatrimestre = reader.GetInt32(6),
                    NumeroCuatrimestre = reader.GetInt32(7),
                    Nota = reader.GetDecimal(8),
                    Estado = reader.GetString(9),
                    FechaEvaluacion = reader.GetDateTime(10),
                    TipoParticipacion = reader.GetString(11)
                });
            }
            reader.Close();

            return notas;
        }

        // Obtener rendimiento agregado por cuatrimestre
        private List<RendimientoCuatrimestre> ObtenerRendimientoPorCuatrimestre(SqlConnection conn, int estudianteId, FiltrosRendimiento filtros)
        {
            var rendimientos = new List<RendimientoCuatrimestre>();

            var query = @"
                SELECT 
                    cu.CuatrimestreId,
                    cu.Nombre AS NombreCuatrimestre,
                    cu.Anio,
                    cu.Numero,
                    AVG(e.Nota) AS PromedioNotas,
                    SUM(CASE WHEN e.Estado = 'Aprobado' THEN 1 ELSE 0 END) AS CursosAprobados,
                    SUM(CASE WHEN e.Estado = 'Reprobado' THEN 1 ELSE 0 END) AS CursosReprobados,
                    COUNT(*) AS TotalCursos,
                    cu.Fec_Inicio,
                    cu.Fec_Fin
                FROM Evaluacion e
                INNER JOIN EstudianteCurso ec ON e.EstudianteCursoId = ec.EstudianteCursoId
                INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                WHERE ec.EstudianteId = @EstudianteId
                  AND e.Estado IN ('Aprobado', 'Reprobado')";

            // Aplicar filtros
            if (filtros != null)
            {
                if (filtros.FechaDesde.HasValue)
                    query += " AND cu.Fec_Inicio >= @FechaDesde";
                if (filtros.FechaHasta.HasValue)
                    query += " AND cu.Fec_Fin <= @FechaHasta";
                if (filtros.CuatrimestresIds != null && filtros.CuatrimestresIds.Count > 0)
                    query += " AND cu.CuatrimestreId IN (" + string.Join(",", filtros.CuatrimestresIds) + ")";
            }

            query += @"
                GROUP BY cu.CuatrimestreId, cu.Nombre, cu.Anio, cu.Numero, cu.Fec_Inicio, cu.Fec_Fin
                ORDER BY cu.Anio, cu.Numero";

            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);

            if (filtros != null)
            {
                if (filtros.FechaDesde.HasValue)
                    cmd.Parameters.AddWithValue("@FechaDesde", filtros.FechaDesde.Value);
                if (filtros.FechaHasta.HasValue)
                    cmd.Parameters.AddWithValue("@FechaHasta", filtros.FechaHasta.Value);
            }

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rendimientos.Add(new RendimientoCuatrimestre
                {
                    CuatrimestreId = reader.GetInt32(0),
                    NombreCuatrimestre = reader.GetString(1),
                    Anio = reader.GetInt32(2),
                    Numero = reader.GetInt32(3),
                    PromedioNotas = reader.GetDecimal(4),
                    CursosAprobados = reader.GetInt32(5),
                    CursosReprobados = reader.GetInt32(6),
                    TotalCursos = reader.GetInt32(7),
                    FechaInicio = reader.GetDateTime(8),
                    FechaFin = reader.GetDateTime(9)
                });
            }
            reader.Close();

            return rendimientos;
        }

        // Calcular estadísticas generales
        private EstadisticasEstudiante CalcularEstadisticas(int estudianteId, List<NotaCurso> notas)
        {
            if (notas.Count == 0)
            {
                return new EstadisticasEstudiante
                {
                    EstudianteId = estudianteId,
                    PromedioGeneral = 0,
                    TotalCursosAprobados = 0,
                    TotalCursosReprobados = 0,
                    TotalCursosCursados = 0,
                    PorcentajeAprobacion = 0,
                    NotaMasAlta = 0,
                    NotaMasBaja = 0
                };
            }

            var aprobados = 0;
            var reprobados = 0;
            decimal sumaNotas = 0;
            var notaMasAlta = notas[0];
            var notaMasBaja = notas[0];

            foreach (var nota in notas)
            {
                sumaNotas += nota.Nota;

                if (nota.Estado == "Aprobado")
                    aprobados++;
                else if (nota.Estado == "Reprobado")
                    reprobados++;

                if (nota.Nota > notaMasAlta.Nota)
                    notaMasAlta = nota;
                if (nota.Nota < notaMasBaja.Nota)
                    notaMasBaja = nota;
            }

            int total = notas.Count;

            return new EstadisticasEstudiante
            {
                EstudianteId = estudianteId,
                PromedioGeneral = sumaNotas / total,
                TotalCursosAprobados = aprobados,
                TotalCursosReprobados = reprobados,
                TotalCursosCursados = total,
                PorcentajeAprobacion = total > 0 ? ((decimal)aprobados / total) * 100 : 0,
                NotaMasAlta = notaMasAlta.Nota,
                NotaMasBaja = notaMasBaja.Nota,
                CursoMejorNota = notaMasAlta.NombreCurso,
                CursoPeorNota = notaMasBaja.NombreCurso
            };
        }

        // Obtener EstudianteId desde UserId
        public int? ObtenerEstudianteIdPorUserId(string userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                    SELECT EstudianteId 
                    FROM Estudiante 
                    WHERE UserId = @UserId", conn);

                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                var result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value ? (int)result : (int?)null;
            }
        }

        // Obtener cuatrimestres del estudiante
        public List<Cuatrimestre> ObtenerCuatrimestresEstudiante(int estudianteId)
        {
            var cuatrimestres = new List<Cuatrimestre>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                    SELECT DISTINCT cu.CuatrimestreId, cu.Nombre, cu.Anio, cu.Numero, 
                                    cu.Fec_Inicio, cu.Fec_Fin, cu.Ind_Estado, cu.Fec_Registro
                    FROM Cuatrimestre cu
                    INNER JOIN CursoCuatrimestre cc ON cu.CuatrimestreId = cc.CuatrimestreId
                    INNER JOIN EstudianteCurso ec ON cc.CursoCuatrimestreId = ec.CursoCuatrimestreId
                    WHERE ec.EstudianteId = @EstudianteId
                    ORDER BY cu.Anio, cu.Numero", conn);

                cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                conn.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cuatrimestres.Add(new Cuatrimestre
                    {
                        CuatrimestreId = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Anio = reader.GetInt32(2),
                        Numero = reader.GetInt32(3),
                        Fec_Inicio = reader.GetDateTime(4),
                        Fec_Fin = reader.GetDateTime(5),
                        Ind_Estado = reader.GetString(6),
                        Fec_Registro = reader.GetDateTime(7)
                    });
                }
            }

            return cuatrimestres;
        }

        // Obtener cursos del estudiante
        public List<Curso> ObtenerCursosEstudiante(int estudianteId)
        {
            var cursos = new List<Curso>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                    SELECT DISTINCT c.CursoId, c.Codigo, c.Nom_Curso, c.Desc_Curso, 
                                    c.Num_Creditos, c.Ind_Estado, c.Fec_Registro
                    FROM Curso c
                    INNER JOIN CursoCuatrimestre cc ON c.CursoId = cc.CursoId
                    INNER JOIN EstudianteCurso ec ON cc.CursoCuatrimestreId = ec.CursoCuatrimestreId
                    WHERE ec.EstudianteId = @EstudianteId
                    ORDER BY c.Codigo", conn);

                cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                conn.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cursos.Add(new Curso
                    {
                        CursoId = reader.GetInt32(0),
                        Codigo = reader.GetString(1),
                        Nom_Curso = reader.GetString(2),
                        Desc_Curso = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Num_Creditos = reader.GetInt32(4),
                        Ind_Estado = reader.GetString(5),
                        Fec_Registro = reader.GetDateTime(6)
                    });
                }
            }

            return cursos;
        }

        // Verificar si hay nuevas evaluaciones
        public bool VerificarNuevasEvaluaciones(int estudianteId, DateTime ultimaConsulta)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Evaluacion e
                    INNER JOIN EstudianteCurso ec ON e.EstudianteCursoId = ec.EstudianteCursoId
                    WHERE ec.EstudianteId = @EstudianteId 
                      AND e.Fec_Registro > @UltimaConsulta", conn);

                cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                cmd.Parameters.AddWithValue("@UltimaConsulta", ultimaConsulta);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }
    }
}
