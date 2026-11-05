using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Data;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && await _userManager.IsInRoleAsync(user, "Administrador"))
                        return RedirectToAction("Index", "Home");

                    return RedirectToLocal(returnUrl ?? "/Home/PortalEstudiante");
                }
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            }
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Carreras = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar que la cédula no esté registrada
                if (await _context.Estudiantes.AnyAsync(e => e.Cedula == model.Cedula))
                {
                    ModelState.AddModelError("Cedula", "Ya existe un estudiante con esa cédula.");
                    ViewBag.Carreras = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre");
                    return View(model);
                }

                var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Estudiante");

                    // Crear registro de estudiante vinculado
                    var estudiante = new Estudiante
                    {
                        NombreCompleto = model.NombreCompleto,
                        Cedula = model.Cedula,
                        Correo = model.Email,
                        CarreraId = model.CarreraId,
                        UserId = user.Id
                    };
                    _context.Estudiantes.Add(estudiante);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("PortalEstudiante", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Carreras = new SelectList(await _context.Carreras.ToListAsync(), "Id", "Nombre");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccesoDenegado
        public IActionResult AccesoDenegado() => View();

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
