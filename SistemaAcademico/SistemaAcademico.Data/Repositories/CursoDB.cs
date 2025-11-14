using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaAcademico.Data.Entities;

namespace SistemaAcademico.Data.Repositories
{
    public class CursoDB
    {
        private readonly string _connectionString;

        public CursoDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool>  Crear(Curso curso)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                INSERT INTO Curso (Codigo, Nom_Curso, Desc_Curso, Num_Creditos)
                VALUES (@Codigo, @Nombre, @Descripcion, @Creditos)", conn);

                cmd.Parameters.AddWithValue("@Codigo", curso.Codigo);
                cmd.Parameters.AddWithValue("@Nombre", curso.Nom_Curso);
                cmd.Parameters.AddWithValue("@Descripcion", curso.Desc_Curso ?? "");
                cmd.Parameters.AddWithValue("@Creditos", curso.Num_Creditos);

                conn.Open();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> Actualizar(Curso curso)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                UPDATE Curso 
                SET Nom_Curso = @Nombre, 
                    Desc_Curso = @Descripcion, 
                    Num_Creditos = @Creditos
                WHERE CursoId = @CursoId", conn);

                cmd.Parameters.AddWithValue("@CursoId", curso.CursoId);
                cmd.Parameters.AddWithValue("@Nombre", curso.Nom_Curso);
                cmd.Parameters.AddWithValue("@Descripcion", curso.Desc_Curso ?? "");
                cmd.Parameters.AddWithValue("@Creditos", curso.Num_Creditos);

                conn.Open();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> Eliminar(int cursoId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // Verificar si tiene estudiantes inscritos
                var cmdVerificar = new SqlCommand(@"
                SELECT COUNT(*) FROM EstudianteCurso ec
                INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                WHERE cc.CursoId = @CursoId", conn);
                cmdVerificar.Parameters.AddWithValue("@CursoId", cursoId);

                conn.Open();
                int count = (int)await cmdVerificar.ExecuteNonQueryAsync();

                if (count > 0)
                    return false; // No se puede eliminar

                var cmd = new SqlCommand("UPDATE Curso SET Ind_Estado = 'I' WHERE CursoId = @CursoId", conn);
                cmd.Parameters.AddWithValue("@CursoId", cursoId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> AsignarDocente(int cursoId, int cuatrimestreId, int docenteId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM CursoCuatrimestre 
                               WHERE CursoId = @CursoId 
                               AND CuatrimestreId = @CuatrimestreId 
                               AND DocenteId = @DocenteId)
                BEGIN
                    INSERT INTO CursoCuatrimestre (CursoId, CuatrimestreId, DocenteId)
                    VALUES (@CursoId, @CuatrimestreId, @DocenteId)
                END", conn);

                cmd.Parameters.AddWithValue("@CursoId", cursoId);
                cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId);
                cmd.Parameters.AddWithValue("@DocenteId", docenteId);

                conn.Open();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<List<Curso>> ObtenerTodos()
        {
            var cursos = new List<Curso>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                SELECT CursoId, Codigo, Nom_Curso, Desc_Curso, Num_Creditos, Ind_Estado
                FROM Curso WHERE Ind_Estado = 'A'", conn);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    cursos.Add(new Curso
                    {
                        CursoId = reader.GetInt32(0),
                        Codigo = reader.GetString(1),
                        Nom_Curso = reader.GetString(2),
                        Desc_Curso = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Num_Creditos = reader.GetInt32(4),
                        Ind_Estado = reader.GetString(5)
                    });
                }
            }
            return cursos;
        }
        public async Task<Curso?> ObtenerPorId(int id)
        {
            Curso? curso = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
            SELECT CursoId, Codigo, Nom_Curso, Desc_Curso, Num_Creditos, Ind_Estado
            FROM Curso
            WHERE CursoId = @CursoId AND Ind_Estado = 'A';
        ", conn);

                cmd.Parameters.AddWithValue("@CursoId", id);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    curso = new Curso
                    {
                        CursoId = reader.GetInt32(0),
                        Codigo = reader.GetString(1),
                        Nom_Curso = reader.GetString(2),
                        Desc_Curso = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Num_Creditos = reader.GetInt32(4),
                        Ind_Estado = reader.GetString(5)
                    };
                }
            }

            return curso; // Si no existe, regresa null
        }

    }
}
