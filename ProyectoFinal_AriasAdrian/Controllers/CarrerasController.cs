using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CarrerasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrerasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carreras
        public async Task<IActionResult> Index()
        {
            return View(await _context.Carreras.ToListAsync());
        }

        // GET: Carreras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var carrera = await _context.Carreras
                .Include(c => c.Cursos)
                .Include(c => c.Estudiantes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrera == null) return NotFound();
            return View(carrera);
        }

        // GET: Carreras/Create
        public IActionResult Create() => View();

        // POST: Carreras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Descripcion,Semestres,CreditosTotales")] Carrera carrera)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carrera);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Carrera creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(carrera);
        }

        // GET: Carreras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera == null) return NotFound();
            return View(carrera);
        }

        // POST: Carreras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Descripcion,Semestres,CreditosTotales")] Carrera carrera)
        {
            if (id != carrera.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrera);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Carrera actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Carreras.AnyAsync(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carrera);
        }

        // GET: Carreras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var carrera = await _context.Carreras.FirstOrDefaultAsync(m => m.Id == id);
            if (carrera == null) return NotFound();
            return View(carrera);
        }

        // POST: Carreras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera != null)
            {
                _context.Carreras.Remove(carrera);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Carrera eliminada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
