using Microsoft.Data.SqlClient;
using SistemaAcademico.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Repositories
{
    public class BitacoraDB
    {
        private readonly string _connectionString;

        public BitacoraDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Registrar(Bitacora bitacora)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                INSERT INTO Bitacora (UserId, Accion, Modulo, Descripcion, DireccionIP, Fec_Accion)
                VALUES (@UserId, @Accion, @Modulo, @Descripcion, @DireccionIP, GETDATE())", conn);

                cmd.Parameters.AddWithValue("@UserId", bitacora.UserId);
                cmd.Parameters.AddWithValue("@Accion", bitacora.Accion);
                cmd.Parameters.AddWithValue("@Modulo", bitacora.Modulo);
                cmd.Parameters.AddWithValue("@Descripcion", bitacora.Descripcion ?? "");
                cmd.Parameters.AddWithValue("@DireccionIP", bitacora.DireccionIP ?? "");

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<Bitacora> ObtenerTodas()
        {
            var bitacoras = new List<Bitacora>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"
                SELECT b.BitacoraId, b.UserId, u.UserName, b.Accion, b.Modulo, 
                       b.Descripcion, b.DireccionIP, b.Fec_Accion
                FROM Bitacora b
                INNER JOIN Usuarios u ON b.UserId = u.Id
                ORDER BY b.Fec_Accion DESC", conn);

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bitacoras.Add(new Bitacora
                    {
                        BitacoraId = reader.GetInt32(0),
                        UserId = reader.GetString(1),
                        Accion = reader.GetString(3),
                        Modulo = reader.GetString(4),
                        Descripcion = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        DireccionIP = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        Fec_Accion = reader.GetDateTime(7)
                    });
                }
            }
            return bitacoras;
        }

        public List<Bitacora> BuscarBitacora(string userId = null, string modulo = null,
                                             DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var bitacoras = new List<Bitacora>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                SELECT b.BitacoraId, b.UserId, u.UserName, b.Accion, b.Modulo, 
                       b.Descripcion, b.DireccionIP, b.Fec_Accion
                FROM Bitacora b
                INNER JOIN Usuarios u ON b.UserId = u.Id
                WHERE 1=1";

                if (!string.IsNullOrEmpty(userId))
                    query += " AND b.UserId = @UserId";
                if (!string.IsNullOrEmpty(modulo))
                    query += " AND b.Modulo LIKE @Modulo";
                if (fechaDesde.HasValue)
                    query += " AND b.Fec_Accion >= @FechaDesde";
                if (fechaHasta.HasValue)
                    query += " AND b.Fec_Accion <= @FechaHasta";

                query += " ORDER BY b.Fec_Accion DESC";

                var cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(userId))
                    cmd.Parameters.AddWithValue("@UserId", userId);
                if (!string.IsNullOrEmpty(modulo))
                    cmd.Parameters.AddWithValue("@Modulo", "%" + modulo + "%");
                if (fechaDesde.HasValue)
                    cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde.Value);
                if (fechaHasta.HasValue)
                    cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta.Value);

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bitacoras.Add(new Bitacora
                    {
                        BitacoraId = reader.GetInt32(0),
                        UserId = reader.GetString(1),
                        Accion = reader.GetString(3),
                        Modulo = reader.GetString(4),
                        Descripcion = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        DireccionIP = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        Fec_Accion = reader.GetDateTime(7)
                    });
                }
            }
            return bitacoras;
        }
    }
}
