using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vereyon.Web;

namespace GestionDeArchivos.Controllers
{
    public class AccesoController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Usuario _usuario)
        {
            var usuario = ValidarUsuario(_usuario.Correo, _usuario.Clave);

            if (usuario != null)
            {

                //2.- CONFIGURACION DE LA AUTENTICACION
                #region AUTENTICACTION
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Correo", usuario.Correo),
                };
                claims.Add(new Claim(ClaimTypes.Role, usuario.Roles));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                #endregion


                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }

        }
        public async Task<IActionResult> Salir()
        {
            //3.- CONFIGURACION DE LA AUTENTICACION
            #region AUTENTICACTION
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            #endregion

            return RedirectToAction("Index");

        }
        public IActionResult NotAuthorized()
        {
            return View();
        }
        //______________________________________________________________

        public AccesoController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;

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
