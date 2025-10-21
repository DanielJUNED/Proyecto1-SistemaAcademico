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
