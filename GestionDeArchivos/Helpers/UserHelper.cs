using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;

namespace GestionDeArchivos.Helpers
{
    public class UserHelper
    {
        private readonly DataContext _context;
        public UserHelper(DataContext context)
        {
            _context = context;
        }
        public List<Usuario> ListaUsuario()
        {
            List<Usuario> list = _context.Usuarios.Select(u => new Usuario
            {
                Correo = u.Correo,
                Clave = u.Clave,
                Nombre = u.Nombre,
                Roles = u.Roles
            }).ToList();
            return list;

        }

        public Usuario ValidarUsuario(string _correo, string _clave)
        {
            ListaUsuario().Where(item => item.Correo == _correo && item.Clave == _clave).FirstOrDefault();
            Usuario usuario = ListaUsuario().Where(item => item.Correo == _correo && item.Clave == _clave).FirstOrDefault();

            return usuario;

        }
    }
}
