using GestionDeArchivos.Data.Entities;
namespace GestionDeArchivos.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        public SeedDb(DataContext context)
        {
            _context = context;
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        await CheckUsersAsync();
            await CheckAreasAsync();
            await CheckDocumentsAsync();
        }
        private async Task CheckAreasAsync()
        {
            if (!_context.Areas.Any())
            {
                _context.Areas.Add(new Areas { name = "Funeraria" });
                _context.Areas.Add(new Areas { name = "Contabilidad" });
                _context.Areas.Add(new Areas { name = "Gerencia" });
            }
            await _context.SaveChangesAsync();
        }
        private async Task CheckDocumentsAsync()
        {
            if (!_context.Documents.Any())
            {
                _context.Documents.Add(new Document { name = "Documento 1", remarks = "Base de Datos", Date = DateTime.Now, DocumentStatus = "Aprobado" });
                _context.Documents.Add(new Document { name = "Documento 2", remarks = "Base de Datos", Date = DateTime.Now, DocumentStatus = "Aprobado" });
                _context.Documents.Add(new Document { name = "Documento 3", remarks = "Base de Datos", Date = DateTime.Now, DocumentStatus = "Aprobado" });
            }
            await _context.SaveChangesAsync();
        }
        private async Task CheckUsersAsync()
        {
            if (!_context.Usuarios.Any())
            {
                _context.Usuarios.Add(new Usuario {Nombre = "Admin", Correo = "Admin@gmail.com" , Clave = "123456", Roles = "Administrador" });
                _context.Usuarios.Add(new Usuario { Nombre = "User", Correo = "User@gmail.com", Clave = "123456", Roles = "Usuario" });
            }
            await _context.SaveChangesAsync();
        }
    }
}