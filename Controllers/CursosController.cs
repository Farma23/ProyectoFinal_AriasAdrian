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
        // Soporta filtros por carrera, período, créditos y búsqueda de texto,
        // además de paginación (5 elementos por página).
        public async Task<IActionResult> Index(string? buscar, int? carreraId, string? periodo, int? creditos, int pagina = 1)
        {
            const int tamanoPagina = 5;

            var query = _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(c => c.Nombre.Contains(buscar) || c.Codigo.Contains(buscar));

            if (carreraId.HasValue)
                query = query.Where(c => c.CarreraId == carreraId.Value);

            if (!string.IsNullOrWhiteSpace(periodo))
                query = query.Where(c => c.Periodo == periodo);

            if (creditos.HasValue)
                query = query.Where(c => c.Creditos == creditos.Value);

            query = query.OrderBy(c => c.Codigo);

            ViewBag.Carreras = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre", carreraId);
            ViewBag.Periodos = await _context.Cursos.Select(c => c.Periodo).Distinct().ToListAsync();
            ViewBag.Buscar = buscar;
            ViewBag.CarreraId = carreraId;
            ViewBag.Periodo = periodo;
            ViewBag.Creditos = creditos;

            var cursosPaginados = await PaginatedList<Curso>.CreateAsync(query, pagina, tamanoPagina);

            ViewData["PageIndex"] = cursosPaginados.PageIndex;
            ViewData["TotalPages"] = cursosPaginados.TotalPages;
            ViewData["RouteValues"] = new Dictionary<string, string?>
            {
                ["buscar"] = buscar,
                ["carreraId"] = carreraId?.ToString(),
                ["periodo"] = periodo,
                ["creditos"] = creditos?.ToString()
            };

            return View(cursosPaginados);
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
