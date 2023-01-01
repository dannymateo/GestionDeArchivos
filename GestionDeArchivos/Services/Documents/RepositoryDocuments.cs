using Dapper;
using Microsoft.Data.SqlClient;

namespace GestionDeArchivos.Services.Documents
{
    public class RepositoryDocuments : IRepositoryDocuments
    {
        private readonly string connectioString;

        public RepositoryDocuments(IConfiguration configuration)
        {
            connectioString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> UpdateStatus(int id)
        {
            using var connection = new SqlConnection(connectioString);
            var update = await connection.ExecuteAsync(@"UPDATE Documents set DocumentStatus = 'Aprobado'
                                                                           WHERE Id = "+id+";");
            return update == 1;

        }
    }
}
