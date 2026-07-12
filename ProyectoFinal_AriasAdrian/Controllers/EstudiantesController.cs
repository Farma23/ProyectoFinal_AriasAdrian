using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstudiantesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstudiantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() =>
            View(await _context.Estudiantes.Include(e => e.Carrera).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.Matriculas).ThenInclude(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estudiante == null) return NotFound();
            return View(estudiante);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null) return NotFound();
            ViewBag.CarreraId = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre", estudiante.CarreraId);
            return View(estudiante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Cedula,Correo,FechaIngreso,Activo,CarreraId,UserId")] Estudiante estudiante)
        {
            if (id != estudiante.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(estudiante);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Estudiante actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CarreraId = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre", estudiante.CarreraId);
            return View(estudiante);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var estudiante = await _context.Estudiantes.Include(e => e.Carrera).FirstOrDefaultAsync(m => m.Id == id);
            if (estudiante == null) return NotFound();
            return View(estudiante);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante != null)
            {
                _context.Estudiantes.Remove(estudiante);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Estudiante eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
