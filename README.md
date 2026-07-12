# Sistema de Matrícula Universitaria — Avance 2

**Estudiante:** Adrián José Arias Vargas  
**Curso:** Programación Avanzada  
**Profesor:** Lenin Antonio Ortega Jiménez  
**Universidad:** Fidélitas  

---

## Requisitos previos

- Visual Studio 2022 (o superior)
- .NET 8 SDK
- SQL Server 2019+ o SQL Server Express con LocalDB
- SQL Server Management Studio (opcional, para revisar la BD)

---

## Pasos para ejecutar el proyecto

### 1. Abrir la solución

Abra el archivo `ProyectoFinal_AriasAdrian.sln` en Visual Studio.

### 2. Restaurar paquetes NuGet

Visual Studio los restaura automáticamente. Si no, haga clic derecho en la solución → **Restore NuGet Packages**.

### 3. Configurar la cadena de conexión (si es necesario)

En `appsettings.json` la cadena de conexión por defecto usa **LocalDB**:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MatriculaUniversitariaDB;Trusted_Connection=True;"
```

Si usa SQL Server instalado, cámbiela por:

```json
"DefaultConnection": "Server=localhost;Database=MatriculaUniversitariaDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 4. Crear la base de datos con migraciones

En Visual Studio abra la **Consola del Administrador de Paquetes** (Herramientas → NuGet → Consola del Administrador de Paquetes) y ejecute:

```powershell
Add-Migration InitialCreate
Update-Database
```

Esto crea todas las tablas (incluyendo las de Identity) y carga los datos de prueba automáticamente.

> **Alternativa:** puede ejecutar el script SQL en `/BaseDatos/MatriculaUniversitariaDB.sql` en SSMS para crear las tablas de negocio. Las tablas de Identity se crean sólo con `Update-Database`.

### 5. Ejecutar el proyecto

Presione **F5** o el botón ▶️ en Visual Studio. El navegador abrirá automáticamente.

---

## Credenciales de prueba

| Rol           | Correo                  | Contraseña |
|---------------|-------------------------|------------|
| Administrador | admin@matricula.cr      | Admin123!  |
| Estudiante    | Regístrese en /Account/Register |    |

---

## Estructura del proyecto

```
ProyectoFinal_AriasAdrian/
├── Controllers/
│   ├── AccountController.cs      ← Login, Registro, Logout
│   ├── HomeController.cs         ← Inicio, PanelAdmin, PortalEstudiante
│   ├── CarrerasController.cs     ← CRUD Carreras (Admin)
│   ├── CursosController.cs       ← CRUD Cursos (Admin)
│   ├── DocentesController.cs     ← CRUD Docentes (Admin)
│   └── EstudiantesController.cs  ← Gestión Estudiantes (Admin)
├── Models/
│   ├── Carrera.cs
│   ├── Curso.cs
│   ├── Docente.cs
│   ├── Estudiante.cs
│   ├── Matricula.cs
│   └── AccountViewModels.cs
├── Data/
│   ├── ApplicationDbContext.cs   ← EF Core + Identity
│   └── DbInitializer.cs          ← Seed de roles y datos de prueba
├── Views/                        ← Vistas Razor MVC
└── wwwroot/                      ← Archivos estáticos
```

## Funcionalidades implementadas (Avance 2)

- ✅ Registro y autenticación con ASP.NET Identity
- ✅ Roles: Administrador y Estudiante
- ✅ CRUD completo de Carreras
- ✅ CRUD completo de Cursos
- ✅ CRUD completo de Docentes
- ✅ Gestión de Estudiantes (Admin)
- ✅ Portal del Estudiante (ver cursos matriculados)
- ✅ Panel de Administración con métricas
- ✅ Seed automático de datos de prueba
