
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

-- Relacionar cursos con cuatrimestres
INSERT INTO CursoCuatrimestre (CursoId, CuatrimestreId) VALUES 
(1, 3), -- Programación Web en III Cuatrimestre
(2, 3),
(3, 3),
(4, 3);



-- Insertar un Docente de prueba
-- Password: Admin123! (debes encriptar esto en tu aplicación)
INSERT INTO Docentes (Usuario, PasswordHash, Nombre, Apellidos, Email) VALUES 
('admin', 'Admin123!', 'Carlos', 'Rodríguez Pérez', 'carlos.rodriguez@uned.ac.cr');

-- Insertar Estudiantes de prueba
INSERT INTO Estudiante (Identificacion, Nombre, Apellidos, Fec_Nacimiento, Email, DistritoID) VALUES 
('118230456', 'María', 'González López', '2000-05-15', 'maria.gonzalez@est.uned.ac.cr', 1),
('207890123', 'José', 'Ramírez Castro', '1999-08-22', 'jose.ramirez@est.uned.ac.cr', 2),
('305670234', 'Ana', 'Méndez Vargas', '2001-03-10', 'ana.mendez@est.uned.ac.cr', 3);

-- Matricular estudiantes en cursos
INSERT INTO EstudianteCurso (EstudianteId, CursoCuatrimestreId) VALUES 
(1, 1), -- María en Programación Web
(1, 2), -- María en Bases de Datos
(2, 1), -- José en Programación Web
(3, 1); -- Ana en Programación Web

-- Insertar evaluaciones de ejemplo
INSERT INTO Evaluacion (EstudianteCursoID, DocenteID, Nota, Observaciones, TipoParticipacion, Estado) VALUES 
(1, 1, 85.50, 'Excelente desempeño en el proyecto 1', 'Excelente', 'Aprobado'),
(2, 1, 72.00, 'Buen trabajo, puede mejorar en documentación', 'Buena', 'Aprobado'),
(3, 1, 55.00, 'Necesita reforzar conceptos básicos', 'Regular', 'Reprobado');

GO
