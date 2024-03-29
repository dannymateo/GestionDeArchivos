﻿using GestionDeArchivos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeArchivos.Controllers
{
    public class DashBoardController : Controller
    {
        // GET: DashBoardController
        private readonly DataContext _context;

        public DashBoardController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UsersCount = _context.Usuarios.Count();
            ViewBag.DocumentsAprobados = _context.Documents.Where(d => d.DocumentStatus == "Aprobado").Count();
            ViewBag.DocumentsAprobadosPorMi = _context.Documents.Where(d => d.DocumentStatus == "Aprobado" && d.UserRecibes == (User.Identity.Name)).Count();
            ViewBag.DocumentsRevisar = _context.Documents.Where(d => d.DocumentStatus == "Revisar").Count();
            ViewBag.DocumentsRevisadosPorMi = _context.Documents.Where(d => d.DocumentStatus == "Revisar" && d.UserRecibes == (User.Identity.Name)).Count();
            ViewBag.DocumentsAprobar = _context.Documents.Where(d => d.DocumentStatus == "Aprobar").Count();

            return View(await _context.Documents.Where(d => d.DocumentStatus == "Aprobar").Take(10).ToListAsync());
        }
    }
}
