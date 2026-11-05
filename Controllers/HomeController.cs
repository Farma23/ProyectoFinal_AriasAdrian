using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using System.Diagnostics;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Administrador"))
                    return RedirectToAction("PanelAdmin");
                return RedirectToAction("PortalEstudiante");
            }
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PanelAdmin()
        {
            ViewBag.TotalEstudiantes = await _context.Estudiantes.CountAsync();
            ViewBag.TotalCursos = await _context.Cursos.CountAsync();
            ViewBag.TotalDocentes = await _context.Docentes.CountAsync();
            ViewBag.TotalMatriculas = await _context.Matriculas.CountAsync();
            return View();
        }

        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> PortalEstudiante()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.Matriculas)
                    .ThenInclude(m => m.Curso)
                        .ThenInclude(c => c!.Docente)
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            return View(estudiante);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
