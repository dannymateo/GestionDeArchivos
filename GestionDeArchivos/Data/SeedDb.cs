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
                _context.Areas.Add(new Areas { Name = "Funeraria" });
                _context.Areas.Add(new Areas { Name = "Contabilidad" });
                _context.Areas.Add(new Areas { Name = "Gerencia" });
            }
            await _context.SaveChangesAsync();
        }
        private async Task CheckDocumentsAsync()
        {
            if (!_context.Documents.Any())
            {
                _context.Documents.Add(new Document { Name = "Documento 1", Location = "Base de Datos", Date = DateTime.Now, DocumentStatus = "Aprobado", User = "Admin@gmail.com" });
                _context.Documents.Add(new Document { Name = "Documento 2", Location = "Base de Datos", Date = DateTime.Now, DocumentStatus = "Aprobado", User = "Admin@gmail.com" });
                _context.Documents.Add(new Document { Name = "Documento 3", Location = "Base de Datos", Date = DateTime.Now, DocumentStatus = "Aprobado", User = "User@gmail.com" });
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