using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DocentesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocentesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() =>
            View(await _context.Docentes.Include(d => d.Cursos).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var docente = await _context.Docentes.Include(d => d.Cursos).FirstOrDefaultAsync(m => m.Id == id);
            if (docente == null) return NotFound();
            return View(docente);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreCompleto,Correo,Especialidad,Activo")] Docente docente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Docente registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(docente);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var docente = await _context.Docentes.FindAsync(id);
            if (docente == null) return NotFound();
            return View(docente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Correo,Especialidad,Activo")] Docente docente)
        {
            if (id != docente.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(docente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Docente actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(docente);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var docente = await _context.Docentes.FirstOrDefaultAsync(m => m.Id == id);
            if (docente == null) return NotFound();
            return View(docente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);
            if (docente != null)
            {
                _context.Docentes.Remove(docente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Docente eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
