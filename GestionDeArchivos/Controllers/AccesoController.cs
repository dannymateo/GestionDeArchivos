using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using GestionDeArchivos.Helpers;
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
        private readonly UserHelper _helper;

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Usuario _usuario)
        {
            var usuario = _helper.ValidarUsuario(_usuario.Correo, _usuario.Clave);

            if (usuario != null)
            {

                //2.- CONFIGURACION DE LA AUTENTICACION
                #region AUTENTICACTION
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Correo),
                    new Claim("Correo", usuario.Correo)
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

        public AccesoController(DataContext context, IFlashMessage flashMessage, UserHelper helper)
        {
            _context = context;
            _flashMessage = flashMessage;
            _helper = helper;

        }
    }
}
