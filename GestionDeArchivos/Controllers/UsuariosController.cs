using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using static GestionDeArchivos.Helpers.ModalHelper;
using GestionDeArchivos.Helpers;

namespace GestionDeArchivos.Controllers
{

    public class UsuariosController : Controller
    {
        private readonly DataContext _context;
        private readonly IGetAreasHelper _getAreasHelper;

        public UsuariosController(DataContext context, IGetAreasHelper getAreasHelper)
        {
            _context = context;
            _getAreasHelper = getAreasHelper;
        }

        // GET: Usuarios

        [Authorize(Roles = "Administrador")]

        public async Task<IActionResult> Index()
        {
            return _context.Usuarios != null ?
                        View(await _context.Usuarios.Include(x => x.Documents).ToListAsync()) :
                        Problem("Entity set 'DataContext.Usuarios'  is null.");
        }
        [Authorize(Roles = "Administrador,Usuario")]
        public async Task<IActionResult> DocumentsUser()
        {
            return _context.Documents != null ?
                        View(await _context.Documents.Where(u => u.User == (User.Identity.Name)).Where(d => d.DocumentStatus == "Revisar").ToListAsync()) :
                        Problem("Entity set 'DataContext.Usuarios'  is null.");
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Usuario());
            }
            else
            {
                Usuario usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }
                return View(usuario);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Usuario usuario)
        {
            if (usuario == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(usuario);
                        await _context.SaveChangesAsync();
                    }
                    else //Update
                    {
                        _context.Update(usuario);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un usuario con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, (dbUpdateException.InnerException.Message));
                    }
                    return View(usuario);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, (exception.Message));
                    return View(usuario);
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll",
                _context.Usuarios.ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", usuario) });
        }
        // GET: Usuarios/Delete/5
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Usuario usuario = await _context.Usuarios.FirstOrDefaultAsync(a => a.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "No se puede borrar el usuario porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(Index));
        }
        [NoDirectAccess]
        public async Task<IActionResult> EditDocument(int id)
        {
            Document document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
                document.DocumentStatus = "Aprobar";
                document.Date = DateTime.Now;
                ; return View(document);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDocument(int id, Document document)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == document.Location);
                    if (area == null || user == null)
                    {
                        return NotFound();
                    }
                    document.UserRecibes = "";
                    document.Areas = area;
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(document.Name, "Ya existe un documento con este nombre. ");
                    }
                    else
                    {
                        ModelState.AddModelError(document.Name, dbUpdateException.InnerException.Message);
                    }
                    ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
                    return View(document);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(document.Name, exception.Message);
                    return View(document);
                }
                ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "DocumentsUser",
                _context.Areas.ToList())
                });
            }
            ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditDocument", document) });
        }

        [NoDirectAccess]
        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
