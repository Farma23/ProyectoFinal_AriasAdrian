using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    // Index y Details son visibles para cualquier usuario autenticado
    // (el estudiante necesita poder explorar las carreras); las acciones
    // de escritura (Create/Edit/Delete) se restringen a Administrador.
    [Authorize]
    public class CarrerasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrerasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carreras
        // Soporta búsqueda por nombre/código y paginación (6 por página,
        // ya que se muestran como tarjetas en 3 columnas).
        public async Task<IActionResult> Index(string? buscar, int pagina = 1)
        {
            const int tamanoPagina = 6;

            var query = _context.Carreras.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(c => c.Nombre.Contains(buscar) || c.Codigo.Contains(buscar));

            query = query.OrderBy(c => c.Nombre);

            ViewBag.Buscar = buscar;

            var carrerasPaginadas = await PaginatedList<Carrera>.CreateAsync(query, pagina, tamanoPagina);

            ViewData["PageIndex"] = carrerasPaginadas.PageIndex;
            ViewData["TotalPages"] = carrerasPaginadas.TotalPages;
            ViewData["RouteValues"] = new Dictionary<string, string?>
            {
                ["buscar"] = buscar
            };

            return View(carrerasPaginadas);
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Create() => View();

        // POST: Carreras/Create
        [HttpPost]
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera == null) return NotFound();
            return View(carrera);
        }

        // POST: Carreras/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var carrera = await _context.Carreras.FirstOrDefaultAsync(m => m.Id == id);
            if (carrera == null) return NotFound();
            return View(carrera);
        }

        // POST: Carreras/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
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
