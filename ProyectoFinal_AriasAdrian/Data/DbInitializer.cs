using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_AriasAdrian.Models;

namespace ProyectoFinal_AriasAdrian.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Aplicar migraciones pendientes automáticamente
            await context.Database.MigrateAsync();

            // Crear roles si no existen
            string[] roles = { "Administrador", "Estudiante" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Crear usuario administrador por defecto
            var adminEmail = "admin@matricula.cr";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Administrador");
            }

            // Datos de prueba: Carreras
            if (!await context.Carreras.AnyAsync())
            {
                context.Carreras.AddRange(
                    new Carrera { Codigo = "ING-SIS", Nombre = "Ingeniería en Sistemas de Computación", Semestres = 8, CreditosTotales = 148, Descripcion = "Carrera orientada al desarrollo de software y sistemas computacionales." },
                    new Carrera { Codigo = "ADM-EMP", Nombre = "Administración de Empresas", Semestres = 8, CreditosTotales = 132, Descripcion = "Carrera enfocada en gestión y dirección de organizaciones." },
                    new Carrera { Codigo = "CNT-PUB", Nombre = "Contaduría Pública", Semestres = 8, CreditosTotales = 130, Descripcion = "Carrera orientada a las finanzas y contabilidad empresarial." }
                );
                await context.SaveChangesAsync();
            }

            // Datos de prueba: Docentes
            if (!await context.Docentes.AnyAsync())
            {
                context.Docentes.AddRange(
                    new Docente { NombreCompleto = "Lenin Antonio Ortega Jiménez", Correo = "lortega@fidelitas.cr", Especialidad = "Programación Avanzada" },
                    new Docente { NombreCompleto = "Laura Mora Solís", Correo = "lmora@fidelitas.cr", Especialidad = "Bases de Datos" },
                    new Docente { NombreCompleto = "Carlos Ríos Campos", Correo = "crios@fidelitas.cr", Especialidad = "Matemáticas" }
                );
                await context.SaveChangesAsync();
            }

            // Datos de prueba: Cursos
            if (!await context.Cursos.AnyAsync())
            {
                var carreraSistemas = await context.Carreras.FirstOrDefaultAsync(c => c.Codigo == "ING-SIS");
                var docente1 = await context.Docentes.FirstOrDefaultAsync(d => d.Correo == "lortega@fidelitas.cr");
                var docente2 = await context.Docentes.FirstOrDefaultAsync(d => d.Correo == "lmora@fidelitas.cr");
                var docente3 = await context.Docentes.FirstOrDefaultAsync(d => d.Correo == "crios@fidelitas.cr");

                if (carreraSistemas != null)
                {
                    context.Cursos.AddRange(
                        new Curso { Codigo = "CI-101", Nombre = "Programación I", Creditos = 4, CuposDisponibles = 30, CarreraId = carreraSistemas.Id, DocenteId = docente1?.Id },
                        new Curso { Codigo = "CI-102", Nombre = "Fundamentos de Bases de Datos", Creditos = 3, CuposDisponibles = 30, CarreraId = carreraSistemas.Id, DocenteId = docente2?.Id },
                        new Curso { Codigo = "MA-110", Nombre = "Cálculo Diferencial", Creditos = 4, CuposDisponibles = 35, CarreraId = carreraSistemas.Id, DocenteId = docente3?.Id }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
