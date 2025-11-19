using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SistemaAcademico.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Repositories
{
    public class CursoCuatrimestreDB
    {
        private readonly string _connectionString;

        public CursoCuatrimestreDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<CursoCuatrimestre>> ObtenerTodos(int? cuatrimestreId = null)
        {
            var lista = new List<CursoCuatrimestre>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT cc.CursoCuatrimestreId, cc.CursoId, cc.CuatrimestreId, 
                           cc.Ind_Estado, cc.Fec_Registro,
                           c.Codigo, c.Nom_Curso, cu.Nombre as NombreCuatrimestre
                    FROM CursoCuatrimestre cc
                    INNER JOIN Curso c ON cc.CursoId = c.CursoId
                    INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                    WHERE cc.Ind_Estado = 'A'";

                if (cuatrimestreId.HasValue)
                    query += " AND cc.CuatrimestreId = @CuatrimestreId";

                query += " ORDER BY cu.Anio DESC, cu.Numero DESC, c.Nom_Curso";

                using (var cmd = new SqlCommand(query, conn))
                {
                    if (cuatrimestreId.HasValue)
                        cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId.Value);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<CursoCuatrimestre> ObtenerPorId(int id)
        {
            CursoCuatrimestre entity = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT cc.*, c.Codigo, c.Nom_Curso, c.Desc_Curso, c.Num_Creditos,
                           cu.Nombre, cu.Anio, cu.Numero, cu.Fec_Inicio, cu.Fec_Fin
                    FROM CursoCuatrimestre cc
                    INNER JOIN Curso c ON cc.CursoId = c.CursoId
                    INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                    WHERE cc.CursoCuatrimestreId = @Id AND cc.Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            entity = MapFromReaderDetailed(reader);
                        }
                    }
                }
            }
            return entity;
        }

        public async Task<CursoCuatrimestre> ObtenerConDetalle(int id)
        {
            return await ObtenerPorId(id);
        }

        public async Task<int> CreateAsync(CursoCuatrimestre entity)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO CursoCuatrimestre (CursoId, CuatrimestreId, Ind_Estado, Fec_Registro)
                    VALUES (@CursoId, @CuatrimestreId, @Ind_Estado, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CursoId", entity.CursoId);
                    cmd.Parameters.AddWithValue("@CuatrimestreId", entity.CuatrimestreId);
                    cmd.Parameters.AddWithValue("@Ind_Estado", entity.Ind_Estado ?? "A");

                    await conn.OpenAsync();
                    var id = (int)await cmd.ExecuteScalarAsync();
                    return id;
                }
            }
        }

        public async Task<bool> Actualizar(CursoCuatrimestre entity)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE CursoCuatrimestre 
                    SET CursoId = @CursoId, 
                        CuatrimestreId = @CuatrimestreId
                    WHERE CursoCuatrimestreId = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", entity.CursoCuatrimestreId);
                    cmd.Parameters.AddWithValue("@CursoId", entity.CursoId);
                    cmd.Parameters.AddWithValue("@CuatrimestreId", entity.CuatrimestreId);

                    await conn.OpenAsync();
                    var rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "UPDATE CursoCuatrimestre SET Ind_Estado = 'I' WHERE CursoCuatrimestreId = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    var rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public async Task<bool> Existe(int cursoId, int cuatrimestreId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM CursoCuatrimestre 
                    WHERE CursoId = @CursoId AND CuatrimestreId = @CuatrimestreId AND Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CursoId", cursoId);
                    cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId);
                    await conn.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> TieneEstudiantes(int cursoCuatrimestreId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM EstudianteCurso 
                    WHERE CursoCuatrimestreId = @Id AND Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cursoCuatrimestreId);
                    await conn.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> TieneEvaluaciones(int cursoCuatrimestreId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM Evaluacion e
                    INNER JOIN EstudianteCurso ec ON e.EstudianteCursoId = ec.EstudianteCursoId
                    WHERE ec.CursoCuatrimestreId = @Id AND e.Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cursoCuatrimestreId);
                    await conn.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> PuedeEliminar(int cursoCuatrimestreId)
        {
            var tieneEstudiantes = await TieneEstudiantes(cursoCuatrimestreId);
            var tieneEvaluaciones = await TieneEvaluaciones(cursoCuatrimestreId);
            var docentes = await ObtenerDocentePorCurso(cursoCuatrimestreId);

            return !tieneEstudiantes && !tieneEvaluaciones && !docentes.Any();
        }

        // CursoCuatrimestreDocente Methods
        public async Task<IEnumerable<CursoCuatrimestreDocente>> ObtenerDocentePorCurso(int cursoCuatrimestreId)
        {
            var lista = new List<CursoCuatrimestreDocente>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT ccd.*, d.Nombre, d.Apellidos, d.Email
                    FROM CursoCuatrimestreDocente ccd
                    INNER JOIN Docente d ON ccd.DocenteId = d.DocenteId
                    WHERE ccd.CursoCuatrimestreId = @Id AND ccd.Ind_Estado = 'A'
                    ORDER BY d.Apellidos, d.Nombre";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cursoCuatrimestreId);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new CursoCuatrimestreDocente
                            {
                                CursoCuatriDocenteId = reader.GetInt32(0),
                                CursoCuatrimestreId = reader.GetInt32(1),
                                DocenteId = reader.GetInt32(2),
                                Ind_Estado = reader.GetString(3),
                                Fec_Registro = reader.GetDateTime(4),
                                Docente = new Docente
                                {
                                    DocenteId = reader.GetInt32(2),
                                    Nombre = reader.GetString(5),
                                    Apellidos = reader.GetString(6),
                                    Email = reader.GetString(7)
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public async Task<IEnumerable<EstudianteCurso>> ObtenerEstudientePorCurso(int cursoCuatrimestreId)
        {
            var lista = new List<EstudianteCurso>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT ec.*
                    FROM EstudianteCurso ec 
                    WHERE ec.CursoCuatrimestreId = @Id AND ec.Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cursoCuatrimestreId);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new EstudianteCurso
                            {
                                EstudianteCursoId = reader.GetInt32(0),
                                EstudianteId = reader.GetInt32(1),
                                CursoCuatrimestreId = reader.GetInt32(2),
                                Fec_Matricula = reader.GetDateTime(3),
                                Ind_Estado = reader.GetString(4),
                                Fec_Registro = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public async Task<bool> TieneAsignacion(int cursoCuatrimestreId, int docenteId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM CursoCuatrimestreDocente 
                    WHERE CursoCuatrimestreId = @cursoCuatrimestreId AND Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cursoCuatrimestreId);
                    await conn.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }
        public async Task<bool> AsignarDocente(int cursoCuatrimestreId, int docenteId)
        {
            var existeAsignacionInactiva = await DocenteYaAsignadoInactivo(cursoCuatrimestreId, docenteId);
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "";
                if (existeAsignacionInactiva) {
                    query = @"UPDATE CursoCuatrimestreDocente SET ind_estado = 'A' WHERE CursoCuatrimestreId = @CursoCuatrimestreId and DocenteId = @DocenteId";
                }
                else{
                    query = @"
                    INSERT INTO CursoCuatrimestreDocente (CursoCuatrimestreId, DocenteId, Ind_Estado, Fec_Registro)
                    VALUES (@CursoCuatrimestreId, @DocenteId, 'A', GETDATE())";
                }
                   

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                    cmd.Parameters.AddWithValue("@DocenteId", docenteId);

                    await conn.OpenAsync();
                    var rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public async Task<bool> RemoverDocente(int cursoCuatriDocenteId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "UPDATE CursoCuatrimestreDocente SET Ind_Estado = 'I' WHERE CursoCuatriDocenteId = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cursoCuatriDocenteId);
                    await conn.OpenAsync();
                    var rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public async Task<bool> DocenteYaAsignado(int cursoCuatrimestreId, int docenteId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM CursoCuatrimestreDocente 
                    WHERE CursoCuatrimestreId = @CursoCuatrimestreId 
                    AND DocenteId = @DocenteId 
                    AND Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                    cmd.Parameters.AddWithValue("@DocenteId", docenteId);
                    await conn.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> DocenteYaAsignadoInactivo(int cursoCuatrimestreId, int docenteId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(*) 
                    FROM CursoCuatrimestreDocente 
                    WHERE CursoCuatrimestreId = @CursoCuatrimestreId 
                    AND DocenteId = @DocenteId 
                    AND Ind_Estado = 'I'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                    cmd.Parameters.AddWithValue("@DocenteId", docenteId);
                    await conn.OpenAsync();
                    var count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        // Catálogos
        public async Task<IEnumerable<Cuatrimestre>> ObtenerCuatrimestresActivos()
        {
            var lista = new List<Cuatrimestre>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Cuatrimestre WHERE Ind_Estado = 'A' ORDER BY Anio DESC, Numero DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapCuatrimestreFromReader(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<Cuatrimestre> ObtenerCuatrimestrePorId(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Cuatrimestre WHERE CuatrimestreId = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapCuatrimestreFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<Curso>> ObtenerCursosActivos()
        {
            var lista = new List<Curso>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Curso WHERE Ind_Estado = 'A' ORDER BY Nom_Curso";
                using (var cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapCursoFromReader(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<Curso> ObtenerCursoPorId(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Curso WHERE CursoId = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapCursoFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }
        public async Task<IEnumerable<Curso>> ObtenerCursoNoEnCuartrimestre(int cuatrimestreId)
        {
            var lista = new List<Curso>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"SELECT * 
                              FROM Curso 
                              WHERE CursoId not in (select cc.cursoid 
                                                    from CursoCuatrimestre cc 
                                                    where cc.cuatrimestreId = @Id
                                                    )";
                using (var cmd = new SqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@Id", cuatrimestreId);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapCursoFromReader(reader));
                        }
                    }
                }
            }
            return lista; 
        }
        public async Task<IEnumerable<Docente>> ObtenerDocentesActivos()
        {
            var lista = new List<Docente>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"SELECT d.* 
                              FROM Docente d
                              WHERE d.Ind_Estado = 'A' 
                                AND NOT EXISTS (SELECT 'S' FROM UsuarioRoles WHERE userid = d.userid and roleid ='ADMIN'  )
                              ORDER BY d.Apellidos, d.Nombre";
                using (var cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapDocenteFromReader(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<Docente> ObtenerDocentePorId(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Docente WHERE DocenteId = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapDocenteFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        // Métodos auxiliares de mapeo
        private CursoCuatrimestre MapFromReader(SqlDataReader reader)
        {
            return new CursoCuatrimestre
            {
                CursoCuatrimestreId = reader.GetInt32(reader.GetOrdinal("CursoCuatrimestreId")),
                CursoId = reader.GetInt32(reader.GetOrdinal("CursoId")),
                CuatrimestreId = reader.GetInt32(reader.GetOrdinal("CuatrimestreId")),
                Ind_Estado = reader.GetString(reader.GetOrdinal("Ind_Estado")),
                Fec_Registro = reader.GetDateTime(reader.GetOrdinal("Fec_Registro")),
                Curso = new Curso
                {
                    Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                    Nom_Curso = reader.GetString(reader.GetOrdinal("Nom_Curso"))
                },
                Cuatrimestre = new Cuatrimestre
                {
                    Nombre = reader.GetString(reader.GetOrdinal("NombreCuatrimestre"))
                }
            };
        }

        private CursoCuatrimestre MapFromReaderDetailed(SqlDataReader reader)
        {
            return new CursoCuatrimestre
            {
                CursoCuatrimestreId = reader.GetInt32(reader.GetOrdinal("CursoCuatrimestreId")),
                CursoId = reader.GetInt32(reader.GetOrdinal("CursoId")),
                CuatrimestreId = reader.GetInt32(reader.GetOrdinal("CuatrimestreId")),
                Ind_Estado = reader.GetString(reader.GetOrdinal("Ind_Estado")),
                Fec_Registro = reader.GetDateTime(reader.GetOrdinal("Fec_Registro")),
                Curso = new Curso
                {
                    CursoId = reader.GetInt32(reader.GetOrdinal("CursoId")),
                    Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                    Nom_Curso = reader.GetString(reader.GetOrdinal("Nom_Curso")),
                    Desc_Curso = reader.IsDBNull(reader.GetOrdinal("Desc_Curso")) ? null : reader.GetString(reader.GetOrdinal("Desc_Curso")),
                    Num_Creditos = reader.GetInt32(reader.GetOrdinal("Num_Creditos"))
                },
                Cuatrimestre = new Cuatrimestre
                {
                    CuatrimestreId = reader.GetInt32(reader.GetOrdinal("CuatrimestreId")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Anio = reader.GetInt32(reader.GetOrdinal("Anio")),
                    Numero = reader.GetInt32(reader.GetOrdinal("Numero")),
                    Fec_Inicio = reader.GetDateTime(reader.GetOrdinal("Fec_Inicio")),
                    Fec_Fin = reader.GetDateTime(reader.GetOrdinal("Fec_Fin"))
                }
            };
        }

        private Cuatrimestre MapCuatrimestreFromReader(SqlDataReader reader)
        {
            return new Cuatrimestre
            {
                CuatrimestreId = reader.GetInt32(reader.GetOrdinal("CuatrimestreId")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Anio = reader.GetInt32(reader.GetOrdinal("Anio")),
                Numero = reader.GetInt32(reader.GetOrdinal("Numero")),
                Fec_Inicio = reader.GetDateTime(reader.GetOrdinal("Fec_Inicio")),
                Fec_Fin = reader.GetDateTime(reader.GetOrdinal("Fec_Fin")),
                Ind_Estado = reader.GetString(reader.GetOrdinal("Ind_Estado"))
            };
        }

        private Curso MapCursoFromReader(SqlDataReader reader)
        {
            return new Curso
            {
                CursoId = reader.GetInt32(reader.GetOrdinal("CursoId")),
                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                Nom_Curso = reader.GetString(reader.GetOrdinal("Nom_Curso")),
                Desc_Curso = reader.IsDBNull(reader.GetOrdinal("Desc_Curso")) ? null : reader.GetString(reader.GetOrdinal("Desc_Curso")),
                Num_Creditos = reader.GetInt32(reader.GetOrdinal("Num_Creditos")),
                Ind_Estado = reader.GetString(reader.GetOrdinal("Ind_Estado"))
            };
        }

        private Docente MapDocenteFromReader(SqlDataReader reader)
        {
            return new Docente
            {
                DocenteId = reader.GetInt32(reader.GetOrdinal("DocenteId")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Apellidos = reader.GetString(reader.GetOrdinal("Apellidos")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Ind_Estado = reader.GetString(reader.GetOrdinal("Ind_Estado"))
            };
        }
    }
}
