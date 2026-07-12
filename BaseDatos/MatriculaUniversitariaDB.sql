-- ================================================================
-- Sistema de Matrícula Universitaria
-- Base de Datos: MatriculaUniversitariaDB
-- Estudiante: Adrián José Arias Vargas
-- Curso: Programación Avanzada - Universidad Fidélitas
-- Avance 2 - Proyecto Final
-- ================================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'MatriculaUniversitariaDB')
BEGIN
    ALTER DATABASE MatriculaUniversitariaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MatriculaUniversitariaDB;
END
GO

CREATE DATABASE MatriculaUniversitariaDB;
GO

USE MatriculaUniversitariaDB;
GO

-- ================================================================
-- NOTA: Las tablas de ASP.NET Identity (AspNetUsers, AspNetRoles,
-- AspNetUserRoles, etc.) son creadas automáticamente por Entity
-- Framework Core al ejecutar las migraciones con:
--   Add-Migration InitialCreate
--   Update-Database
-- en la Consola del Administrador de Paquetes de Visual Studio.
-- ================================================================

-- ================================================================
-- Tabla: Carreras
-- ================================================================
CREATE TABLE Carreras (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Codigo           NVARCHAR(20)  NOT NULL UNIQUE,
    Nombre           NVARCHAR(100) NOT NULL,
    Descripcion      NVARCHAR(300) NULL,
    Semestres        INT           NOT NULL DEFAULT 8,
    CreditosTotales  INT           NOT NULL DEFAULT 130
);
GO

-- ================================================================
-- Tabla: Docentes
-- ================================================================
CREATE TABLE Docentes (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto   NVARCHAR(100) NOT NULL,
    Correo           NVARCHAR(256) NOT NULL,
    Especialidad     NVARCHAR(100) NULL,
    Activo           BIT           NOT NULL DEFAULT 1
);
GO

-- ================================================================
-- Tabla: Estudiantes
-- ================================================================
CREATE TABLE Estudiantes (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto   NVARCHAR(100) NOT NULL,
    Cedula           NVARCHAR(20)  NOT NULL UNIQUE,
    Correo           NVARCHAR(256) NOT NULL,
    FechaIngreso     DATE          NOT NULL DEFAULT GETDATE(),
    Activo           BIT           NOT NULL DEFAULT 1,
    CarreraId        INT           NOT NULL,
    UserId           NVARCHAR(450) NULL,
    CONSTRAINT FK_Estudiantes_Carreras FOREIGN KEY (CarreraId)
        REFERENCES Carreras(Id) ON DELETE CASCADE
);
GO

-- ================================================================
-- Tabla: Cursos
-- ================================================================
CREATE TABLE Cursos (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Codigo           NVARCHAR(20)  NOT NULL UNIQUE,
    Nombre           NVARCHAR(150) NOT NULL,
    Creditos         INT           NOT NULL DEFAULT 3,
    CuposDisponibles INT           NOT NULL DEFAULT 30,
    Periodo          NVARCHAR(20)  NOT NULL DEFAULT '2025-I',
    CarreraId        INT           NOT NULL,
    DocenteId        INT           NULL,
    CONSTRAINT FK_Cursos_Carreras  FOREIGN KEY (CarreraId) REFERENCES Carreras(Id),
    CONSTRAINT FK_Cursos_Docentes  FOREIGN KEY (DocenteId) REFERENCES Docentes(Id) ON DELETE SET NULL
);
GO

-- ================================================================
-- Tabla: Matriculas
-- ================================================================
CREATE TABLE Matriculas (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    FechaMatricula   DATE NOT NULL DEFAULT GETDATE(),
    Estado           INT  NOT NULL DEFAULT 0,   -- 0=Activa, 1=Cancelada, 2=Completada
    EstudianteId     INT  NOT NULL,
    CursoId          INT  NOT NULL,
    CONSTRAINT FK_Matriculas_Estudiantes FOREIGN KEY (EstudianteId)
        REFERENCES Estudiantes(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Matriculas_Cursos FOREIGN KEY (CursoId)
        REFERENCES Cursos(Id) ON DELETE NO ACTION
);
GO

-- ================================================================
-- Datos de prueba: Carreras
-- ================================================================
INSERT INTO Carreras (Codigo, Nombre, Descripcion, Semestres, CreditosTotales) VALUES
('ING-SIS', 'Ingeniería en Sistemas de Computación', 'Carrera orientada al desarrollo de software y sistemas computacionales.', 8, 148),
('ADM-EMP', 'Administración de Empresas', 'Carrera enfocada en gestión y dirección de organizaciones.', 8, 132),
('CNT-PUB', 'Contaduría Pública', 'Carrera orientada a las finanzas y contabilidad empresarial.', 8, 130);
GO

-- ================================================================
-- Datos de prueba: Docentes
-- ================================================================
INSERT INTO Docentes (NombreCompleto, Correo, Especialidad, Activo) VALUES
('Lenin Antonio Ortega Jiménez', 'lortega@fidelitas.cr', 'Programación Avanzada', 1),
('Laura Mora Solís',             'lmora@fidelitas.cr',   'Bases de Datos',        1),
('Carlos Ríos Campos',           'crios@fidelitas.cr',   'Matemáticas',           1);
GO

-- ================================================================
-- Datos de prueba: Cursos
-- ================================================================
INSERT INTO Cursos (Codigo, Nombre, Creditos, CuposDisponibles, Periodo, CarreraId, DocenteId) VALUES
('CI-101', 'Programación I',              4, 30, '2025-I', 1, 1),
('CI-102', 'Fundamentos de Bases de Datos', 3, 30, '2025-I', 1, 2),
('MA-110', 'Cálculo Diferencial',         4, 35, '2025-I', 1, 3);
GO

PRINT 'Base de datos MatriculaUniversitariaDB creada exitosamente.';
PRINT 'IMPORTANTE: Ejecute las migraciones de EF Core para crear las tablas de Identity.';
GO
