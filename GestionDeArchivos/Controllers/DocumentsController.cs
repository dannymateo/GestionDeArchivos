using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using GestionDeArchivos.Helpers;
using GestionDeArchivos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using static GestionDeArchivos.Helpers.ModalHelper;

namespace GestionDeArchivos.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;
        private readonly IGetAreasHelper _getAreasHelper;
        private readonly IGetTypeDocuementsHelper _getTypeDocuementsHelper;

        public DocumentsController(DataContext context, IFlashMessage flashMessage, IGetAreasHelper getAreasHelper, IGetTypeDocuementsHelper getTypeDocuementsHelper)
        {
            _flashMessage = flashMessage;
            _context = context;
            _getAreasHelper = getAreasHelper;
            _getTypeDocuementsHelper = getTypeDocuementsHelper;
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return _context.Documents != null ?
                        View(await _context.Documents.ToListAsync()) :
                        Problem("Entity set 'DataContext.Documents'  is null.");
        }

        [Authorize(Roles = "Administrador,Usuario")]

        public async Task<IActionResult> Create()
        {
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
            Document document = new()
            {
                Date = DateTime.Today,
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
                    DocumentType documentType = await _context.DocumentTypes.FirstOrDefaultAsync(a => a.Name == document.TypeDocument);
                    if (user == null || area == null || documentType == null)
                    {
                        return NotFound();
                    }
                    document.User = user.Correo;
                    document.TypeDocuments = documentType;
                    document.Areas = area;
                    document.Usuario = user;
                    _context.Add(document);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Se inserto correctamente el documento. ");
                    return RedirectToAction(nameof(Create));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un documento con este nombre. ");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
            return View(document);
        }
        [Authorize(Roles = "Administrador,Usuario")]
        public async Task<IActionResult> ListDocuments()
        {
            return _context.Documents != null ?
                        View(await _context.Documents.Where(d => d.User == User.Identity.Name).ToListAsync()) :
                        Problem("Entity set 'DataContext.Documents'  is null.");
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;

            if (id == 0)
            {
                Document document = new()
                {
                    Date = DateTime.Today,
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
                    Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == document.Location);
                    DocumentType documentType = await _context.DocumentTypes.FirstOrDefaultAsync(a => a.Name == document.TypeDocument);
                    if (user == null || area == null || documentType == null)
                    {
                        return NotFound();
                    }
                    document.TypeDocuments = documentType;
                    document.Areas = area;
                    if (id == 0) //Insert
                    {
                        document.User = user.Correo;
                        document.Usuario = user;
                        _context.Add(document);
                        _flashMessage.Confirmation("Se inserto correctamente el documento. ");
                        await _context.SaveChangesAsync();;
                    }
                    else //Update
                    {
                        document.UserRecibes = User.Identity.Name;
                        _context.Update(document);
                        _flashMessage.Confirmation("Se actualizo correctamente el documento. ");
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un documento con este nombre. ");
                    }
                    else
                    {
                        _flashMessage.Danger(document.Name, dbUpdateException.InnerException.Message);
                    }
                    ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
                    ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
                    return View(document);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(document.Name, exception.Message);
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
            if (document == null)
            {
                return NotFound();
            }
            try
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Área eliminada correctamente. ");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar la documento porque tiene registros relacionados. ");
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

