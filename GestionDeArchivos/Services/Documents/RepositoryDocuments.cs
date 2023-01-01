using Dapper;
using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GestionDeArchivos.Services.Documents
{
    public class RepositoryDocuments : IRepositoryDocuments
    {
        private readonly string connectioString;
        private readonly DataContext _context;

        public RepositoryDocuments(IConfiguration configuration, DataContext context)
        {
            connectioString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public async Task<bool> UpdateStatus(int id, string userName)
        {
            using var connection = new SqlConnection(connectioString);
            Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (userName));
            var update = await connection.ExecuteAsync(@"UPDATE Documents set DocumentStatus = 'Aprobado', UserRecibes = '"+user.Correo+"'WHERE Id = "+id+";");
            return update == 1;

        }
    }
}
