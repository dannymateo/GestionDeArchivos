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
        private readonly IGetAdvisorHelper _getAdvisorsHelper;
        public DocumentsController(DataContext context, IFlashMessage flashMessage, IGetAreasHelper getAreasHelper, IGetTypeDocuementsHelper getTypeDocuementsHelper, IGetAdvisorHelper getAdvisorsHelper)
        {
            _flashMessage = flashMessage;
            _context = context;
            _getAreasHelper = getAreasHelper;
            _getTypeDocuementsHelper = getTypeDocuementsHelper;
            _getAdvisorsHelper = getAdvisorsHelper;
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return _context.Documents != null ?
                        View(await _context.Documents.Include(d => d.TypeDocument).Include(d => d.Location).Include(d => d.User).ToListAsync()) :
                        Problem("Entity set 'DataContext.Documents'  is null.");
        }

        [Authorize(Roles = "Administrador,Usuario")]
        public async Task<IActionResult> ListDocuments()
        {
            return _context.Documents != null ?
                        View(await _context.Documents.Include(d => d.TypeDocument).Include(d => d.Location).Include(d => d.User).Where(d => d.User.Correo == User.Identity.Name).ToListAsync()) :
                        Problem("Entity set 'DataContext.Documents'  is null.");
        }
        [Authorize(Roles = "Administrador,Usuario")]
        public async Task<IActionResult> DocumentsUserReview()
        {
            Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
            return _context.Documents != null ?
                        View(await _context.Documents.Include(d => d.TypeDocument).Include(d => d.Location).Include(d => d.User).Where(d => d.DocumentStatus == "Revisar" && d.User == user).ToListAsync()) :
                        Problem("Entity set 'DataContext.Usuarios'  is null.");
        }


        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id)
        {
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
            ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;


            if (id == 0)
            {
                AddDocumentViewModel document = new()
                {
                    DocumentStatus = "Aprobar"
                };
                return View(document);
            }
            else
            {
                Document document = await _context.Documents.Include(d => d.TypeDocument).Include(d => d.Location).Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
                if (document == null)
                {
                    return NotFound();
                }
                AddDocumentViewModel documentViewModel = new()
                {
                    Id = document.Id,
                    Areas = document.Location.Name,
                    Date = document.Date,
                    DocumentStatus = document.DocumentStatus,
                    Name = document.Name,
                    Remark = document.Remark,
                    TypeDocuments = document.TypeDocument.Name
                };
                return View(documentViewModel);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, AddDocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == model.Areas);
                    DocumentType documentType = await _context.DocumentTypes.FirstOrDefaultAsync(a => a.Name == model.TypeDocuments);
                    if (user == null || area == null || documentType == null)
                    {
                        return NotFound();
                    }
                    if (id == 0) //Insert
                    {
                        Document document = new()
                        {
                            DocumentStatus = model.DocumentStatus,
                            User = user,
                            Location = area,
                            Date = DateTime.Today,
                            Name = model.Name,
                            Remark = model.Remark,
                            TypeDocument = documentType,
                        };
                        _context.Add(document);
                        _flashMessage.Confirmation("Se inserto" +
                            " correctamente el documento. ");
                        await _context.SaveChangesAsync();
                    }
                    else //Update
                    {
                        Document document = new()
                        {
                            Id = model.Id,
                            DocumentStatus = model.DocumentStatus,
                            Location = area,
                            Date = model.Date,
                            Name = model.Name,
                            Remark = model.Remark,
                            TypeDocument = documentType,
                            UserRecibes = user.Correo

                        };
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
                        _flashMessage.Danger(model.Name, dbUpdateException.InnerException.Message);
                    }
                    ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
                    ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
                    ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;

                    return View(model);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(model.Name, exception.Message);
                    return View(model);
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll",
                _context.Areas.ToList())
                });
            }
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
            ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", model) });
        }
        [NoDirectAccess]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
            ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;

            Document document = await _context.Documents.Include(d => d.TypeDocument).Include(d => d.Location).Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
                if (document == null)
                {
                    return NotFound();
                }
                AddDocumentViewModel documentViewModel = new()
                {
                    Id = document.Id,
                    Areas = document.Location.Name,
                    Date = document.Date,
                    DocumentStatus = document.DocumentStatus,
                    Name = document.Name,
                    Remark = document.Remark,
                    TypeDocuments = document.TypeDocument.Name
                };
                return View(documentViewModel);
 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddDocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == model.Areas);
                    DocumentType documentType = await _context.DocumentTypes.FirstOrDefaultAsync(a => a.Name == model.TypeDocuments);
                    if (user == null || area == null || documentType == null)
                    {
                        return NotFound();
                    }

                        Document document = new()
                        {
                            Id = model.Id,
                            DocumentStatus = model.DocumentStatus,
                            Location = area,
                            Date = model.Date,
                            Name = model.Name,
                            Remark = model.Remark,
                            TypeDocument = documentType,
                            UserRecibes = null

                        };
                        _context.Update(document);
                        _flashMessage.Confirmation("Se actualizo correctamente el documento. ");
                        await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un documento con este nombre. ");
                    }
                    else
                    {
                        _flashMessage.Danger(model.Name, dbUpdateException.InnerException.Message);
                    }
                    ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
                    ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
                    ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;

                    return View(model);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(model.Name, exception.Message);
                    return View(model);
                }
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll",
                _context.Areas.ToList())
                });
            }
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
            ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", model) });
        }
        [Authorize(Roles = "Administrador,Usuario")]

        public async Task<IActionResult> Create()
        {
            ViewBag.itemsAreas = _getAreasHelper.GetAreasAsync().Result;
            ViewBag.itemsTypeDocuments = _getTypeDocuementsHelper.GetTypeDocuementsAsync().Result;
            ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;
            AddDocumentViewModel document = new()
            {
                DocumentStatus = "Aprobar"
            };
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddDocumentViewModel model)
        {
            if (ModelState.IsValid)
                try
                {
                    Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == (User.Identity.Name));
                    Areas area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == model.Areas);
                    DocumentType documentType = await _context.DocumentTypes.FirstOrDefaultAsync(a => a.Name == model.TypeDocuments);
                    if (user == null || area == null || documentType == null)
                    {
                        return NotFound();
                    }
                    Document document = new()
                    {
                        DocumentStatus = model.DocumentStatus,
                        User = user,
                        Location = area,
                        Date = DateTime.Today,
                        Name = model.Name,
                        Remark = model.Remark,
                        TypeDocument = documentType,
                    };

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
            ViewBag.itemsAdvisors = _getAdvisorsHelper.GetAdvisorsAsync().Result;

            return View(model);
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

