using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .ToListAsync();
            return View(cursos);
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // GET: Cursos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CarreraId = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre");
            ViewBag.DocenteId = new SelectList(await _context.Docentes.Where(d => d.Activo).ToListAsync(), "Id", "NombreCompleto");
            return View();
        }

        // POST: Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Creditos,CuposDisponibles,Periodo,CarreraId,DocenteId")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Curso creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CarreraId = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre", curso.CarreraId);
            ViewBag.DocenteId = new SelectList(await _context.Docentes.ToListAsync(), "Id", "NombreCompleto", curso.DocenteId);
            return View(curso);
        }

        // GET: Cursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            ViewBag.CarreraId = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre", curso.CarreraId);
            ViewBag.DocenteId = new SelectList(await _context.Docentes.ToListAsync(), "Id", "NombreCompleto", curso.DocenteId);
            return View(curso);
        }

        // POST: Cursos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Creditos,CuposDisponibles,Periodo,CarreraId,DocenteId")] Curso curso)
        {
            if (id != curso.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Curso actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Cursos.AnyAsync(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CarreraId = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre", curso.CarreraId);
            ViewBag.DocenteId = new SelectList(await _context.Docentes.ToListAsync(), "Id", "NombreCompleto", curso.DocenteId);
            return View(curso);
        }

        // GET: Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Curso eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
