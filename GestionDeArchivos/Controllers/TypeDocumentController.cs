using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using static GestionDeArchivos.Helpers.ModalHelper;
using GestionDeArchivos.Helpers;
using Vereyon.Web;

namespace TypeDocument.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class TypeDocumentController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public TypeDocumentController(IFlashMessage flashMessage, DataContext context)
        {
            _flashMessage = flashMessage;
            _context = context;

        }

        // GET: TypeDocuments
        public async Task<IActionResult> Index()
        {
            return _context.DocumentTypes != null ?
                        View(await _context.DocumentTypes.Include(x => x.Documents).ToListAsync()) :
                        Problem("Entity set 'DataContext.Usuarios'  is null.");
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new DocumentType());
            }
            else
            {
                DocumentType documentType = await _context.DocumentTypes.FindAsync(id);
                if (documentType == null)
                {
                    return NotFound();
                }
                return View(documentType);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, DocumentType documentType)
        {
            if (documentType == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _flashMessage.Confirmation("Se inserto correctamente el tipo de documento. ");
                        _context.Add(documentType);
                        await _context.SaveChangesAsync();
                    }
                    else //Update
                    {
                        _flashMessage.Confirmation("Se actualizo correctamente el tipo de documento. ");
                        _context.Update(documentType);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un tipo de documento con el mismo nombre. ");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(documentType);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(documentType);
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll",
                _context.DocumentTypes.ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", documentType) });
        }

        // GET: TypeDocuements/Delete/5
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            DocumentType documentType = await _context.DocumentTypes.FirstOrDefaultAsync(dt => dt.Id == id);
            if (documentType == null)
            {
                return NotFound();
            }
            try
            {
                _context.DocumentTypes.Remove(documentType);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Tipo de documento eliminado correctamente. ");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el tipo de documento porque tiene registros relacionados. ");
            }
            return RedirectToAction(nameof(Index));
        }
        private bool TypeDocuemtnExists(int id)
        {
            return (_context.DocumentTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
