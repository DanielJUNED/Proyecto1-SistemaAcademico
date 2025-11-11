using SistemaAcademico.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SistemaAcademico.Data
{
    public class EvaluacionDB
    {
        private readonly string _connectionString;

        public EvaluacionDB()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public EvaluacionDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        // =============================================
        // BUSCAR ESTUDIANTES
        // =============================================
        public async Task<List<BusquedaEstudianteViewModel>> BuscarEstudiantesAsync(string criterio)
        {
            var resultado = new List<BusquedaEstudianteViewModel>();
            var criterioBusqueda = criterio.Trim().ToLower();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT TOP 10
                        e.EstudianteId,
                        e.Identificacion,
                        e.Nombre,
                        e.Apellidos,
                        e.Email,
                        e.Fec_Nacimiento,
                        d.Nom_Distrito,
                        c.Nom_Canton,
                        p.Nom_Provincia
                    FROM Estudiante e
                    INNER JOIN Distrito d ON e.DistritoId = d.DistritoId
                    INNER JOIN Canton c ON d.CantonId = c.CantonId
                    INNER JOIN Provincia p ON c.ProvinciaId = p.ProvinciaId
                    WHERE e.Ind_Estado = 'A'
                    AND (
                        LOWER(e.Nombre) LIKE '%' + @Criterio + '%'
                        OR LOWER(e.Apellidos) LIKE '%' + @Criterio + '%'
                        OR e.Identificacion LIKE '%' + @Criterio + '%'
                    )
                    ORDER BY e.Nombre, e.Apellidos";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Criterio", criterioBusqueda);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var estudiante = new BusquedaEstudianteViewModel
                            {
                                EstudianteID = reader.GetInt32(0),
                                Identificacion = reader.GetString(1),
                                NombreCompleto = $"{reader.GetString(2)} {reader.GetString(3)}",
                                Email = reader.GetString(4),
                                Fec_Nacimiento = reader.GetDateTime(5),
                                Edad = CalcularEdad(reader.GetDateTime(5)),
                                DireccionCompleta = $"{reader.GetString(6)}, {reader.GetString(7)}, {reader.GetString(8)}",
                                CursosMatriculados = new List<CursoMatriculadoViewModel>()
                            };
                            resultado.Add(estudiante);
                        }
                    }
                }

                // Obtener cursos matriculados para cada estudiante
                foreach (var estudiante in resultado)
                {
                    estudiante.CursosMatriculados = await ObtenerCursosMatriculadosAsync(conn, estudiante.EstudianteID);
                }
            }

            return resultado;
        }

        // =============================================
        // OBTENER CURSOS MATRICULADOS
        // =============================================
        private async Task<List<CursoMatriculadoViewModel>> ObtenerCursosMatriculadosAsync(
            SqlConnection conn,
            int estudianteId,
            int? docenteLogin = null)
        {
            var cursos = new List<CursoMatriculadoViewModel>();

            string query = @"
                SELECT 
                    ec.EstudianteCursoId,
                    cc.CursoId,
                    c.Codigo,
                    c.Nom_Curso,
                    cu.Nombre AS NombreCuatrimestre,
                    cu.CuatrimestreId,
                    c.Num_Creditos,
                    d.DocenteId,
                    d.Nombre + ' ' + d.Apellidos AS NombreDocente,
                    cc.CursoCuatrimestreId,
                    ev.EvaluacionId,
                    ev.Nota,
                    ev.Estado,
                    ev.TipoParticipacion,
                    ev.Observaciones
                FROM EstudianteCurso ec
                INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                INNER JOIN Curso c ON cc.CursoId = c.CursoId
                INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                INNER JOIN Docente d ON cc.DocenteId = d.DocenteId
                LEFT JOIN Evaluacion ev ON ec.EstudianteCursoId = ev.EstudianteCursoId
                WHERE ec.EstudianteId = @EstudianteId
                AND ec.Ind_Estado = 'A'
                ORDER BY cu.Anio DESC, cu.Numero DESC, c.Codigo";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var curso = new CursoMatriculadoViewModel
                        {
                            EstudianteCursoID = reader.GetInt32(0),
                            CursoID = reader.GetInt32(1),
                            CodigoCurso = reader.GetString(2),
                            NombreCurso = reader.GetString(3),
                            NombreCuatrimestre = reader.GetString(4),
                            CuatrimestreID = reader.GetInt32(5),
                            Creditos = reader.GetInt32(6),
                            DocenteId = reader.GetInt32(7),
                            NombreDocente = reader.GetString(8),
                            TieneEvaluacion = !reader.IsDBNull(10),
                            EvaluacionId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                            NotaActual = reader.IsDBNull(11) ? (decimal?)null : reader.GetDecimal(11),
                            EstadoActual = reader.IsDBNull(12) ? null : reader.GetString(12),
                            TipoParticipacion = reader.IsDBNull(13) ? null : reader.GetString(13),
                            Observacion = reader.IsDBNull(14) ? null : reader.GetString(14)
                        };

                        // Si se proporcionó docenteLogin, verificar permisos
                        if (docenteLogin.HasValue)
                        {
                            int cursoCuatrimestreId = reader.GetInt32(9);
                            curso.PermisoEvaluar = VerificarPermisoEvaluar(cursoCuatrimestreId, docenteLogin.Value);
                        }

                        cursos.Add(curso);
                    }
                }
            }

            return cursos;
        }

        // =============================================
        // OBTENER DETALLE DE ESTUDIANTE
        // =============================================
        public async Task<BusquedaEstudianteViewModel> ObtenerEstudianteDetalleAsync(int estudianteId, int docenteLogin)
        {
            BusquedaEstudianteViewModel resultado = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT 
                        e.EstudianteId,
                        e.Identificacion,
                        e.Nombre,
                        e.Apellidos,
                        e.Email,
                        e.Fec_Nacimiento,
                        d.Nom_Distrito,
                        c.Nom_Canton,
                        p.Nom_Provincia
                    FROM Estudiante e
                    INNER JOIN Distrito d ON e.DistritoId = d.DistritoId
                    INNER JOIN Canton c ON d.CantonId = c.CantonId
                    INNER JOIN Provincia p ON c.ProvinciaId = p.ProvinciaId
                    WHERE e.EstudianteId = @EstudianteId
                    AND e.Ind_Estado = 'A'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado = new BusquedaEstudianteViewModel
                            {
                                EstudianteID = reader.GetInt32(0),
                                Identificacion = reader.GetString(1),
                                NombreCompleto = $"{reader.GetString(2)} {reader.GetString(3)}",
                                Email = reader.GetString(4),
                                Fec_Nacimiento = reader.GetDateTime(5),
                                Edad = CalcularEdad(reader.GetDateTime(5)),
                                DireccionCompleta = $"{reader.GetString(6)}, {reader.GetString(7)}, {reader.GetString(8)}",
                                CursosMatriculados = new List<CursoMatriculadoViewModel>()
                            };
                        }
                    }
                }

                if (resultado != null)
                {
                    resultado.CursosMatriculados = await ObtenerCursosMatriculadosAsync(conn, estudianteId, docenteLogin);
                }
            }

            return resultado;
        }

        // =============================================
        // REGISTRAR EVALUACIÓN
        // =============================================
        public async Task<EvaluacionResultViewModel> RegistrarEvaluacionAsync(
            RegistrarEvaluacionViewModel modelo, int docenteId)
        {
            var resultado = new EvaluacionResultViewModel();
            int evaluacionId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Verificar si ya existe evaluación
                        string queryVerificar = @"
                            SELECT COUNT(*) 
                            FROM Evaluacion 
                            WHERE EstudianteCursoId = @EstudianteCursoId";

                        using (SqlCommand cmd = new SqlCommand(queryVerificar, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@EstudianteCursoId", modelo.EstudianteCursoID);
                            int existe = (int)await cmd.ExecuteScalarAsync();

                            if (existe > 0)
                            {
                                resultado.Success = false;
                                resultado.Message = "Ya existe una evaluación registrada para este curso. Use la opción de actualizar.";
                                return resultado;
                            }
                        }

                        // Insertar nueva evaluación
                        string queryInsertar = @"
                            INSERT INTO Evaluacion (
                                EstudianteCursoId, 
                                DocenteId, 
                                Nota, 
                                Observaciones, 
                                TipoParticipacion, 
                                Estado, 
                                Fec_Evaluacion, 
                                Fec_Registro, 
                                Ind_Estado
                            )
                            VALUES (
                                @EstudianteCursoId, 
                                @DocenteId, 
                                @Nota, 
                                @Observaciones, 
                                @TipoParticipacion, 
                                @Estado, 
                                @FechaEvaluacion, 
                                @FechaRegistro, 
                                'A'
                            );
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (SqlCommand cmd = new SqlCommand(queryInsertar, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@EstudianteCursoId", modelo.EstudianteCursoID);
                            cmd.Parameters.AddWithValue("@DocenteId", docenteId);
                            cmd.Parameters.AddWithValue("@Nota", modelo.Nota);
                            cmd.Parameters.AddWithValue("@Observaciones",
                                string.IsNullOrWhiteSpace(modelo.Observaciones) ? (object)DBNull.Value : modelo.Observaciones.Trim());
                            cmd.Parameters.AddWithValue("@TipoParticipacion", modelo.TipoParticipacion);
                            cmd.Parameters.AddWithValue("@Estado", modelo.Estado);
                            cmd.Parameters.AddWithValue("@FechaEvaluacion", DateTime.Now);
                            cmd.Parameters.AddWithValue("@FechaRegistro", DateTime.Now);

                            evaluacionId = (int)await cmd.ExecuteScalarAsync();
                        }

                        transaction.Commit();

                        // Obtener detalle completo
                        var detalle = await ObtenerEvaluacionPorIdAsync(evaluacionId);

                        resultado.Success = true;
                        resultado.Message = "Evaluación registrada exitosamente";
                        resultado.EvaluacionID = evaluacionId;
                        resultado.Evaluacion = detalle;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        resultado.Success = false;
                        resultado.Message = "Error al registrar la evaluación";

                        // Capturar detalles del error
                        var detalle = ex.InnerException?.InnerException?.Message
                                    ?? ex.InnerException?.Message
                                    ?? ex.Message;
                        resultado.Errors.Add(detalle);

                        // Log para depuración
                        Console.WriteLine("Error principal: " + ex.Message);
                        var inner = ex.InnerException;
                        int nivel = 1;
                        while (inner != null)
                        {
                            Console.WriteLine($"Inner Exception {nivel++}: {inner.Message}");
                            inner = inner.InnerException;
                        }
                    }
                }
            }

            return resultado;
        }

        // =============================================
        // ACTUALIZAR EVALUACIÓN
        // =============================================
        public async Task<EvaluacionResultViewModel> ActualizarEvaluacionAsync(ActualizarEvaluacionViewModel modelo)
        {
            var resultado = new EvaluacionResultViewModel();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    // Verificar que existe la evaluación
                    string queryVerificar = "SELECT COUNT(*) FROM Evaluacion WHERE EvaluacionId = @EvaluacionId";

                    using (SqlCommand cmd = new SqlCommand(queryVerificar, conn))
                    {
                        cmd.Parameters.AddWithValue("@EvaluacionId", modelo.EvaluacionID);
                        int existe = (int)await cmd.ExecuteScalarAsync();

                        if (existe == 0)
                        {
                            resultado.Success = false;
                            resultado.Message = "Evaluación no encontrada";
                            return resultado;
                        }
                    }

                    // Actualizar evaluación
                    string queryActualizar = @"
                        UPDATE Evaluacion
                        SET 
                            Nota = @Nota,
                            Observaciones = @Observaciones,
                            TipoParticipacion = @TipoParticipacion,
                            Estado = @Estado
                        WHERE EvaluacionId = @EvaluacionId";

                    using (SqlCommand cmd = new SqlCommand(queryActualizar, conn))
                    {
                        cmd.Parameters.AddWithValue("@EvaluacionId", modelo.EvaluacionID);
                        cmd.Parameters.AddWithValue("@Nota", modelo.Nota);
                        cmd.Parameters.AddWithValue("@Observaciones",
                            string.IsNullOrWhiteSpace(modelo.Observaciones) ? (object)DBNull.Value : modelo.Observaciones.Trim());
                        cmd.Parameters.AddWithValue("@TipoParticipacion", modelo.TipoParticipacion);
                        cmd.Parameters.AddWithValue("@Estado", modelo.Estado);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Obtener detalle actualizado
                    var detalle = await ObtenerEvaluacionPorIdAsync(modelo.EvaluacionID);

                    resultado.Success = true;
                    resultado.Message = "Evaluación actualizada exitosamente";
                    resultado.EvaluacionID = modelo.EvaluacionID;
                    resultado.Evaluacion = detalle;
                }
                catch (Exception ex)
                {
                    resultado.Success = false;
                    resultado.Message = "Error al actualizar la evaluación";
                    resultado.Errors.Add(ex.Message);
                }
            }

            return resultado;
        }

        // =============================================
        // OBTENER EVALUACIÓN POR ID
        // =============================================
        public async Task<EvaluacionDetalleViewModel> ObtenerEvaluacionPorIdAsync(int evaluacionId)
        {
            EvaluacionDetalleViewModel resultado = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT 
                        e.EvaluacionId,
                        est.Nombre + ' ' + est.Apellidos AS NombreEstudiante,
                        est.Identificacion,
                        c.Codigo AS CodigoCurso,
                        c.Nom_Curso,
                        cu.Nombre AS NombreCuatrimestre,
                        e.Nota,
                        e.Observaciones,
                        e.TipoParticipacion,
                        e.Estado,
                        e.Fec_Evaluacion,
                        d.Nombre + ' ' + d.Apellidos AS NombreDocente
                    FROM Evaluacion e
                    INNER JOIN EstudianteCurso ec ON e.EstudianteCursoId = ec.EstudianteCursoId
                    INNER JOIN Estudiante est ON ec.EstudianteId = est.EstudianteId
                    INNER JOIN CursoCuatrimestre cc ON ec.CursoCuatrimestreId = cc.CursoCuatrimestreId
                    INNER JOIN Curso c ON cc.CursoId = c.CursoId
                    INNER JOIN Cuatrimestre cu ON cc.CuatrimestreId = cu.CuatrimestreId
                    INNER JOIN Docente d ON e.DocenteId = d.DocenteId
                    WHERE e.EvaluacionId = @EvaluacionId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EvaluacionId", evaluacionId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado = new EvaluacionDetalleViewModel
                            {
                                EvaluacionID = reader.GetInt32(0),
                                NombreEstudiante = reader.GetString(1),
                                IdentificacionEstudiante = reader.GetString(2),
                                CodigoCurso = reader.GetString(3),
                                NombreCurso = reader.GetString(4),
                                NombreCuatrimestre = reader.GetString(5),
                                Nota = reader.GetDecimal(6),
                                Observaciones = reader.IsDBNull(7) ? null : reader.GetString(7),
                                TipoParticipacion = reader.GetString(8),
                                Estado = reader.GetString(9),
                                FechaEvaluacion = reader.GetDateTime(10),
                                NombreDocente = reader.GetString(11)
                            };
                        }
                    }
                }
            }

            return resultado;
        }

        // =============================================
        // VERIFICAR SI EXISTE EVALUACIÓN
        // =============================================
        public async Task<bool> ExisteEvaluacionAsync(int estudianteCursoId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT COUNT(*) FROM Evaluacion WHERE EstudianteCursoId = @EstudianteCursoId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EstudianteCursoId", estudianteCursoId);
                    int count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        // =============================================
        // OBTENER EVALUACIÓN POR ESTUDIANTE CURSO
        // =============================================
        public async Task<bool> ObtEvaluacionPorAsync(int estudianteCursoId)
        {
            return await ExisteEvaluacionAsync(estudianteCursoId);
        }

        // =============================================
        // VERIFICAR PERMISO PARA EVALUAR
        // =============================================
        public bool VerificarPermisoEvaluar(int cursoCuatrimestreId, int docenteId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT COUNT(*) 
                    FROM CursoCuatrimestre 
                    WHERE CursoCuatrimestreId = @CursoCuatrimestreId 
                    AND DocenteId = @DocenteId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CursoCuatrimestreId", cursoCuatrimestreId);
                    cmd.Parameters.AddWithValue("@DocenteId", docenteId);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // =============================================
        // HELPER: CALCULAR EDAD
        // =============================================
        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }
    }
}