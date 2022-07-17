using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;

namespace GestionDeArchivos.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;
        public DocumentsController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return _context.Documents != null ?
                        View(await _context.Documents.ToListAsync()) :
                        Problem("Entity set 'DataContext.Documents'  is null.");
        }

        [Authorize(Roles = "Administrador,Usuario")]

        public async Task<IActionResult> CreateAsync()
        {
            ViewBag.items = GetAreasAsync().Result;
            Document document = new()
            {
                Date = DateTime.Now,
                DocumentStatus = "Aprobar"
            };
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document document)
        {
            if (ModelState.IsValid)
                try
                {
                    Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.name == document.remarks);
                    document.User = user.Correo;
                    document.AreaId = area.id;
                    document.UsuarioId = user.Id;
                    _context.Add(document);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Guardado exitoso.");
                    return RedirectToAction(nameof(Create));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un Documento con este mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            ViewBag.items = GetAreasAsync().Result;
            return View(document);
        }
        [Authorize(Roles = "Administrador")]

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.items = GetAreasAsync().Result;
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Document document)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.name == document.remarks);
                    document.UserRecibes = user.Correo;
                    document.AreaId = area.id;
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                    _flashMessage.Warning("Documento editado");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un Documento con este mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            ViewBag.items = GetAreasAsync().Result;
            return View(document);
        }
        [Authorize(Roles = "Administrador")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .FirstOrDefaultAsync(m => m.id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Documents == null)
            {
                return Problem("Entity set 'DataContext.Documents'  is null.");
            }
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
            }

            await _context.SaveChangesAsync();
            _flashMessage.Danger("Documento eliminado");
            return RedirectToAction(nameof(Index));
        }
        //____________________________________________________________________________________________
        private bool DocumentExists(int id)
        {
            return (_context.Documents?.Any(e => e.id == id)).GetValueOrDefault();
        }
        private async Task<IEnumerable<SelectListItem>> GetAreasAsync()
        {
            List<Areas> list = await _context.Areas.Select(a => new Areas
            {
                id = a.id,
                name = a.name
            })
                .OrderBy(a => a.name)
                .ToListAsync();
            List<SelectListItem> items = list.ConvertAll(l =>
            {
                return new SelectListItem()
                {
                    Text = l.name,
                    Value = l.name,
                    Selected = false
                };
            });
            return items;
        }
    }
}

