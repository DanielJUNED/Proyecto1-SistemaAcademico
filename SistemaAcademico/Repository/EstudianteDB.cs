using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaAcademico.Repository
{
    public class EstudianteDB
    {
        private readonly string _connectionString;

        public EstudianteDB()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public EstudianteDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        // =============================================
        // REGISTRAR ESTUDIANTE
        // =============================================
        public ResultadoRegistro RegistrarEstudiante(EstudianteViewModel modelo)
        {
            var resultado = new ResultadoRegistro();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Verificar duplicados
                        if (ExisteIdentificacion(modelo.Identificacion, conn, transaction))
                        {
                            resultado.Exitoso = false;
                            resultado.Mensaje = "Ya existe un estudiante con esta identificación";
                            return resultado;
                        }

                        if (ExisteEmail(modelo.Email, conn, transaction))
                        {
                            resultado.Exitoso = false;
                            resultado.Mensaje = "Ya existe un estudiante con este correo electrónico";
                            return resultado;
                        }

                        // 2. Insertar Estudiante
                        string queryEstudiante = @"
                            INSERT INTO Estudiante (
                                Identificacion, 
                                Nombre, 
                                Apellidos, 
                                Fec_Nacimiento, 
                                Email, 
                                DistritoId, 
                                Fec_Registro, 
                                Ind_Estado
                            )
                            VALUES (
                                @Identificacion, 
                                @Nombre, 
                                @Apellidos, 
                                @FechaNacimiento, 
                                @Email, 
                                @DistritoId, 
                                @FechaRegistro, 
                                'A'
                            );
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int estudianteId;
                        using (SqlCommand cmd = new SqlCommand(queryEstudiante, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Identificacion", modelo.Identificacion.Trim());
                            cmd.Parameters.AddWithValue("@Nombre", modelo.Nombre.Trim());
                            cmd.Parameters.AddWithValue("@Apellidos", modelo.Apellidos.Trim());
                            cmd.Parameters.AddWithValue("@FechaNacimiento", modelo.FechaNacimiento);
                            cmd.Parameters.AddWithValue("@Email", modelo.Email.Trim().ToLower());
                            cmd.Parameters.AddWithValue("@DistritoId", modelo.DistritoID);
                            cmd.Parameters.AddWithValue("@FechaRegistro", DateTime.Now);

                            estudianteId = (int)cmd.ExecuteScalar();
                        }

                        // 3. Matricular en los cursos seleccionados
                        foreach (var cursoId in modelo.CursosSeleccionados)
                        {
                            // Buscar el CursoCuatrimestreId
                            string queryCursoCuatrimestre = @"
                                SELECT CursoCuatrimestreId
                                FROM CursoCuatrimestre
                                WHERE CursoId = @CursoId
                                AND CuatrimestreId = @CuatrimestreId
                                AND Ind_Estado = 'A'";

                            int cursoCuatrimestreId = 0;
                            using (SqlCommand cmd = new SqlCommand(queryCursoCuatrimestre, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CursoId", cursoId);
                                cmd.Parameters.AddWithValue("@CuatrimestreId", modelo.CuatrimestreID);

                                var result = cmd.ExecuteScalar();
                                if (result == null)
                                {
                                    throw new Exception($"No se encontró el curso {cursoId} en el cuatrimestre {modelo.CuatrimestreID}");
                                }
                                cursoCuatrimestreId = Convert.ToInt32(result);
                            }

                            // Insertar matrícula
                            string queryMatricula = @"
                                INSERT INTO EstudianteCurso (
                                    EstudianteId, 
                                    CursoCuatrimestreId, 
                                    Fec_Matricula, 
                                    Fec_Registro, 
                                    Ind_Estado
                                )
                                VALUES (
                                    @EstudianteId, 
                                    @CursoCuatrimestreId, 
                                    @FechaMatricula, 
                                    @FechaRegistro, 
                                    'A'
                                )";

                            using (SqlCommand cmd = new SqlCommand(queryMatricula, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                                cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                                cmd.Parameters.AddWithValue("@FechaMatricula", DateTime.Now);
                                cmd.Parameters.AddWithValue("@FechaRegistro", DateTime.Now);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 4. Confirmar transacción
                        transaction.Commit();

                        resultado.Exitoso = true;
                        resultado.Mensaje = "Estudiante registrado exitosamente";
                        resultado.PersonaID = estudianteId;
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        resultado.Exitoso = false;
                        resultado.Mensaje = "Error al guardar en la base de datos";

                        var detalle = ex.InnerException?.Message ?? ex.Message;
                        resultado.Errores.Add(detalle);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        resultado.Exitoso = false;
                        resultado.Mensaje = "Error al registrar el estudiante";
                        resultado.Errores.Add(ex.Message);
                    }
                }
            }

            return resultado;
        }

        // =============================================
        // VERIFICACIONES DE DUPLICADOS
        // =============================================
        public bool ExisteIdentificacion(string identificacion)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return ExisteIdentificacion(identificacion, conn, null);
            }
        }

        private bool ExisteIdentificacion(string identificacion, SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT COUNT(*) FROM Estudiante WHERE Identificacion = @Identificacion";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Identificacion", identificacion);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool ExisteEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return ExisteEmail(email, conn, null);
            }
        }

        private bool ExisteEmail(string email, SqlConnection conn, SqlTransaction transaction)
        {
            var emailNormalizado = email.Trim().ToLower();
            string query = "SELECT COUNT(*) FROM Estudiante WHERE LOWER(Email) = @Email";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Email", emailNormalizado);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // =============================================
        // OBTENER UBICACIÓN GEOGRÁFICA
        // =============================================
        public IEnumerable<Provincia> ObtProvincias()
        {
            var provincias = new List<Provincia>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT ProvinciaId, Nom_Provincia, Ind_Estado, Fec_Registro
                    FROM Provincia
                    ORDER BY Nom_Provincia";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            provincias.Add(new Provincia
                            {
                                ProvinciaId = reader.GetInt32(0),
                                Nom_Provincia = reader.GetString(1),
                                Ind_Estado = reader.GetString(2),
                                Fec_Registro = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }

            return provincias;
        }

        public IEnumerable<Canton> ObtCantonesPorProvincia(int provinciaId)
        {
            var cantones = new List<Canton>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT CantonId, Nom_Canton, ProvinciaId, Ind_Estado, Fec_Registro
                    FROM Canton
                    WHERE ProvinciaId = @ProvinciaId
                    ORDER BY Nom_Canton";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProvinciaId", provinciaId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cantones.Add(new Canton
                            {
                                CantonId = reader.GetInt32(0),
                                Nom_Canton = reader.GetString(1),
                                ProvinciaId = reader.GetInt32(2),
                                Ind_Estado = reader.GetString(3),
                                Fec_Registro = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return cantones;
        }

        public IEnumerable<Distrito> ObtDistritosPorCanton(int cantonId)
        {
            var distritos = new List<Distrito>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT DistritoId, Nom_Distrito, CantonId, Ind_Estado, Fec_Registro
                    FROM Distrito
                    WHERE CantonId = @CantonId
                    ORDER BY Nom_Distrito";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CantonId", cantonId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            distritos.Add(new Distrito
                            {
                                DistritoId = reader.GetInt32(0),
                                Nom_Distrito = reader.GetString(1),
                                CantonId = reader.GetInt32(2),
                                Ind_Estado = reader.GetString(3),
                                Fec_Registro = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return distritos;
        }

        // =============================================
        // OBTENER INFORMACIÓN ACADÉMICA
        // =============================================
        public IEnumerable<Cuatrimestre> ObtCuatrimestresActivos()
        {
            var cuatrimestres = new List<Cuatrimestre>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        CuatrimestreId, 
                        Nombre, 
                        Anio, 
                        Numero, 
                        Fec_Inicio, 
                        Fec_Fin, 
                        Ind_Estado, 
                        Fec_Registro
                    FROM Cuatrimestre
                    WHERE Ind_Estado = 'A'
                    ORDER BY Anio DESC, Numero DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                }
            }

            return cuatrimestres;
        }

        public IEnumerable<Curso> ObtCursosPorCuatrimestre(int cuatrimestreId)
        {
            var cursos = new List<Curso>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT DISTINCT
                        c.CursoId,
                        c.Codigo,
                        c.Nom_Curso,
                        c.Desc_Curso,
                        c.Num_Creditos,
                        c.Ind_Estado,
                        c.Fec_Registro
                    FROM Curso c
                    INNER JOIN CursoCuatrimestre cc ON c.CursoId = cc.CursoId
                    WHERE c.Ind_Estado = 'A'
                    AND cc.CuatrimestreId = @CuatrimestreId
                    AND cc.Ind_Estado = 'A'
                    ORDER BY c.Codigo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                }
            }

            return cursos;
        }

        // =============================================
        // OBTENER ESTUDIANTE COMPLETO
        // =============================================
        public Estudiante ObtenerEstudianteCompleto(int estudianteId)
        {
            Estudiante estudiante = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Obtener datos básicos del estudiante con ubicación
                string queryEstudiante = @"
                    SELECT 
                        e.EstudianteId,
                        e.Identificacion,
                        e.Nombre,
                        e.Apellidos,
                        e.Fec_Nacimiento,
                        e.Email,
                        e.DistritoId,
                        e.Ind_Estado,
                        e.Fec_Registro,
                        d.Nom_Distrito,
                        c.CantonId,
                        c.Nom_Canton,
                        p.ProvinciaId,
                        p.Nom_Provincia
                    FROM Estudiante e
                    INNER JOIN Distrito d ON e.DistritoId = d.DistritoId
                    INNER JOIN Canton c ON d.CantonId = c.CantonId
                    INNER JOIN Provincia p ON c.ProvinciaId = p.ProvinciaId
                    WHERE e.EstudianteId = @EstudianteId";

                using (SqlCommand cmd = new SqlCommand(queryEstudiante, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                                Distrito = new Distrito
                                {
                                    DistritoId = reader.GetInt32(6),
                                    Nom_Distrito = reader.GetString(9),
                                    CantonId = reader.GetInt32(10),
                                    Canton = new Canton
                                    {
                                        CantonId = reader.GetInt32(10),
                                        Nom_Canton = reader.GetString(11),
                                        ProvinciaId = reader.GetInt32(12),
                                        Provincia = new Provincia
                                        {
                                            ProvinciaId = reader.GetInt32(12),
                                            Nom_Provincia = reader.GetString(13)
                                        }
                                    }
                                },
                                EstudianteCurso = new List<EstudianteCurso>()
                            };
                        }
                    }
                }

                // Si se encontró el estudiante, obtener sus cursos matriculados
                if (estudiante != null)
                {
                    string queryCursos = @"
                        SELECT 
                            ec.EstudianteCursoId,
                            ec.EstudianteId,
                            ec.CursoCuatrimestreId,
                            ec.Fec_Matricula,
                            ec.Ind_Estado,
                            c.CursoId,
                            c.Codigo,
                            c.Nom_Curso,
                            c.Num_Creditos,
                            cu.CuatrimestreId,
                            cu.Nombre AS NombreCuatrimestre,
                            cu.Anio
                        FROM EstudianteCurso ec
                        INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                        INNER JOIN Curso c ON cc.CursoId = c.CursoId
                        INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                        WHERE ec.EstudianteId = @EstudianteId";

                    using (SqlCommand cmd = new SqlCommand(queryCursos, conn))
                    {
                        cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var estudianteCurso = new EstudianteCurso
                                {
                                    EstudianteCursoId = reader.GetInt32(0),
                                    EstudianteId = reader.GetInt32(1),
                                    CursoCuatrimestreId = reader.GetInt32(2),
                                    Fec_Matricula = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                    Ind_Estado = reader.GetString(4),
                                    CursoCuatrimestre = new CursoCuatrimestre
                                    {
                                        CursoCuatrimestreId = reader.GetInt32(2),
                                        CursoId = reader.GetInt32(5),
                                        Curso = new Curso
                                        {
                                            CursoId = reader.GetInt32(5),
                                            Codigo = reader.GetString(6),
                                            Nom_Curso = reader.GetString(7),
                                            Num_Creditos = reader.GetInt32(8)
                                        },
                                        CuatrimestreId = reader.GetInt32(9),
                                        Cuatrimestre = new Cuatrimestre
                                        {
                                            CuatrimestreId = reader.GetInt32(9),
                                            Nombre = reader.GetString(10),
                                            Anio = reader.GetInt32(11)
                                        }
                                    }
                                };

                                estudiante.EstudianteCurso.Add(estudianteCurso);
                            }
                        }
                    }
                }
            }

            return estudiante;
        }

        // =============================================
        // BUSCAR ESTUDIANTES
        // =============================================
        public Estudiante BuscarEstudiantes(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return new Estudiante();

            Estudiante estudiante = null;
            var criterioBusqueda = criterio.Trim().ToLower();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT TOP 1
                        EstudianteId,
                        Identificacion,
                        Nombre,
                        Apellidos,
                        Fec_Nacimiento,
                        Email,
                        DistritoId,
                        Ind_Estado,
                        Fec_Registro
                    FROM Estudiante
                    WHERE (
                        LOWER(Nombre) LIKE '%' + @Criterio + '%'
                        OR LOWER(Apellidos) LIKE '%' + @Criterio + '%'
                        OR Identificacion LIKE '%' + @Criterio + '%'
                        OR LOWER(Email) LIKE '%' + @Criterio + '%'
                    )
                    ORDER BY Nombre, Apellidos";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Criterio", criterioBusqueda);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                                Fec_Registro = reader.GetDateTime(8)
                            };
                        }
                    }
                }
            }

            return estudiante ?? new Estudiante();
        }

        // =============================================
        // OBTENER CURSOS MATRICULADOS
        // =============================================
        public List<Curso> ObtCursosMatriculados(int estudianteId, int cuatrimestreId)
        {
            var cursos = new List<Curso>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        c.CursoId,
                        c.Codigo,
                        c.Nom_Curso,
                        c.Desc_Curso,
                        c.Num_Creditos,
                        c.Ind_Estado,
                        c.Fec_Registro
                    FROM EstudianteCurso ec
                    INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                    INNER JOIN Curso c ON cc.CursoId = c.CursoId
                    WHERE ec.EstudianteId = @EstudianteId
                    AND cc.CuatrimestreId = @CuatrimestreId
                    AND ec.Ind_Estado = 'A'
                    ORDER BY c.Codigo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                }
            }

            return cursos;
        }

        // =============================================
        // OBTENER ESTADÍSTICAS DE CUATRIMESTRE
        // =============================================
        public object ObtEstadisticasCuatrimestreAsync(int cuatrimestreId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        COUNT(*) AS TotalMatriculas,
                        COUNT(DISTINCT ec.EstudianteId) AS EstudiantesUnicos,
                        COUNT(DISTINCT cc.CursoId) AS CursosOfrecidos
                    FROM EstudianteCurso ec
                    INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                    WHERE cc.CuatrimestreId = @CuatrimestreId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CuatrimestreId", cuatrimestreId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new
                            {
                                TotalMatriculas = reader.GetInt32(0),
                                EstudiantesUnicos = reader.GetInt32(1),
                                CursosOfrecidos = reader.GetInt32(2)
                            };
                        }
                    }
                }
            }

            return new { TotalMatriculas = 0, EstudiantesUnicos = 0, CursosOfrecidos = 0 };
        }
    }
}