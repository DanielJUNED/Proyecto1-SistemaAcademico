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
    public class CursoCuatrimestreDocenteDB
    {
        private readonly string _connectionString;

        public CursoCuatrimestreDocenteDB(string connectionString)
        {
            _connectionString = connectionString;
        }
         

        public async Task<CursoCuatrimestreDocente> ObtenerPorId(int id)
        {
            CursoCuatrimestreDocente entity = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT ccd.* 
                    FROM CursoCuatrimestreDocente ccd 
                    WHERE ccd.CursoCuatriDocenteId = @Id AND ccd.Ind_Estado = 'A'";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            entity = MapFromReader(reader);
                        }
                    }
                }
            }
            return entity;
        }


        // Métodos auxiliares de mapeo 
        private CursoCuatrimestreDocente MapFromReader(SqlDataReader reader)
        {
            return new CursoCuatrimestreDocente
            {
                CursoCuatriDocenteId = reader.GetInt32(reader.GetOrdinal("CursoCuatriDocenteId")),
                CursoCuatrimestreId = reader.GetInt32(reader.GetOrdinal("CursoCuatrimestreId")),
                DocenteId = reader.GetInt32(reader.GetOrdinal("DocenteId")),
                Ind_Estado = reader.GetString(reader.GetOrdinal("Ind_Estado")),
                Fec_Registro = reader.GetDateTime(reader.GetOrdinal("Fec_Registro"))
            };
        }
    }
}
