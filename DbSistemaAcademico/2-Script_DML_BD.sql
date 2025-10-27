-- ------------------------------
-- Provincias
-- ------------------------------
INSERT INTO Provincia (Nom_Provincia) VALUES 
('San José'),      -- 1
('Alajuela'),      -- 2
('Cartago'),       -- 3
('Heredia'),       -- 4
('Guanacaste'),    -- 5
('Puntarenas'),    -- 6
('Limón');         -- 7

-- ------------------------------
-- Cantones por provincia 
-- ------------------------------

-- San José (ProvinciaId = 1)
INSERT INTO Canton (Nom_Canton, ProvinciaId) VALUES
('San José', 1),
('Escazú', 1),
('Desamparados', 1),
('Puriscal', 1),
('Tarrazú', 1);

-- Alajuela (ProvinciaId = 2)
INSERT INTO Canton (Nom_Canton, ProvinciaId) VALUES
('Alajuela', 2),
('San Ramón', 2),
('Grecia', 2),
('San Carlos', 2),
('Naranjo', 2);

-- Cartago (ProvinciaId = 3)
INSERT INTO Canton (Nom_Canton, ProvinciaId) VALUES
('Cartago', 3),
('Paraiso', 3),
('La Unión', 3),
('Jiménez', 3),
('Turrialba', 3);

-- Heredia (ProvinciaId = 4)
INSERT INTO Canton (Nom_Canton, ProvinciaId) VALUES
('Heredia', 4),
('Barva', 4),
('Santo Domingo', 4),
('Santa Bárbara', 4),
('San Rafael', 4);

-- Guanacaste (ProvinciaId = 5)
INSERT INTO Canton (Nom_Canton, ProvinciaId) VALUES
('Liberia', 5),
('Nicoya', 5),
('Santa Cruz', 5),
('Bagaces', 5),
('Carrillo', 5);

-- Puntarenas (ProvinciaId = 6)
INSERT INTO Canton (Nom_Canton, ProvinciaId) VALUES
('Puntarenas', 6),
('Esparza', 6),
('Buenos Aires', 6),
('Montes de Oro', 6),
('Osa', 6);

-- Limón (ProvinciaId = 7)
INSERT INTO Canton (Nom_Canton, ProvinciaId) VALUES
('Limón', 7),
('Pococí', 7),
('Siquirres', 7),
('Talamanca', 7),
('Matina', 7);

-- ------------------------------
-- Distritos por cantón 
-- ------------------------------

-- Cantones San José (CantonId 1-5)
INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Carmen', 1),
('Merced', 1),
('Hospital', 1);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Escazú Centro', 2),
('San Rafael', 2),
('San Antonio', 2);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Desamparados Centro', 3),
('San Miguel', 3),
('San Juan de Dios', 3);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Puriscal Centro', 4),
('Santiago', 4),
('Mercedes Sur', 4);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Tarrazú Centro', 5),
('San Marcos', 5),
('San Lorenzo', 5);

-- Cantones Alajuela (CantonId 6-10)
INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Alajuela Centro', 6),
('San José', 6),
('San Antonio', 6);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('San Ramón Centro', 7),
('San Rafael', 7),
('Santiago', 7);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Grecia Centro', 8),
('San Isidro', 8),
('San José', 8);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Ciudad Quesada', 9),
('Florencia', 9),
('La Fortuna', 9);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Naranjo Centro', 10),
('San Miguel', 10),
('San José', 10);

-- Cantones Cartago (CantonId 11-15)
INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Oriental', 11),
('Occidental', 11),
('San Nicolás', 11);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Paraiso Centro', 12),
('Santiago', 12),
('Orosi', 12);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Tres Ríos', 13),
('San Diego', 13),
('San Juan', 13);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Juan Viñas', 14),
('Tucurrique', 14),
('Pejibaye', 14);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Turrialba Centro', 15),
('La Suiza', 15),
('Santa Cruz', 15);

-- Cantones Heredia (CantonId 16-20)
INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Heredia Centro', 16),
('Mercedes', 16),
('San Francisco', 16);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Barva Centro', 17),
('San Pedro', 17),
('San Pablo', 17);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Santo Domingo Centro', 18),
('San Vicente', 18),
('Paracito', 18);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Santa Bárbara Centro', 19),
('San Juan', 19),
('Jesús', 19);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('San Rafael Centro', 20),
('San Josecito', 20),
('Santiago', 20);

-- Cantones Guanacaste (CantonId 21-25)
INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Liberia Centro', 21),
('Cañas Dulces', 21),
('Curubandé', 21);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Nicoya Centro', 22),
('San Antonio', 22),
('Sámara', 22);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Santa Cruz Centro', 23),
('Bolsón', 23),
('Veintisiete de Abril', 23);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Bagaces Centro', 24),
('La Fortuna', 24),
('Mogote', 24);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Carrillo Centro', 25),
('Palmira', 25),
('Belén', 25);

-- Cantones Puntarenas (CantonId 26-30)
INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Puntarenas Centro', 26),
('Chacarita', 26),
('El Roble', 26);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Esparza Centro', 27),
('Caldera', 27),
('San Juan Grande', 27);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Buenos Aires Centro', 28),
('Volcán', 28),
('Potrero Grande', 28);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Montes de Oro Centro', 29),
('Miramar', 29),
('La Unión', 29);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Osa Centro', 30),
('Puerto Cortés', 30),
('Palmar', 30);

-- Cantones Limón (CantonId 31-35)
INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Limón Centro', 31),
('Valle La Estrella', 31),
('Río Blanco', 31);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Pococí Centro', 32),
('Guápiles', 32),
('Jiménez', 32);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Siquirres Centro', 33),
('Pacuarito', 33),
('Florida', 33);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Talamanca Centro', 34),
('Bratsi', 34),
('Sixaola', 34);

INSERT INTO Distrito (Nom_Distrito, CantonId) VALUES
('Matina Centro', 35),
('Batán', 35),
('Carrandí', 35);


INSERT INTO Usuarios (Id, Email, EmailConfirmed, 
                      PasswordHash, SecurityStamp, PhoneNumber,
                      PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, 
                      LockoutEnabled, AccessFailedCount, UserName
                    )
VALUES 
(
    '7e9f33d5-ed39-4c60-ac9a-0157f85fe7de', 'danielg@prueba.com', 0,
    'AHKTqzhTjb/RltZhp8vCVrDn6eL8wRMkk1LdUH/keQvbRwlwXqVEbwRcJa38g+1egA==', 'ea83ceb0-17fb-45c5-92a2-15896c5f6012', NULL,
    0, 0, '2025-10-22 05:38:43.017',
    1,0,'DanielG'
),
(
    'd0efef33-6ea7-4380-9a47-2916c411c655', 'nbrunner@uned.ac.cr', 0,
    'AJ3HyifRP0KjhaCrfOKAu33o42CGBAERFav7CfmDHEeAlslys83a+/1fuI5eO8p59w==', 'cb73e2e4-6870-404b-b194-521eb96f5e20',     NULL,
    0, 0, NULL,
    1,0,'NorberthB'
),
(
    'e64be105-16a8-4830-b6c7-0a71df6f0ef7','admin@prueba.com', 0,
    'ABaVTAJGpid2heCZdgQoZFxp7VqKWMpFHnQlYCZjzLPHA1l0ja/OwRaTU3RSB1VaDw==', '6a00e6e6-c50d-4f06-8104-cf7ea74201a9', NULL,
    0, 0, NULL,
    1, 0, 'Admin'
);



INSERT INTO Roles(Id,Name) 
values('DOCEN','Docente'),
      ('ADMIN','Administrador');




      INSERT INTO UsuarioRoles(UserId,RoleId)
VALUES('7e9f33d5-ed39-4c60-ac9a-0157f85fe7de','DOCEN'),--DanielG
      ('d0efef33-6ea7-4380-9a47-2916c411c655','DOCEN'),--NorberthB
      ('e64be105-16a8-4830-b6c7-0a71df6f0ef7','ADMIN');--Admin
 

 -- =============================================
-- DOCENTES
-- =============================================
INSERT INTO Docente (Nombre, Apellidos, Email, UserId)
VALUES
('Daniel', 'Gutiérrez Arrieta', 'danielg@prueba.com', '7e9f33d5-ed39-4c60-ac9a-0157f85fe7de'),
('Norberth', 'Brunner Méndez', 'nbrunner@uned.ac.cr', 'd0efef33-6ea7-4380-9a47-2916c411c655'),
('Admin', ' - ', 'admin@prueba.com', 'e64be105-16a8-4830-b6c7-0a71df6f0ef7');
 

-- Insertar Cuatrimestres de ejemplo
INSERT INTO Cuatrimestre (Nombre, Anio, Numero, Fec_Inicio, Fec_Fin) VALUES 
('I Cuatrimestre 2025', 2025, 1, '2025-01-15', '2025-05-15'),
('II Cuatrimestre 2025', 2025, 2, '2025-05-20', '2025-09-20'),
('III Cuatrimestre 2025', 2025, 3, '2025-09-25', '2026-01-25');

-- Insertar Cursos de ejemplo
INSERT INTO Curso (Codigo, Nom_Curso, Desc_curso, num_Creditos) VALUES 
('03101', 'Programación Avanzada en Web', 'Curso sobre desarrollo web con .NET MVC', 3),
('03102', 'Bases de Datos II', 'Curso avanzado de bases de datos relacionales', 3),
('03103', 'Ingeniería de Software II', 'Metodologías ágiles y gestión de proyectos', 3),
('03104', 'Desarrollo Móvil', 'Desarrollo de aplicaciones móviles multiplataforma', 3);


-- =============================================
-- DATOS GENÉRICOS - SISTEMA ACADÉMICO
-- =============================================

USE DBSistemaAcademico;
GO

-- =============================================
-- ESTUDIANTES (10 registros)
-- =============================================
INSERT INTO Estudiante (Identificacion, Nombre, Apellidos, Fec_Nacimiento, Email, DistritoId, Ind_Estado, Fec_Registro)
VALUES
('1-0234-0567', 'María', 'González Ramírez', '2000-03-15', 'mgonzalez@estudiante.com', 1, 'A', '2024-12-10 08:30:00'),
('1-0345-0678', 'Carlos', 'Mora Solís', '1999-07-22', 'cmora@estudiante.com', 4, 'A', '2024-12-10 09:15:00'),
('1-0456-0789', 'Ana', 'Vargas Castro', '2001-01-10', 'avargas@estudiante.com', 7, 'A', '2024-12-10 10:00:00'),
('1-0567-0890', 'José', 'Rodríguez Pérez', '2000-11-05', 'jrodriguez@estudiante.com', 10, 'A', '2024-12-10 11:20:00'),
('1-0678-0901', 'Laura', 'Fernández Jiménez', '1998-05-18', 'lfernandez@estudiante.com', 13, 'A', '2024-12-10 14:00:00'),
('1-0789-0012', 'Diego', 'Alvarado Muñoz', '2001-09-30', 'dalvarado@estudiante.com', 16, 'A', '2024-12-11 08:45:00'),
('1-0890-0123', 'Sofía', 'Quesada Hernández', '2000-02-14', 'squesada@estudiante.com', 19, 'A', '2024-12-11 09:30:00'),
('1-0901-0234', 'Andrés', 'Chaves Monge', '1999-12-20', 'achaves@estudiante.com', 22, 'A', '2024-12-11 10:15:00'),
('1-1012-0345', 'Gabriela', 'Salas Araya', '2001-06-08', 'gsalas@estudiante.com', 25, 'A', '2024-12-11 11:00:00'),
('1-1123-0456', 'Roberto', 'Campos Villalobos', '2000-04-25', 'rcampos@estudiante.com', 28, 'A', '2024-12-11 13:30:00');

-- =============================================
-- CURSO-CUATRIMESTRE (Asignación de cursos a cuatrimestres con docentes)
-- =============================================

-- I Cuatrimestre 2025
INSERT INTO CursoCuatrimestre (CursoId, CuatrimestreId, DocenteId, Ind_Estado, Fec_Registro)
VALUES
(1, 1, 1, 'A', '2024-12-15 10:00:00'), -- Prog. Avanzada Web - Daniel
(2, 1, 2, 'A', '2024-12-15 10:15:00'), -- BD II - Norberth
(3, 1, 1, 'A', '2024-12-15 10:30:00'), -- Ing. Software II - Daniel
(4, 1, 2, 'A', '2024-12-15 10:45:00'); -- Desarrollo Móvil - Norberth

-- II Cuatrimestre 2025
INSERT INTO CursoCuatrimestre (CursoId, CuatrimestreId, DocenteId, Ind_Estado, Fec_Registro)
VALUES
(1, 2, 1, 'A', '2025-04-20 09:00:00'), -- Prog. Avanzada Web - Daniel
(2, 2, 2, 'A', '2025-04-20 09:15:00'), -- BD II - Norberth
(3, 2, 1, 'A', '2025-04-20 09:30:00'), -- Ing. Software II - Daniel
(4, 2, 2, 'A', '2025-04-20 09:45:00'); -- Desarrollo Móvil - Norberth

-- III Cuatrimestre 2025
INSERT INTO CursoCuatrimestre (CursoId, CuatrimestreId, DocenteId, Ind_Estado, Fec_Registro)
VALUES
(1, 3, 1, 'A', '2025-08-25 08:00:00'), -- Prog. Avanzada Web - Daniel
(2, 3, 2, 'A', '2025-08-25 08:15:00'), -- BD II - Norberth
(3, 3, 1, 'A', '2025-08-25 08:30:00'), -- Ing. Software II - Daniel
(4, 3, 2, 'A', '2025-08-25 08:45:00'); -- Desarrollo Móvil - Norberth

-- =============================================
-- MATRÍCULA DE ESTUDIANTES - I CUATRIMESTRE 2025
-- =============================================

-- Todos los estudiantes se matriculan en los 4 cursos del I Cuatrimestre
INSERT INTO EstudianteCurso (EstudianteId, CursoCuatrimestreId, Fec_Matricula, Ind_Estado, Fec_Registro)
VALUES
-- María González (EstudianteId = 1)
(1, 1, '2025-01-10 09:00:00', 'A', '2025-01-10 09:00:00'),
(1, 2, '2025-01-10 09:05:00', 'A', '2025-01-10 09:05:00'),
(1, 3, '2025-01-10 09:10:00', 'A', '2025-01-10 09:10:00'),
(1, 4, '2025-01-10 09:15:00', 'A', '2025-01-10 09:15:00'),

-- Carlos Mora (EstudianteId = 2)
(2, 1, '2025-01-10 10:00:00', 'A', '2025-01-10 10:00:00'),
(2, 2, '2025-01-10 10:05:00', 'A', '2025-01-10 10:05:00'),
(2, 3, '2025-01-10 10:10:00', 'A', '2025-01-10 10:10:00'),
(2, 4, '2025-01-10 10:15:00', 'A', '2025-01-10 10:15:00'),

-- Ana Vargas (EstudianteId = 3) - Reprobará BD II
(3, 1, '2025-01-10 11:00:00', 'A', '2025-01-10 11:00:00'),
(3, 2, '2025-01-10 11:05:00', 'A', '2025-01-10 11:05:00'),
(3, 3, '2025-01-10 11:10:00', 'A', '2025-01-10 11:10:00'),
(3, 4, '2025-01-10 11:15:00', 'A', '2025-01-10 11:15:00'),

-- José Rodríguez (EstudianteId = 4)
(4, 1, '2025-01-10 14:00:00', 'A', '2025-01-10 14:00:00'),
(4, 2, '2025-01-10 14:05:00', 'A', '2025-01-10 14:05:00'),
(4, 3, '2025-01-10 14:10:00', 'A', '2025-01-10 14:10:00'),
(4, 4, '2025-01-10 14:15:00', 'A', '2025-01-10 14:15:00'),

-- Laura Fernández (EstudianteId = 5) - Reprobará Desarrollo Móvil
(5, 1, '2025-01-11 08:00:00', 'A', '2025-01-11 08:00:00'),
(5, 2, '2025-01-11 08:05:00', 'A', '2025-01-11 08:05:00'),
(5, 3, '2025-01-11 08:10:00', 'A', '2025-01-11 08:10:00'),
(5, 4, '2025-01-11 08:15:00', 'A', '2025-01-11 08:15:00'),

-- Diego Alvarado (EstudianteId = 6)
(6, 1, '2025-01-11 09:00:00', 'A', '2025-01-11 09:00:00'),
(6, 2, '2025-01-11 09:05:00', 'A', '2025-01-11 09:05:00'),
(6, 3, '2025-01-11 09:10:00', 'A', '2025-01-11 09:10:00'),
(6, 4, '2025-01-11 09:15:00', 'A', '2025-01-11 09:15:00'),

-- Sofía Quesada (EstudianteId = 7)
(7, 1, '2025-01-11 10:00:00', 'A', '2025-01-11 10:00:00'),
(7, 2, '2025-01-11 10:05:00', 'A', '2025-01-11 10:05:00'),
(7, 3, '2025-01-11 10:10:00', 'A', '2025-01-11 10:10:00'),
(7, 4, '2025-01-11 10:15:00', 'A', '2025-01-11 10:15:00'),

-- Andrés Chaves (EstudianteId = 8) - Reprobará Ing. Software II
(8, 1, '2025-01-11 11:00:00', 'A', '2025-01-11 11:00:00'),
(8, 2, '2025-01-11 11:05:00', 'A', '2025-01-11 11:05:00'),
(8, 3, '2025-01-11 11:10:00', 'A', '2025-01-11 11:10:00'),
(8, 4, '2025-01-11 11:15:00', 'A', '2025-01-11 11:15:00'),

-- Gabriela Salas (EstudianteId = 9)
(9, 1, '2025-01-11 13:00:00', 'A', '2025-01-11 13:00:00'),
(9, 2, '2025-01-11 13:05:00', 'A', '2025-01-11 13:05:00'),
(9, 3, '2025-01-11 13:10:00', 'A', '2025-01-11 13:10:00'),
(9, 4, '2025-01-11 13:15:00', 'A', '2025-01-11 13:15:00'),

-- Roberto Campos (EstudianteId = 10)
(10, 1, '2025-01-11 14:00:00', 'A', '2025-01-11 14:00:00'),
(10, 2, '2025-01-11 14:05:00', 'A', '2025-01-11 14:05:00'),
(10, 3, '2025-01-11 14:10:00', 'A', '2025-01-11 14:10:00'),
(10, 4, '2025-01-11 14:15:00', 'A', '2025-01-11 14:15:00');

-- =============================================
-- EVALUACIONES - I CUATRIMESTRE 2025
-- =============================================

-- María González - Aprueba todos
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(1, 1, 85.50, 'Excelente desempeño en proyectos', 'Excelente', 'Aprobado', '2025-05-10 10:00:00', 'A', '2025-05-10 10:00:00'),
(2, 2, 88.00, 'Muy buena comprensión de consultas SQL', 'Excelente', 'Aprobado', '2025-05-10 11:00:00', 'A', '2025-05-10 11:00:00'),
(3, 1, 82.75, 'Buen manejo de metodologías ágiles', 'Buena', 'Aprobado', '2025-05-10 12:00:00', 'A', '2025-05-10 12:00:00'),
(4, 2, 90.00, 'Destacada en desarrollo de apps móviles', 'Excelente', 'Aprobado', '2025-05-10 13:00:00', 'A', '2025-05-10 13:00:00');

-- Carlos Mora - Aprueba todos
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(5, 1, 78.50, 'Buen nivel técnico', 'Buena', 'Aprobado', '2025-05-10 10:30:00', 'A', '2025-05-10 10:30:00'),
(6, 2, 75.00, 'Cumple con los objetivos del curso', 'Buena', 'Aprobado', '2025-05-10 11:30:00', 'A', '2025-05-10 11:30:00'),
(7, 1, 80.25, 'Participación activa en clases', 'Buena', 'Aprobado', '2025-05-10 12:30:00', 'A', '2025-05-10 12:30:00'),
(8, 2, 77.50, 'Buen trabajo en equipo', 'Buena', 'Aprobado', '2025-05-10 13:30:00', 'A', '2025-05-10 13:30:00');

-- Ana Vargas - Reprueba BD II (nota 55)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(9, 1, 83.00, 'Muy buena en programación web', 'Buena', 'Aprobado', '2025-05-11 09:00:00', 'A', '2025-05-11 09:00:00'),
(10, 2, 55.00, 'Dificultades con consultas complejas, necesita reforzar', 'Regular', 'Reprobado', '2025-05-11 10:00:00', 'A', '2025-05-11 10:00:00'),
(11, 1, 79.50, 'Buen análisis de requerimientos', 'Buena', 'Aprobado', '2025-05-11 11:00:00', 'A', '2025-05-11 11:00:00'),
(12, 2, 81.00, 'Buen desarrollo de interfaces móviles', 'Buena', 'Aprobado', '2025-05-11 12:00:00', 'A', '2025-05-11 12:00:00');

-- José Rodríguez - Aprueba todos
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(13, 1, 92.00, 'Excelente programador', 'Excelente', 'Aprobado', '2025-05-11 09:30:00', 'A', '2025-05-11 09:30:00'),
(14, 2, 89.50, 'Dominio excepcional de BD', 'Excelente', 'Aprobado', '2025-05-11 10:30:00', 'A', '2025-05-11 10:30:00'),
(15, 1, 87.75, 'Liderazgo en proyectos', 'Excelente', 'Aprobado', '2025-05-11 11:30:00', 'A', '2025-05-11 11:30:00'),
(16, 2, 91.00, 'Innovador en soluciones móviles', 'Excelente', 'Aprobado', '2025-05-11 12:30:00', 'A', '2025-05-11 12:30:00');

-- Laura Fernández - Reprueba Desarrollo Móvil (nota 50)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(17, 1, 76.00, 'Buen manejo de frameworks web', 'Buena', 'Aprobado', '2025-05-12 08:00:00', 'A', '2025-05-12 08:00:00'),
(18, 2, 84.50, 'Excelente en modelado de datos', 'Excelente', 'Aprobado', '2025-05-12 09:00:00', 'A', '2025-05-12 09:00:00'),
(19, 1, 78.25, 'Comprende bien UML y patrones', 'Buena', 'Aprobado', '2025-05-12 10:00:00', 'A', '2025-05-12 10:00:00'),
(20, 2, 50.00, 'Dificultades con programación móvil, debe repetir', 'Baja', 'Reprobado', '2025-05-12 11:00:00', 'A', '2025-05-12 11:00:00');

-- Diego Alvarado - Aprueba todos
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(21, 1, 81.50, 'Buen nivel de código limpio', 'Buena', 'Aprobado', '2025-05-12 08:30:00', 'A', '2025-05-12 08:30:00'),
(22, 2, 79.00, 'Maneja bien transacciones', 'Buena', 'Aprobado', '2025-05-12 09:30:00', 'A', '2025-05-12 09:30:00'),
(23, 1, 85.00, 'Buen trabajo en Scrum', 'Excelente', 'Aprobado', '2025-05-12 10:30:00', 'A', '2025-05-12 10:30:00'),
(24, 2, 82.50, 'Desarrollo eficiente de apps', 'Buena', 'Aprobado', '2025-05-12 11:30:00', 'A', '2025-05-12 11:30:00');

-- Sofía Quesada - Aprueba todos
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(25, 1, 88.50, 'Excelente lógica de programación', 'Excelente', 'Aprobado', '2025-05-12 14:00:00', 'A', '2025-05-12 14:00:00'),
(26, 2, 86.00, 'Muy buena optimización de consultas', 'Excelente', 'Aprobado', '2025-05-12 15:00:00', 'A', '2025-05-12 15:00:00'),
(27, 1, 84.75, 'Excelente documentación', 'Excelente', 'Aprobado', '2025-05-12 16:00:00', 'A', '2025-05-12 16:00:00'),
(28, 2, 89.00, 'Creatividad en diseño de UI móvil', 'Excelente', 'Aprobado', '2025-05-13 08:00:00', 'A', '2025-05-13 08:00:00');

-- Andrés Chaves - Reprueba Ing. Software II (nota 58)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(29, 1, 74.00, 'Cumple con los requisitos básicos', 'Buena', 'Aprobado', '2025-05-13 09:00:00', 'A', '2025-05-13 09:00:00'),
(30, 2, 80.50, 'Buen manejo de procedimientos almacenados', 'Buena', 'Aprobado', '2025-05-13 10:00:00', 'A', '2025-05-13 10:00:00'),
(31, 1, 58.00, 'Falta comprensión de metodologías, debe reforzar', 'Regular', 'Reprobado', '2025-05-13 11:00:00', 'A', '2025-05-13 11:00:00'),
(32, 2, 77.00, 'Buen desarrollo básico de apps', 'Buena', 'Aprobado', '2025-05-13 12:00:00', 'A', '2025-05-13 12:00:00');

-- Gabriela Salas - Aprueba todos
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(33, 1, 91.50, 'Sobresaliente en todos los aspectos', 'Excelente', 'Aprobado', '2025-05-13 13:00:00', 'A', '2025-05-13 13:00:00'),
(34, 2, 93.00, 'Excelente normalización y diseño', 'Excelente', 'Aprobado', '2025-05-13 14:00:00', 'A', '2025-05-13 14:00:00'),
(35, 1, 90.25, 'Liderazgo excepcional', 'Excelente', 'Aprobado', '2025-05-13 15:00:00', 'A', '2025-05-13 15:00:00'),
(36, 2, 94.00, 'Mejor proyecto del cuatrimestre', 'Excelente', 'Aprobado', '2025-05-13 16:00:00', 'A', '2025-05-13 16:00:00');

-- Roberto Campos - Aprueba todos
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(37, 1, 79.50, 'Buen rendimiento general', 'Buena', 'Aprobado', '2025-05-14 09:00:00', 'A', '2025-05-14 09:00:00'),
(38, 2, 81.00, 'Comprende bien índices y vistas', 'Buena', 'Aprobado', '2025-05-14 10:00:00', 'A', '2025-05-14 10:00:00'),
(39, 1, 83.50, 'Buen análisis de casos de uso', 'Buena', 'Aprobado', '2025-05-14 11:00:00', 'A', '2025-05-14 11:00:00'),
(40, 2, 80.00, 'Desarrollo correcto de funcionalidades', 'Buena', 'Aprobado', '2025-05-14 12:00:00', 'A', '2025-05-14 12:00:00');

-- =============================================
-- MATRÍCULA - II CUATRIMESTRE 2025
-- =============================================

-- Todos se matriculan en cursos nuevos
-- Ana Vargas repite BD II (CursoCuatrimestreId = 6)
-- Laura Fernández repite Desarrollo Móvil (CursoCuatrimestreId = 8)
-- Andrés Chaves repite Ing. Software II (CursoCuatrimestreId = 7)

INSERT INTO EstudianteCurso (EstudianteId, CursoCuatrimestreId, Fec_Matricula, Ind_Estado, Fec_Registro)
VALUES
-- María González - cursos nuevos
(1, 5, '2025-05-15 09:00:00', 'A', '2025-05-15 09:00:00'),
(1, 6, '2025-05-15 09:05:00', 'A', '2025-05-15 09:05:00'),
(1, 7, '2025-05-15 09:10:00', 'A', '2025-05-15 09:10:00'),
(1, 8, '2025-05-15 09:15:00', 'A', '2025-05-15 09:15:00'),

-- Carlos Mora - cursos nuevos
(2, 5, '2025-05-15 10:00:00', 'A', '2025-05-15 10:00:00'),
(2, 6, '2025-05-15 10:05:00', 'A', '2025-05-15 10:05:00'),
(2, 7, '2025-05-15 10:10:00', 'A', '2025-05-15 10:10:00'),
(2, 8, '2025-05-15 10:15:00', 'A', '2025-05-15 10:15:00'),

-- Ana Vargas - REPITE BD II (id 6) + otros nuevos
(3, 5, '2025-05-15 11:00:00', 'A', '2025-05-15 11:00:00'),
(3, 6, '2025-05-15 11:05:00', 'A', '2025-05-15 11:05:00'), -- REPITE
(3, 7, '2025-05-15 11:10:00', 'A', '2025-05-15 11:10:00'),
(3, 8, '2025-05-15 11:15:00', 'A', '2025-05-15 11:15:00'),

-- José Rodríguez - cursos nuevos
(4, 5, '2025-05-15 14:00:00', 'A', '2025-05-15 14:00:00'),
(4, 6, '2025-05-15 14:05:00', 'A', '2025-05-15 14:05:00'),
(4, 7, '2025-05-15 14:10:00', 'A', '2025-05-15 14:10:00'),
(4, 8, '2025-05-15 14:15:00', 'A', '2025-05-15 14:15:00'),

-- Laura Fernández - REPITE Desarrollo Móvil (id 8) + otros nuevos
(5, 5, '2025-05-16 08:00:00', 'A', '2025-05-16 08:00:00'),
(5, 6, '2025-05-16 08:05:00', 'A', '2025-05-16 08:05:00'),
(5, 7, '2025-05-16 08:10:00', 'A', '2025-05-16 08:10:00'),
(5, 8, '2025-05-16 08:15:00', 'A', '2025-05-16 08:15:00'), -- REPITE

-- Diego Alvarado - cursos nuevos
(6, 5, '2025-05-16 09:00:00', 'A', '2025-05-16 09:00:00'),
(6, 6, '2025-05-16 09:05:00', 'A', '2025-05-16 09:05:00'),
(6, 7, '2025-05-16 09:10:00', 'A', '2025-05-16 09:10:00'),
(6, 8, '2025-05-16 09:15:00', 'A', '2025-05-16 09:15:00'),

-- Sofía Quesada - cursos nuevos
(7, 5, '2025-05-16 10:00:00', 'A', '2025-05-16 10:00:00'),
(7, 6, '2025-05-16 10:05:00', 'A', '2025-05-16 10:05:00'),
(7, 7, '2025-05-16 10:10:00', 'A', '2025-05-16 10:10:00'),
(7, 8, '2025-05-16 10:15:00', 'A', '2025-05-16 10:15:00'),

-- Andrés Chaves - REPITE Ing. Software II (id 7) + otros nuevos
(8, 5, '2025-05-16 11:00:00', 'A', '2025-05-16 11:00:00'),
(8, 6, '2025-05-16 11:05:00', 'A', '2025-05-16 11:05:00'),
(8, 7, '2025-05-16 11:10:00', 'A', '2025-05-16 11:10:00'), -- REPITE
(8, 8, '2025-05-16 11:15:00', 'A', '2025-05-16 11:15:00'),

-- Gabriela Salas - cursos nuevos
(9, 5, '2025-05-16 13:00:00', 'A', '2025-05-16 13:00:00'),
(9, 6, '2025-05-16 13:05:00', 'A', '2025-05-16 13:05:00'),
(9, 7, '2025-05-16 13:10:00', 'A', '2025-05-16 13:10:00'),
(9, 8, '2025-05-16 13:15:00', 'A', '2025-05-16 13:15:00'),

-- Roberto Campos - cursos nuevos
(10, 5, '2025-05-16 14:00:00', 'A', '2025-05-16 14:00:00'),
(10, 6, '2025-05-16 14:05:00', 'A', '2025-05-16 14:05:00'),
(10, 7, '2025-05-16 14:10:00', 'A', '2025-05-16 14:10:00'),
(10, 8, '2025-05-16 14:15:00', 'A', '2025-05-16 14:15:00');

-- =============================================
-- EVALUACIONES - II CUATRIMESTRE 2025
-- =============================================

-- María González - Aprueba todos (EstudianteCursoId 41-44)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(41, 1, 87.00, 'Mejora continua en sus proyectos', 'Excelente', 'Aprobado', '2025-09-15 10:00:00', 'A', '2025-09-15 10:00:00'),
(42, 2, 90.50, 'Dominio avanzado de BD', 'Excelente', 'Aprobado', '2025-09-15 11:00:00', 'A', '2025-09-15 11:00:00'),
(43, 1, 85.25, 'Excelente aplicación de metodologías', 'Excelente', 'Aprobado', '2025-09-15 12:00:00', 'A', '2025-09-15 12:00:00'),
(44, 2, 92.00, 'Proyecto móvil destacado', 'Excelente', 'Aprobado', '2025-09-15 13:00:00', 'A', '2025-09-15 13:00:00');

-- Carlos Mora - Aprueba todos (EstudianteCursoId 45-48)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(45, 1, 80.00, 'Mantiene buen rendimiento', 'Buena', 'Aprobado', '2025-09-15 10:30:00', 'A', '2025-09-15 10:30:00'),
(46, 2, 77.50, 'Comprensión sólida de conceptos', 'Buena', 'Aprobado', '2025-09-15 11:30:00', 'A', '2025-09-15 11:30:00'),
(47, 1, 82.00, 'Buen trabajo colaborativo', 'Buena', 'Aprobado', '2025-09-15 12:30:00', 'A', '2025-09-15 12:30:00'),
(48, 2, 79.00, 'Desarrollo competente', 'Buena', 'Aprobado', '2025-09-15 13:30:00', 'A', '2025-09-15 13:30:00');

-- Ana Vargas - APRUEBA BD II esta vez (EstudianteCursoId 49-52)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(49, 1, 84.50, 'Sigue mejorando en web', 'Buena', 'Aprobado', '2025-09-16 09:00:00', 'A', '2025-09-16 09:00:00'),
(50, 2, 72.00, 'Gran mejora, ahora aprueba BD II con esfuerzo', 'Buena', 'Aprobado', '2025-09-16 10:00:00', 'A', '2025-09-16 10:00:00'),
(51, 1, 81.00, 'Buen análisis de sistemas', 'Buena', 'Aprobado', '2025-09-16 11:00:00', 'A', '2025-09-16 11:00:00'),
(52, 2, 83.50, 'Mejora significativa en móviles', 'Buena', 'Aprobado', '2025-09-16 12:00:00', 'A', '2025-09-16 12:00:00');

-- José Rodríguez - Aprueba todos (EstudianteCursoId 53-56)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(53, 1, 93.50, 'Mantiene excelencia académica', 'Excelente', 'Aprobado', '2025-09-16 09:30:00', 'A', '2025-09-16 09:30:00'),
(54, 2, 91.00, 'Continúa sobresaliendo', 'Excelente', 'Aprobado', '2025-09-16 10:30:00', 'A', '2025-09-16 10:30:00'),
(55, 1, 89.50, 'Liderazgo natural en equipos', 'Excelente', 'Aprobado', '2025-09-16 11:30:00', 'A', '2025-09-16 11:30:00'),
(56, 2, 92.50, 'Innovación constante', 'Excelente', 'Aprobado', '2025-09-16 12:30:00', 'A', '2025-09-16 12:30:00');

-- Laura Fernández - APRUEBA Desarrollo Móvil esta vez (EstudianteCursoId 57-60)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(57, 1, 78.00, 'Consistencia en web', 'Buena', 'Aprobado', '2025-09-17 08:00:00', 'A', '2025-09-17 08:00:00'),
(58, 2, 86.00, 'Mantiene alto nivel en BD', 'Excelente', 'Aprobado', '2025-09-17 09:00:00', 'A', '2025-09-17 09:00:00'),
(59, 1, 80.00, 'Buena comprensión de patrones', 'Buena', 'Aprobado', '2025-09-17 10:00:00', 'A', '2025-09-17 10:00:00'),
(60, 2, 70.00, 'Logra aprobar móvil con dedicación extra', 'Buena', 'Aprobado', '2025-09-17 11:00:00', 'A', '2025-09-17 11:00:00');

-- Diego Alvarado - Aprueba todos (EstudianteCursoId 61-64)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(61, 1, 83.00, 'Código bien estructurado', 'Buena', 'Aprobado', '2025-09-17 08:30:00', 'A', '2025-09-17 08:30:00'),
(62, 2, 80.50, 'Buen manejo de BD', 'Buena', 'Aprobado', '2025-09-17 09:30:00', 'A', '2025-09-17 09:30:00'),
(63, 1, 86.50, 'Excelente en gestión de proyectos', 'Excelente', 'Aprobado', '2025-09-17 10:30:00', 'A', '2025-09-17 10:30:00'),
(64, 2, 84.00, 'Buen desarrollo de funcionalidades', 'Buena', 'Aprobado', '2025-09-17 11:30:00', 'A', '2025-09-17 11:30:00');

-- Sofía Quesada - Aprueba todos (EstudianteCursoId 65-68)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(65, 1, 90.00, 'Mantiene excelente nivel', 'Excelente', 'Aprobado', '2025-09-17 14:00:00', 'A', '2025-09-17 14:00:00'),
(66, 2, 88.50, 'Optimización avanzada', 'Excelente', 'Aprobado', '2025-09-17 15:00:00', 'A', '2025-09-17 15:00:00'),
(67, 1, 87.00, 'Documentación impecable', 'Excelente', 'Aprobado', '2025-09-17 16:00:00', 'A', '2025-09-17 16:00:00'),
(68, 2, 91.50, 'UX excepcional en móviles', 'Excelente', 'Aprobado', '2025-09-18 08:00:00', 'A', '2025-09-18 08:00:00');

-- Andrés Chaves - APRUEBA Ing. Software II esta vez (EstudianteCursoId 69-72)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(69, 1, 76.00, 'Mantiene nivel básico', 'Buena', 'Aprobado', '2025-09-18 09:00:00', 'A', '2025-09-18 09:00:00'),
(70, 2, 82.00, 'Buen nivel en BD', 'Buena', 'Aprobado', '2025-09-18 10:00:00', 'A', '2025-09-18 10:00:00'),
(71, 1, 71.00, 'Aprueba Ing. Software II tras refuerzo', 'Buena', 'Aprobado', '2025-09-18 11:00:00', 'A', '2025-09-18 11:00:00'),
(72, 2, 78.50, 'Desarrollo competente', 'Buena', 'Aprobado', '2025-09-18 12:00:00', 'A', '2025-09-18 12:00:00');

-- Gabriela Salas - Aprueba todos (EstudianteCursoId 73-76)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(73, 1, 92.50, 'Sigue destacando', 'Excelente', 'Aprobado', '2025-09-18 13:00:00', 'A', '2025-09-18 13:00:00'),
(74, 2, 94.50, 'Excelencia en BD', 'Excelente', 'Aprobado', '2025-09-18 14:00:00', 'A', '2025-09-18 14:00:00'),
(75, 1, 91.75, 'Liderazgo sobresaliente', 'Excelente', 'Aprobado', '2025-09-18 15:00:00', 'A', '2025-09-18 15:00:00'),
(76, 2, 95.00, 'Mejor estudiante del cuatrimestre', 'Excelente', 'Aprobado', '2025-09-18 16:00:00', 'A', '2025-09-18 16:00:00');

-- Roberto Campos - Aprueba todos (EstudianteCursoId 77-80)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(77, 1, 81.00, 'Rendimiento estable', 'Buena', 'Aprobado', '2025-09-19 09:00:00', 'A', '2025-09-19 09:00:00'),
(78, 2, 82.50, 'Buen manejo de BD', 'Buena', 'Aprobado', '2025-09-19 10:00:00', 'A', '2025-09-19 10:00:00'),
(79, 1, 84.50, 'Mejora en análisis', 'Buena', 'Aprobado', '2025-09-19 11:00:00', 'A', '2025-09-19 11:00:00'),
(80, 2, 81.50, 'Desarrollo correcto', 'Buena', 'Aprobado', '2025-09-19 12:00:00', 'A', '2025-09-19 12:00:00');

-- =============================================
-- MATRÍCULA - III CUATRIMESTRE 2025
-- =============================================

-- Todos continúan con nuevos cursos (CursoCuatrimestreId 9-12)
INSERT INTO EstudianteCurso (EstudianteId, CursoCuatrimestreId, Fec_Matricula, Ind_Estado, Fec_Registro)
VALUES
-- María González
(1, 9, '2025-09-20 09:00:00', 'A', '2025-09-20 09:00:00'),
(1, 10, '2025-09-20 09:05:00', 'A', '2025-09-20 09:05:00'),
(1, 11, '2025-09-20 09:10:00', 'A', '2025-09-20 09:10:00'),
(1, 12, '2025-09-20 09:15:00', 'A', '2025-09-20 09:15:00'),

-- Carlos Mora
(2, 9, '2025-09-20 10:00:00', 'A', '2025-09-20 10:00:00'),
(2, 10, '2025-09-20 10:05:00', 'A', '2025-09-20 10:05:00'),
(2, 11, '2025-09-20 10:10:00', 'A', '2025-09-20 10:10:00'),
(2, 12, '2025-09-20 10:15:00', 'A', '2025-09-20 10:15:00'),

-- Ana Vargas
(3, 9, '2025-09-20 11:00:00', 'A', '2025-09-20 11:00:00'),
(3, 10, '2025-09-20 11:05:00', 'A', '2025-09-20 11:05:00'),
(3, 11, '2025-09-20 11:10:00', 'A', '2025-09-20 11:10:00'),
(3, 12, '2025-09-20 11:15:00', 'A', '2025-09-20 11:15:00'),

-- José Rodríguez
(4, 9, '2025-09-20 14:00:00', 'A', '2025-09-20 14:00:00'),
(4, 10, '2025-09-20 14:05:00', 'A', '2025-09-20 14:05:00'),
(4, 11, '2025-09-20 14:10:00', 'A', '2025-09-20 14:10:00'),
(4, 12, '2025-09-20 14:15:00', 'A', '2025-09-20 14:15:00'),

-- Laura Fernández
(5, 9, '2025-09-21 08:00:00', 'A', '2025-09-21 08:00:00'),
(5, 10, '2025-09-21 08:05:00', 'A', '2025-09-21 08:05:00'),
(5, 11, '2025-09-21 08:10:00', 'A', '2025-09-21 08:10:00'),
(5, 12, '2025-09-21 08:15:00', 'A', '2025-09-21 08:15:00'),

-- Diego Alvarado
(6, 9, '2025-09-21 09:00:00', 'A', '2025-09-21 09:00:00'),
(6, 10, '2025-09-21 09:05:00', 'A', '2025-09-21 09:05:00'),
(6, 11, '2025-09-21 09:10:00', 'A', '2025-09-21 09:10:00'),
(6, 12, '2025-09-21 09:15:00', 'A', '2025-09-21 09:15:00'),

-- Sofía Quesada
(7, 9, '2025-09-21 10:00:00', 'A', '2025-09-21 10:00:00'),
(7, 10, '2025-09-21 10:05:00', 'A', '2025-09-21 10:05:00'),
(7, 11, '2025-09-21 10:10:00', 'A', '2025-09-21 10:10:00'),
(7, 12, '2025-09-21 10:15:00', 'A', '2025-09-21 10:15:00'),

-- Andrés Chaves
(8, 9, '2025-09-21 11:00:00', 'A', '2025-09-21 11:00:00'),
(8, 10, '2025-09-21 11:05:00', 'A', '2025-09-21 11:05:00'),
(8, 11, '2025-09-21 11:10:00', 'A', '2025-09-21 11:10:00'),
(8, 12, '2025-09-21 11:15:00', 'A', '2025-09-21 11:15:00'),

-- Gabriela Salas
(9, 9, '2025-09-21 13:00:00', 'A', '2025-09-21 13:00:00'),
(9, 10, '2025-09-21 13:05:00', 'A', '2025-09-21 13:05:00'),
(9, 11, '2025-09-21 13:10:00', 'A', '2025-09-21 13:10:00'),
(9, 12, '2025-09-21 13:15:00', 'A', '2025-09-21 13:15:00'),

-- Roberto Campos
(10, 9, '2025-09-21 14:00:00', 'A', '2025-09-21 14:00:00'),
(10, 10, '2025-09-21 14:05:00', 'A', '2025-09-21 14:05:00'),
(10, 11, '2025-09-21 14:10:00', 'A', '2025-09-21 14:10:00'),
(10, 12, '2025-09-21 14:15:00', 'A', '2025-09-21 14:15:00');

-- =============================================
-- EVALUACIONES - III CUATRIMESTRE 2025
-- Estos están en proceso (se evalúan en enero 2026)
-- =============================================

-- María González - En Proceso (EstudianteCursoId 81-84)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(81, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-20 10:00:00', 'A', '2025-10-20 10:00:00'),
(82, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-20 11:00:00', 'A', '2025-10-20 11:00:00'),
(83, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-20 12:00:00', 'A', '2025-10-20 12:00:00'),
(84, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-20 13:00:00', 'A', '2025-10-20 13:00:00');

-- Carlos Mora - En Proceso (EstudianteCursoId 85-88)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(85, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-20 10:30:00', 'A', '2025-10-20 10:30:00'),
(86, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-20 11:30:00', 'A', '2025-10-20 11:30:00'),
(87, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-20 12:30:00', 'A', '2025-10-20 12:30:00'),
(88, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-20 13:30:00', 'A', '2025-10-20 13:30:00');

-- Ana Vargas - En Proceso (EstudianteCursoId 89-92)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(89, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-21 09:00:00', 'A', '2025-10-21 09:00:00'),
(90, 2, 0.00, 'Cursando actualmente', 'Regular', 'En Proceso', '2025-10-21 10:00:00', 'A', '2025-10-21 10:00:00'),
(91, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-21 11:00:00', 'A', '2025-10-21 11:00:00'),
(92, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-21 12:00:00', 'A', '2025-10-21 12:00:00');

-- José Rodríguez - En Proceso (EstudianteCursoId 93-96)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(93, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-21 09:30:00', 'A', '2025-10-21 09:30:00'),
(94, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-21 10:30:00', 'A', '2025-10-21 10:30:00'),
(95, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-21 11:30:00', 'A', '2025-10-21 11:30:00'),
(96, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-21 12:30:00', 'A', '2025-10-21 12:30:00');

-- Laura Fernández - En Proceso (EstudianteCursoId 97-100)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(97, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-22 08:00:00', 'A', '2025-10-22 08:00:00'),
(98, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-22 09:00:00', 'A', '2025-10-22 09:00:00'),
(99, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-22 10:00:00', 'A', '2025-10-22 10:00:00'),
(100, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-22 11:00:00', 'A', '2025-10-22 11:00:00');

-- Diego Alvarado - En Proceso (EstudianteCursoId 101-104)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(101, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-22 08:30:00', 'A', '2025-10-22 08:30:00'),
(102, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-22 09:30:00', 'A', '2025-10-22 09:30:00'),
(103, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-22 10:30:00', 'A', '2025-10-22 10:30:00'),
(104, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-22 11:30:00', 'A', '2025-10-22 11:30:00');

-- Sofía Quesada - En Proceso (EstudianteCursoId 105-108)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(105, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-22 14:00:00', 'A', '2025-10-22 14:00:00'),
(106, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-22 15:00:00', 'A', '2025-10-22 15:00:00'),
(107, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-22 16:00:00', 'A', '2025-10-22 16:00:00'),
(108, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-23 08:00:00', 'A', '2025-10-23 08:00:00');

-- Andrés Chaves - En Proceso (EstudianteCursoId 109-112)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(109, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-23 09:00:00', 'A', '2025-10-23 09:00:00'),
(110, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-23 10:00:00', 'A', '2025-10-23 10:00:00'),
(111, 1, 0.00, 'Cursando actualmente', 'Regular', 'En Proceso', '2025-10-23 11:00:00', 'A', '2025-10-23 11:00:00'),
(112, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-23 12:00:00', 'A', '2025-10-23 12:00:00');

-- Gabriela Salas - En Proceso (EstudianteCursoId 113-116)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(113, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-23 13:00:00', 'A', '2025-10-23 13:00:00'),
(114, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-23 14:00:00', 'A', '2025-10-23 14:00:00'),
(115, 1, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-23 15:00:00', 'A', '2025-10-23 15:00:00'),
(116, 2, 0.00, 'Cursando actualmente', 'Excelente', 'En Proceso', '2025-10-23 16:00:00', 'A', '2025-10-23 16:00:00');

-- Roberto Campos - En Proceso (EstudianteCursoId 117-120)
INSERT INTO Evaluacion (EstudianteCursoId, DocenteId, Nota, Observaciones, TipoParticipacion, Estado, Fec_Evaluacion, Ind_Estado, Fec_Registro)
VALUES
(117, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-24 09:00:00', 'A', '2025-10-24 09:00:00'),
(118, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-24 10:00:00', 'A', '2025-10-24 10:00:00'),
(119, 1, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-24 11:00:00', 'A', '2025-10-24 11:00:00'),
(120, 2, 0.00, 'Cursando actualmente', 'Buena', 'En Proceso', '2025-10-24 12:00:00', 'A', '2025-10-24 12:00:00');
