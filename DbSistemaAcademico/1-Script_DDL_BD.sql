-- =============================================
-- Sistema Académico - Base de Datos
-- Proyecto #1 - Programación Avanzada en Web
-- =============================================

-- Crear la base de datos
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'DBSistemaAcademico')
BEGIN
    ALTER DATABASE DBSistemaAcademico SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DBSistemaAcademico;
END
GO

CREATE DATABASE DBSistemaAcademico;
GO

USE DBSistemaAcademico;
GO

-- =============================================
-- TABLAS DE UBICACIÓN GEOGRÁFICA
-- =============================================

-- Tabla: PROVINCIAS
CREATE TABLE Provincia (
    ProvinciaId INT IDENTITY(1,1) PRIMARY KEY,
    Nom_Provincia NVARCHAR(100) NOT NULL UNIQUE,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
);

-- Tabla: CANTONES
CREATE TABLE Canton (
    CantonId INT IDENTITY(1,1) PRIMARY KEY,
    Nom_Canton NVARCHAR(100) NOT NULL,
    ProvinciaId INT NOT NULL,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT FK_Canton_Provincia FOREIGN KEY (ProvinciaId) 
        REFERENCES Provincia(ProvinciaId)
);

-- Tabla: DISTRITOS
CREATE TABLE Distrito (
    DistritoId INT IDENTITY(1,1) PRIMARY KEY,
    Nom_Distrito NVARCHAR(100) NOT NULL,
    CantonId INT NOT NULL,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT FK_Distrito_Canton FOREIGN KEY (CantonId) 
        REFERENCES Canton(CantonId)
);

-- =============================================
-- TABLA DE USUARIOS
-- =============================================

CREATE TABLE Usuarios(
	Id nvarchar(450) NOT NULL,
	UserName nvarchar(256) NULL,
	NormalizedUserName nvarchar(256) NULL,
	Email nvarchar(256) NULL,
	NormalizedEmail nvarchar(256) NULL,
	EmailConfirmed bit NOT NULL,
	PasswordHash nvarchar(max) NULL,
	SecurityStamp nvarchar(max) NULL,
	ConcurrencyStamp nvarchar(max) NULL,
	PhoneNumber nvarchar(max) NULL,
	PhoneNumberConfirmed bit NOT NULL,
	TwoFactorEnabled bit NOT NULL,
	LockoutEnd datetimeoffset(7) NULL,
	LockoutEnabled bit NOT NULL,
	AccessFailedCount int NOT NULL,
									
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	Id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- =============================================
-- TABLA DE ROLES
-- =============================================

CREATE TABLE Roles(
    Id nvarchar(450) NOT NULL,
    Name nvarchar(256) NOT NULL,
	NormalizedName nvarchar(256) NULL,
	ConcurrencyStamp nvarchar(max) NULL,	 
 CONSTRAINT [PK_dbo.Roles] PRIMARY KEY CLUSTERED 
(
    Id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


-- =============================================
-- TABLA DE RELACION USUARIO Y ROLES
-- =============================================

CREATE TABLE UsuarioRoles(
    UserId nvarchar(450) NOT NULL,
    RoleId nvarchar(450) NOT NULL,
    CONSTRAINT PK_UsuarioRoles PRIMARY KEY NONCLUSTERED 
    (
        UserId ASC,
        RoleId ASC
    )
);
GO

ALTER TABLE UsuarioRoles ADD CONSTRAINT FK_UsuarioRoles_Usuarios
    FOREIGN KEY (UserId) REFERENCES Usuarios(Id) ON DELETE CASCADE;

ALTER TABLE UsuarioRoles ADD CONSTRAINT FK_UsuarioRoles_Roles
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE;
GO
-- =============================================
-- TABLA DE USUARIOS LOGIN
-- ============================================= 

CREATE TABLE UsuarioLogins(
	LoginProvider nvarchar(450) NOT NULL,
	ProviderKey nvarchar(450) NOT NULL,
	ProviderDisplayName nvarchar (max) NULL,
	UserId nvarchar(450) NOT NULL,
CONSTRAINT [PK_dbo.UsuarioLogins] PRIMARY KEY CLUSTERED 
(
    [LoginProvider] ASC,
    [ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE UsuarioLogins  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioLogins_dbo.Usuarios_UserId] FOREIGN KEY(UserId)
REFERENCES Usuarios (Id)
ON DELETE CASCADE
GO

ALTER TABLE UsuarioLogins CHECK CONSTRAINT [FK_dbo.UsuarioLogins_dbo.Usuarios_UserId]
GO
 

 

CREATE TABLE UsuarioClaims(
	Id int IDENTITY(1,1) NOT NULL,
	UserId nvarchar(450) NOT NULL,
	ClaimType nvarchar(max) NULL,
	ClaimValue nvarchar(max) NULL,
 CONSTRAINT [PK_dbo.UsuarioClaims] PRIMARY KEY CLUSTERED 
(
	Id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE UsuarioClaims  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioClaims_dbo.Usuarios_UserId] FOREIGN KEY(UserId)
REFERENCES Usuarios (Id)
ON DELETE CASCADE
GO

ALTER TABLE UsuarioClaims CHECK CONSTRAINT [FK_dbo.UsuarioClaims_dbo.Usuarios_UserId]
GO

 
CREATE TABLE [dbo].[RoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_RoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[RoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[RoleClaims] CHECK CONSTRAINT [FK_RoleClaims_Roles_RoleId]
GO


-- =============================================
-- TABLA DE DOCENTES
-- =============================================

CREATE TABLE Docente (
    DocenteId INT IDENTITY(1,1) PRIMARY KEY, 
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    CONSTRAINT FK_Docente_Usuario FOREIGN KEY (UserId) 
        REFERENCES Usuarios(Id)
);

-- =============================================
-- TABLA DE ESTUDIANTES
-- =============================================

CREATE TABLE Estudiante (
    EstudianteId INT IDENTITY(1,1) PRIMARY KEY,
    Identificacion NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Fec_Nacimiento DATE NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    DistritoId INT NOT NULL,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT FK_Estudiante_Distrito FOREIGN KEY (DistritoId) 
        REFERENCES Distrito(DistritoId)
);

-- =============================================
-- TABLAS ACADÉMICAS
-- =============================================

-- Tabla: CUATRIMESTRES
CREATE TABLE Cuatrimestre (
    CuatrimestreId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL,
    Anio INT NOT NULL,
    Numero INT NOT NULL CHECK (Numero BETWEEN 1 AND 3),
    Fec_Inicio DATE NOT NULL,
    Fec_Fin DATE NOT NULL,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT UQ_Cuatrimestre_Anio_Numero UNIQUE (Anio, Numero)
);

-- Tabla: CURSOS
CREATE TABLE Curso (
    CursoId INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(20) NOT NULL UNIQUE,
    Nom_Curso NVARCHAR(200) NOT NULL,
    Desc_Curso NVARCHAR(500) NULL,
    Num_Creditos INT NOT NULL DEFAULT 3,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
);


-- Tabla: CURSO_CUATRIMESTRE (Relación muchos a muchos)
CREATE TABLE CursoCuatrimestre (
    CursoCuatrimestreId INT IDENTITY(1,1) PRIMARY KEY,
    CursoId INT NOT NULL,
    CuatrimestreId INT NOT NULL,
    --DocenteId INT NOT NULL,
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT FK_CursoCuatrimestre_Curso FOREIGN KEY (CursoId) 
        REFERENCES Curso(CursoId),
    CONSTRAINT FK_CursoCuatrimestre_Cuatrimestre FOREIGN KEY (CuatrimestreId) 
        REFERENCES Cuatrimestre(CuatrimestreId),
    --CONSTRAINT FK_CursoCuatrimestre_Docente FOREIGN KEY (DocenteId)
    --    REFERENCES Docente(DocenteId),
    CONSTRAINT UQ_CursoCuatrimestre UNIQUE (CursoId, CuatrimestreId/*,DocenteId*/)
);

-- Tabla: CURSO_CUATRIMESTRE_DOCENTE (Relación muchos a muchos)
CREATE TABLE CursoCuatrimestreDocente(
	CursoCuatriDocenteId int IDENTITY(1,1) NOT NULL, 
	CursoCuatrimestreId int not null,
	DocenteId int NOT NULL,
	Ind_Estado nvarchar(2) NOT NULL,
	Fec_Registro datetime NOT NULL,
PRIMARY KEY CLUSTERED 
(
	CursoCuatriDocenteId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_CursoCuatrimestreDocente] UNIQUE NONCLUSTERED 
(
	CursoCuatrimestreId ASC,
	DocenteId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE CursoCuatrimestreDocente ADD  DEFAULT ('A') FOR Ind_Estado
GO

ALTER TABLE CursoCuatrimestreDocente ADD  DEFAULT (getdate()) FOR Fec_Registro
GO

ALTER TABLE CursoCuatrimestreDocente  WITH CHECK ADD  CONSTRAINT FK_CursoCuatrimestreDocente_CursoCuatrimestre FOREIGN KEY(CursoCuatrimestreId)
REFERENCES .CursoCuatrimestre (CursoCuatrimestreId)
GO

ALTER TABLE CursoCuatrimestreDocente CHECK CONSTRAINT FK_CursoCuatrimestreDocente_CursoCuatrimestre
GO

ALTER TABLE CursoCuatrimestreDocente  WITH CHECK ADD  CONSTRAINT [FK_CursoCuatrimestreDocente_Docente] FOREIGN KEY(DocenteId)
REFERENCES Docente (DocenteId)
GO

ALTER TABLE CursoCuatrimestreDocente CHECK CONSTRAINT [FK_CursoCuatrimestreDocente_Docente]
GO


-- Tabla: ESTUDIANTE_CURSO (Matrícula)
CREATE TABLE EstudianteCurso (
    EstudianteCursoId INT IDENTITY(1,1) PRIMARY KEY,
    EstudianteId INT NOT NULL,
    CursoCuatrimestreId INT NOT NULL,
    Fec_Matricula DATETIME DEFAULT GETDATE(),
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT FK_EstudianteCurso_Estudiante FOREIGN KEY (EstudianteId) 
        REFERENCES Estudiante(EstudianteId),
    CONSTRAINT FK_EstudianteCurso_CursoCuatrimestre FOREIGN KEY (CursoCuatrimestreId) 
        REFERENCES CursoCuatrimestre(CursoCuatrimestreId),
    CONSTRAINT UQ_EstudianteCurso UNIQUE (EstudianteId, CursoCuatrimestreId)
);

-- =============================================
-- TABLA DE EVALUACIONES
-- =============================================

CREATE TABLE Evaluacion (
    EvaluacionId INT IDENTITY(1,1) PRIMARY KEY,
    EstudianteCursoId INT NOT NULL,
    DocenteId INT NOT NULL,
    Nota DECIMAL(5,2) NOT NULL CHECK (Nota BETWEEN 0 AND 100),
    Observaciones NVARCHAR(1000) NULL,
    TipoParticipacion NVARCHAR(50) NOT NULL 
        CHECK (TipoParticipacion IN ('Excelente', 'Buena', 'Regular', 'Baja', 'Ninguna')),
    Estado NVARCHAR(20) NOT NULL 
        CHECK (Estado IN ('Aprobado', 'Reprobado', 'En Proceso')),
    Fec_Evaluacion DATETIME DEFAULT GETDATE(),
    Ind_Estado NVARCHAR(2) DEFAULT 'A' NOT NULL,
    Fec_Registro DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT FK_Evaluaciones_EstudianteCurso FOREIGN KEY (EstudianteCursoId) 
        REFERENCES EstudianteCurso(EstudianteCursoId),
    CONSTRAINT FK_Evaluaciones_Docentes FOREIGN KEY (DocenteId) 
        REFERENCES Docente(DocenteId)
); 
