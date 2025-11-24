using Microsoft.Data.SqlClient;
using SistemaAcademico.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Repositories
{
    public class HistorialAcademicoDB
    {
        private readonly string _connectionString;

        public HistorialAcademicoDB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<EstudianteInfo>> BuscarEstudiantesAsync(string termino)
        {
            var lista = new List<EstudianteInfo>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        e.EstudianteId,
                        e.Identificacion,
                        e.Nombre,
                        e.Apellidos,
                        e.Fec_Nacimiento,
                        e.Email,
                        d.Nom_Distrito as Distrito,
                        c.Nom_Canton as Canton,
                        p.Nom_Provincia as Provincia
                    FROM Estudiante e
                    INNER JOIN Distrito d ON e.DistritoId = d.DistritoId
                    INNER JOIN Canton c ON d.CantonId = c.CantonId
                    INNER JOIN Provincia p ON c.ProvinciaId = p.ProvinciaId
                    WHERE e.Ind_Estado = 'A'
                        AND (
                            e.Nombre LIKE @Termino 
                            OR e.Apellidos LIKE @Termino
                            OR e.Identificacion LIKE @Termino
                            OR (e.Nombre + ' ' + e.Apellidos) LIKE @Termino
                        )
                    ORDER BY e.Apellidos, e.Nombre";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Termino", $"%{termino}%");
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapEstudianteFromReader(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<EstudianteInfo> GetEstudianteByIdAsync(int estudianteId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        e.EstudianteId,
                        e.Identificacion,
                        e.Nombre,
                        e.Apellidos,
                        e.Fec_Nacimiento,
                        e.Email,
                        d.Nom_Distrito as Distrito,
                        c.Nom_Canton as Canton,
                        p.Nom_Provincia as Provincia
                    FROM Estudiante e
                    INNER JOIN Distrito d ON e.DistritoId = d.DistritoId
                    INNER JOIN Canton c ON d.CantonId = c.CantonId
                    INNER JOIN Provincia p ON c.ProvinciaId = p.ProvinciaId
                    WHERE e.EstudianteId = @EstudianteId AND e.Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapEstudianteFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<HistorialAcademicoItem>> GetHistorialCompletoAsync(int estudianteId)
        {
            var lista = new List<HistorialAcademicoItem>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        ev.EvaluacionId,
                        ec.EstudianteCursoId,
                        cu.CuatrimestreId,
                        cu.Nombre as NombreCuatrimestre,
                        cu.Anio,
                        cu.Numero as NumeroCuatrimestre,
                        cu.Fec_Inicio as Fec_InicioCuatrimestre,
                        cu.Fec_Fin as Fec_FinCuatrimestre,
                        c.CursoId,
                        c.Codigo as CodigoCurso,
                        c.Nom_Curso as NombreCurso,
                        c.Num_Creditos,
                        ev.Nota,
                        ev.Estado,
                        ev.TipoParticipacion,
                        ev.Observaciones,
                        ev.Fec_Evaluacion,
                        ec.Fec_Matricula,
                        d.DocenteId,
                        d.Nombre + ' ' + d.Apellidos as NombreDocente
                    FROM Evaluacion ev
                    INNER JOIN EstudianteCurso ec ON ev.EstudianteCursoId = ec.EstudianteCursoId
                    INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                    INNER JOIN Curso c ON cc.CursoId = c.CursoId
                    INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                    INNER JOIN Docente d ON ev.DocenteId = d.DocenteId
                    WHERE ec.EstudianteId = @EstudianteId
                        AND ev.Ind_Estado = 'A'
                        AND ec.Ind_Estado = 'A'
                    ORDER BY cu.Anio DESC, cu.Numero DESC, c.Nom_Curso";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapHistorialFromReader(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<ResumenAcademico> GetResumenAcademicoAsync(int estudianteId)
        {
            var resumen = new ResumenAcademico();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        COUNT(*) as TotalCursos,
                        SUM(CASE WHEN ev.Estado = 'Aprobado' THEN 1 ELSE 0 END) as CursosAprobados,
                        SUM(CASE WHEN ev.Estado = 'Reprobado' THEN 1 ELSE 0 END) as CursosReprobados,
                        SUM(CASE WHEN ev.Estado = 'En Proceso' THEN 1 ELSE 0 END) as CursosEnProceso,
                        AVG(ev.Nota) as PromedioGeneral,
                        MAX(ev.Nota) as NotaMasAlta,
                        MIN(ev.Nota) as NotaMasBaja,
                        SUM(c.Num_Creditos) as TotalCreditos,
                        SUM(CASE WHEN ev.Estado = 'Aprobado' THEN c.Num_Creditos ELSE 0 END) as CreditosAprobados
                    FROM Evaluacion ev
                    INNER JOIN EstudianteCurso ec ON ev.EstudianteCursoId = ec.EstudianteCursoId
                    INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                    INNER JOIN Curso c ON cc.CursoId = c.CursoId
                    WHERE ec.EstudianteId = @EstudianteId
                        AND ev.Ind_Estado = 'A'
                        AND ec.Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resumen.TotalCursos = reader.GetInt32(0);
                            resumen.CursosAprobados = reader.GetInt32(1);
                            resumen.CursosReprobados = reader.GetInt32(2);
                            resumen.CursosEnProceso = reader.GetInt32(3);
                            resumen.PromedioGeneral = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4);
                            resumen.NotaMasAlta = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5);
                            resumen.NotaMasBaja = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6);
                            resumen.TotalCreditos = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                            resumen.CreditosAprobados = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                        }
                    }
                }
            }
            return resumen;
        }

        public async Task<bool> TieneHistorialAsync(int estudianteId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM Evaluacion ev
                    INNER JOIN EstudianteCurso ec ON ev.EstudianteCursoId = ec.EstudianteCursoId
                    WHERE ec.EstudianteId = @EstudianteId
                        AND ev.Ind_Estado = 'A'
                        AND ec.Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    await conn.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        private EstudianteInfo MapEstudianteFromReader(SqlDataReader reader)
        {
            return new EstudianteInfo
            {
                EstudianteId = reader.GetInt32(0),
                Identificacion = reader.GetString(1),
                Nombre = reader.GetString(2),
                Apellidos = reader.GetString(3),
                Fec_Nacimiento = reader.GetDateTime(4),
                Email = reader.GetString(5),
                Distrito = reader.GetString(6),
                Canton = reader.GetString(7),
                Provincia = reader.GetString(8)
            };
        }

        private HistorialAcademicoItem MapHistorialFromReader(SqlDataReader reader)
        {
            return new HistorialAcademicoItem
            {
                EvaluacionId = reader.GetInt32(0),
                EstudianteCursoId = reader.GetInt32(1),
                CuatrimestreId = reader.GetInt32(2),
                NombreCuatrimestre = reader.GetString(3),
                Anio = reader.GetInt32(4),
                NumeroCuatrimestre = reader.GetInt32(5),
                Fec_InicioCuatrimestre = reader.GetDateTime(6),
                Fec_FinCuatrimestre = reader.GetDateTime(7),
                CursoId = reader.GetInt32(8),
                CodigoCurso = reader.GetString(9),
                NombreCurso = reader.GetString(10),
                Num_Creditos = reader.GetInt32(11),
                Nota = reader.GetDecimal(12),
                Estado = reader.GetString(13),
                TipoParticipacion = reader.GetString(14),
                Observaciones = reader.IsDBNull(15) ? null : reader.GetString(15),
                Fec_Evaluacion = reader.GetDateTime(16),
                Fec_Matricula = reader.GetDateTime(17),
                DocenteId = reader.GetInt32(18),
                NombreDocente = reader.GetString(19)
            };
        }
    }
}
