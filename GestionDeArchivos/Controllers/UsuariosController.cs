using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Vereyon.Web;
using static GestionDeArchivos.Helpers.ModalHelper;
using GestionDeArchivos.Helpers;

namespace GestionDeArchivos.Controllers
{
    [Authorize(Roles = "Administrador")]

    public class UsuariosController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;
        public UsuariosController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return _context.Usuarios != null ?
                        View(await _context.Usuarios.Include(x => x.Documents).ToListAsync()) :
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
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(usuario);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado.");
                    }
                    else //Update
                    {
                        _context.Update(usuario);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado.");
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un usuario con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(usuario);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
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
            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el usuario porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(Index));
        }
        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
