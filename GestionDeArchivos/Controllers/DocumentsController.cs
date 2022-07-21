using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using GestionDeArchivos.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GestionDeArchivos.Helpers.ModalHelper;

namespace GestionDeArchivos.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly DataContext _context;
        private readonly GetAreasHelper _getAreasHelper;

        public DocumentsController(DataContext context, GetAreasHelper getAreasHelper)
        {
            _context = context;
            _getAreasHelper = getAreasHelper;
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
            ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
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
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == document.Location);
                    document.User = user.Correo;
                    document.Areas = area;
                    document.Usuario = user;
                    _context.Add(document);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Create));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un documento con este nombre. ");
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
            ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
            return View(document);
        }
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
                Document document = new()
                {
                    Date = DateTime.Now,
                    DocumentStatus = "Aprobar"
                };
                return View(document);
            }
            else
            {
                Document document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return NotFound();
                }
                ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
                return View(document);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Document document)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                        Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == document.Location);
                        document.User = user.Correo;
                        document.Areas = area;
                        document.Usuario = user;
                        _context.Add(document);
                        await _context.SaveChangesAsync();;
                    }
                    else //Update
                    {
                        Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                        Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == document.Location);
                        document.UserRecibes = user.Correo;
                        document.Areas = area;
                        _context.Update(document);
                        await _context.SaveChangesAsync();
                    }
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
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll",
                _context.Areas.ToList())
                });
            }
            ViewBag.items = _getAreasHelper.GetAreasAsync().Result;
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", document) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Document document = await _context.Documents.FirstOrDefaultAsync(a => a.Id == id);
            try
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError(string.Empty,"No se puede borrar la documento porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(Index));
        }
        //____________________________________________________________________________________________
        private bool DocumentExists(int id)
        {
            return (_context.Documents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

