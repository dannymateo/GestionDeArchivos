using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using static GestionDeArchivos.Helpers.ModalHelper;
using GestionDeArchivos.Helpers;

namespace GestionDeArchivos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AreasController : Controller
    {
        private readonly DataContext _context;

        public AreasController(DataContext context)
        {
            _context = context;
        }

        // GET: Areas
        public async Task<IActionResult> Index()
        {
            return _context.Areas != null ?
                        View(await _context.Areas.Include(x => x.Documents).ToListAsync()) :
                        Problem("Entity set 'DataContext.Usuarios'  is null.");
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Areas());
            }
            else
            {
                Areas areas = await _context.Areas.FindAsync(id);
                if (areas == null)
                {
                    return NotFound();
                }
                return View(areas);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Areas areas)
        {
            if(areas == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(areas);
                        await _context.SaveChangesAsync();
                    }
                    else //Update
                    {
                        _context.Update(areas);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una área con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                    return View(areas);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                    return View(areas);
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll",
                _context.Areas.ToList())
                });
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", areas) });
        }

        // GET: Areas/Delete/5
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Areas areas = await _context.Areas.FirstOrDefaultAsync(a => a.Id == id);
            if (areas == null)
            {
                return NotFound();
            }
            try
            {
                _context.Areas.Remove(areas);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "No se puede borrar el area porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(Index));
        }
        private bool AreasExists(int id)
        {
            return (_context.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
