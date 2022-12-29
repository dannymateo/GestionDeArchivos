using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using GestionDeArchivos.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using static GestionDeArchivos.Helpers.ModalHelper;

namespace GestionDeArchivos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdvisorController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public AdvisorController(IFlashMessage flashMessage, DataContext context)
        {
            _flashMessage = flashMessage;
            _context = context;

        }

        // GET: Advisors
        public async Task<IActionResult> Index()
        {
            return _context.Advisors != null ?
                        View(await _context.Advisors.Include(x => x.Documents).ToListAsync()) :
                        Problem("Entity set 'DataContext.Usuarios'  is null.");
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Advisor());
            }
            else
            {
                Advisor advisor = await _context.Advisors.FindAsync(id);
                if (advisor == null)
                {
                    return NotFound();
                }
                return View(advisor);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Advisor advisors)
        {
            if (advisors == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _flashMessage.Confirmation("Se inserto correctamente el asesor. ");
                        _context.Add(advisors);
                        await _context.SaveChangesAsync();
                    }
                    else //Update
                    {
                        _flashMessage.Confirmation("Se actualizo correctamente el asesor. ");
                        _context.Update(advisors);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un asesor con el mismo nombre. ");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(advisors);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(advisors);
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll",
                _context.Advisors.ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", advisors) });
        }

        // GET: Advisors/Delete/5
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Advisor advisors = await _context.Advisors.FirstOrDefaultAsync(a => a.Id == id);
            if (advisors == null)
            {
                return NotFound();
            }
            bool existeDocumento = _context.Documents
                         .Any(d => d.Advisor.Id.CompareTo(advisors.Id) == 0);
            try
            {
                if (existeDocumento)
                {
                    _flashMessage.Danger("No se puede borrar el asesor porque tiene registros relacionados. ");
                }
                else
                {
                    _context.Advisors.Remove(advisors);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Área eliminada correctamente. ");
                }
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el asesor porque tiene registros relacionados. ");
            }
            return RedirectToAction(nameof(Index));
        }
        private bool AdvisorsExists(int id)
        {
            return (_context.Advisors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}