using GestionDeArchivos.Data.Entities;

namespace GestionDeArchivos.Helpers
{
    public interface IUserHelper
    {
        Usuario ValidarUsuario(string _correo, string _clave);
    }
}
