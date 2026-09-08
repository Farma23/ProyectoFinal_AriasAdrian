using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    // Controlador encargado del flujo de matrícula del estudiante.
    // Las acciones Matricular y Cancelar están pensadas para ser
    // consumidas vía AJAX (fetch) y devuelven JSON en lugar de vistas.
    [Authorize(Roles = "Estudiante")]
    public class MatriculaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Obtiene el registro de Estudiante ligado al usuario de Identity autenticado.
        private async Task<Estudiante?> GetEstudianteActualAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            return await _context.Estudiantes.FirstOrDefaultAsync(e => e.UserId == user.Id);
        }

        // GET: /Matricula/Disponibles
        // Lista los cursos de la carrera del estudiante que aún tienen cupo
        // y en los que no está matriculado activamente. Permite filtrar
        // por período y por búsqueda de nombre/código (funcionalidad extendida).
        public async Task<IActionResult> Disponibles(string? periodo, string? buscar)
        {
            var estudiante = await GetEstudianteActualAsync();
            if (estudiante == null)
            {
                TempData["Error"] = "No se encontró su perfil de estudiante.";
                return RedirectToAction("PortalEstudiante", "Home");
            }

            var idsMatriculados = await _context.Matriculas
                .Where(m => m.EstudianteId == estudiante.Id && m.Estado == EstadoMatricula.Activa)
                .Select(m => m.CursoId)
                .ToListAsync();

            var query = _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .Where(c => c.CarreraId == estudiante.CarreraId && !idsMatriculados.Contains(c.Id))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(periodo))
                query = query.Where(c => c.Periodo == periodo);

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(c => c.Nombre.Contains(buscar) || c.Codigo.Contains(buscar));

            ViewBag.Periodos = await _context.Cursos
                .Where(c => c.CarreraId == estudiante.CarreraId)
                .Select(c => c.Periodo)
                .Distinct()
                .ToListAsync();
            ViewBag.PeriodoSeleccionado = periodo;
            ViewBag.Buscar = buscar;

            var cursos = await query.OrderBy(c => c.Codigo).ToListAsync();
            return View(cursos);
        }

        // POST: /Matricula/Matricular
        // Endpoint AJAX: recibe el id del curso, valida cupos y duplicados,
        // crea la matrícula y descuenta un cupo. Devuelve JSON.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Matricular(int cursoId)
        {
            var estudiante = await GetEstudianteActualAsync();
            if (estudiante == null)
                return Json(new { exito = false, mensaje = "No se encontró su perfil de estudiante." });

            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == cursoId);
            if (curso == null)
                return Json(new { exito = false, mensaje = "El curso no existe." });

            if (curso.CarreraId != estudiante.CarreraId)
                return Json(new { exito = false, mensaje = "Ese curso no pertenece a su carrera." });

            bool yaMatriculado = await _context.Matriculas.AnyAsync(m =>
                m.EstudianteId == estudiante.Id &&
                m.CursoId == cursoId &&
                m.Estado == EstadoMatricula.Activa);
            if (yaMatriculado)
                return Json(new { exito = false, mensaje = "Ya está matriculado en este curso." });

            if (curso.CuposDisponibles <= 0)
                return Json(new { exito = false, mensaje = "No hay cupos disponibles para este curso." });

            var matricula = new Matricula
            {
                EstudianteId = estudiante.Id,
                CursoId = cursoId,
                Estado = EstadoMatricula.Activa,
                FechaMatricula = DateTime.Today
            };

            curso.CuposDisponibles -= 1;
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            return Json(new
            {
                exito = true,
                mensaje = $"Se matriculó exitosamente en {curso.Codigo} - {curso.Nombre}.",
                cuposRestantes = curso.CuposDisponibles,
                matriculaId = matricula.Id
            });
        }

        // POST: /Matricula/Cancelar
        // Endpoint AJAX: cancela una matrícula activa del estudiante autenticado
        // y devuelve el cupo al curso correspondiente.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int matriculaId)
        {
            var estudiante = await GetEstudianteActualAsync();
            if (estudiante == null)
                return Json(new { exito = false, mensaje = "No se encontró su perfil de estudiante." });

            var matricula = await _context.Matriculas
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == matriculaId && m.EstudianteId == estudiante.Id);

            if (matricula == null)
                return Json(new { exito = false, mensaje = "La matrícula no existe o no le pertenece." });

            if (matricula.Estado != EstadoMatricula.Activa)
                return Json(new { exito = false, mensaje = "Esta matrícula ya no está activa." });

            matricula.Estado = EstadoMatricula.Cancelada;
            if (matricula.Curso != null)
                matricula.Curso.CuposDisponibles += 1;

            await _context.SaveChangesAsync();

            return Json(new
            {
                exito = true,
                mensaje = "Matrícula cancelada exitosamente.",
                matriculaId = matricula.Id
            });
        }
    }
}
