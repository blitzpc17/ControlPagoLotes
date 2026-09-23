-- ==============================================================================
-- SISTEMA: CONTROL DE PAGO DE LOTES (ControlPagoLotes)
-- ARCHIVO: 02_seeder_test_databases.sql
-- DESCRIPCIÓN: Seeder completo para pruebas en dos bases de datos:
--              1) [dbpagolotes_teh] (Tehuacán)
--              2) [dbpagolotes_aja] (Ajalpan)
-- Incluye: Esquema, Usuarios autorizados, Zonas, Variables Globales,
--          10 Clientes por BD (PAGOS) y sus Partidas de Pago (PAGOSPARTIDAS).
-- ==============================================================================

SET NOCOUNT ON;

-- ******************************************************************************
-- SECCIÓN 1: BASE DE DATOS [dbpagolotes_teh] (TEHUACÁN)
-- ******************************************************************************
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'dbpagolotes_teh')
BEGIN
    PRINT 'Creando base de datos [dbpagolotes_teh]...';
    CREATE DATABASE [dbpagolotes_teh];
END
GO

USE [dbpagolotes_teh];
GO

-- 1.1 Crear Esquema en dbpagolotes_teh
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USUARIOS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[USUARIOS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Usuario] NVARCHAR(150) NOT NULL UNIQUE,
        [Password] NVARCHAR(150) NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZONAS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZONAS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nombre] NVARCHAR(200) NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VARIABLESGLOBALES]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VARIABLESGLOBALES] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [label] NVARCHAR(150) NOT NULL UNIQUE,
        [valor] NVARCHAR(MAX) NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PAGOS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PAGOS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NombreCliente] NVARCHAR(250) NOT NULL,
        [Total] DECIMAL(18,2) NOT NULL,
        [Meses] NVARCHAR(50) NULL,
        [ZonaId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[ZONAS]([Id]),
        [DiaPago] NVARCHAR(50) NULL,
        [Lotes] NVARCHAR(250) NULL,
        [FechaRegistro] DATETIME NOT NULL,
        [Estado] NVARCHAR(50) NOT NULL DEFAULT '1',
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [Telefonos] NVARCHAR(150) NULL,
        [Observacion] NVARCHAR(MAX) NULL
    );
    CREATE NONCLUSTERED INDEX [IX_PAGOS_ZonaId] ON [dbo].[PAGOS] ([ZonaId]);
    CREATE NONCLUSTERED INDEX [IX_PAGOS_Estado] ON [dbo].[PAGOS] ([Estado]);
    CREATE NONCLUSTERED INDEX [IX_PAGOS_FechaRegistro] ON [dbo].[PAGOS] ([FechaRegistro]);
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PAGOSPARTIDAS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PAGOSPARTIDAS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [PagoId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[PAGOS]([Id]) ON DELETE CASCADE,
        [Monto] DECIMAL(18,2) NOT NULL,
        [MontoOriginal] DECIMAL(18,2) NULL DEFAULT 0,
        [Fecha] DATETIME NOT NULL,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [UsuarioId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[USUARIOS]([Id]),
        [FormaPago] INT NULL, -- 0=MIGRADO, 1=EFECTIVO, 2=TRANSFERENCIA
        [FechaModificacion] DATETIME NULL,
        [UsuarioModificoId] INT NULL FOREIGN KEY REFERENCES [dbo].[USUARIOS]([Id]),
        [FechaBaja] DATETIME NULL,
        [UsuarioBajaId] INT NULL FOREIGN KEY REFERENCES [dbo].[USUARIOS]([Id])
    );
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_PagoId] ON [dbo].[PAGOSPARTIDAS] ([PagoId]);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_Fecha] ON [dbo].[PAGOSPARTIDAS] ([Fecha]);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_FechaCreacion] ON [dbo].[PAGOSPARTIDAS] ([FechaCreacion]);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_FechaBaja] ON [dbo].[PAGOSPARTIDAS] ([FechaBaja]);
END
GO

-- Función de zonas permitidas
CREATE OR ALTER FUNCTION [dbo].[fn_ZonasPermitidasPorUsuario] (@UsuarioId INT)
RETURNS TABLE
AS
RETURN
(
    SELECT CAST(z.value AS INT) AS [ZonaId]
    FROM [dbo].[VARIABLESGLOBALES] vg
    CROSS APPLY OPENJSON(vg.valor) 
    WITH (
        [UsuarioId] INT            '$.UsuarioId',
        [ZonasId]   NVARCHAR(MAX)  '$.ZonasId' AS JSON
    ) u
    CROSS APPLY OPENJSON(u.[ZonasId]) z
    WHERE vg.label = 'FltroZonas'
      AND u.[UsuarioId] = @UsuarioId
);
GO

-- 1.2 Limpiar y Sembrar Datos en dbpagolotes_teh
PRINT 'Sembrando datos en [dbpagolotes_teh]...';

-- Limpiar tablas
DELETE FROM [dbo].[PAGOSPARTIDAS];
DELETE FROM [dbo].[PAGOS];
DELETE FROM [dbo].[VARIABLESGLOBALES];
DELETE FROM [dbo].[ZONAS];
DELETE FROM [dbo].[USUARIOS];

DBCC CHECKIDENT ('[dbo].[USUARIOS]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[ZONAS]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PAGOS]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PAGOSPARTIDAS]', RESEED, 0);

-- Usuarios con privilegios de sistema
SET IDENTITY_INSERT [dbo].[USUARIOS] ON;
INSERT INTO [dbo].[USUARIOS] ([Id], [Usuario], [Password]) VALUES
(1, 'ADMIN', 'admin123'),
(2, 'DIANA', 'diana123'),
(3, 'DONATO', 'donato123'),
(4, 'EMMANUEL', 'emmanuel123'),
(5, 'COBRANZA_TEH', 'caja123');
SET IDENTITY_INSERT [dbo].[USUARIOS] OFF;

-- Zonas de Tehuacán
SET IDENTITY_INSERT [dbo].[ZONAS] ON;
INSERT INTO [dbo].[ZONAS] ([Id], [Nombre]) VALUES
(1, 'Residencial San Lorenzo (Tehuacán)'),
(2, 'Fracc. Vista Hermosa'),
(3, 'Lomas de la Soledad'),
(4, 'Colonia San Nicolás Tetitzintla'),
(5, 'Hacienda San Diego');
SET IDENTITY_INSERT [dbo].[ZONAS] OFF;

-- Variables Globales (Filtro de zonas por usuario)
INSERT INTO [dbo].[VARIABLESGLOBALES] ([label], [valor]) VALUES
('FltroZonas', '[{"UsuarioId":1,"ZonasId":[1,2,3,4,5]},{"UsuarioId":2,"ZonasId":[1,2,3,4,5]},{"UsuarioId":3,"ZonasId":[1,2,3,4,5]},{"UsuarioId":4,"ZonasId":[1,2,3,4,5]},{"UsuarioId":5,"ZonasId":[1,2,3]}]');

-- 10 Clientes (PAGOS) en Tehuacán
-- Estados: '1'=AL CORRIENTE, '2'=PAGADO, '3'=CANCELADO, '4'=ATRASADO
SET IDENTITY_INSERT [dbo].[PAGOS] ON;
INSERT INTO [dbo].[PAGOS] 
([Id], [NombreCliente], [Total], [Meses], [ZonaId], [DiaPago], [Lotes], [FechaRegistro], [Estado], [FechaCreacion], [Telefonos], [Observacion]) 
VALUES
(1, 'Roberto Carlos Morales Sánchez', 180000.00, '36', 1, '15', 'Mz 2 Lt 14', '2025-01-15', '1', '2025-01-15 10:30:00', '2381012345', 'Cliente puntual, pagos quincenales acordados.'),
(2, 'Martha Patricia Gómez Velasco',  220000.00, '48', 2, '01', 'Mz 5 Lt 8',  '2025-02-01', '1', '2025-02-01 11:15:00', '2381234567', 'Traspaso de titular validado en enero.'),
(3, 'Francisco Javier Mendoza Ruiz',  120000.00, '12', 1, '10', 'Mz 1 Lt 3',  '2025-01-10', '2', '2025-01-10 09:00:00', '2381345678', 'CUENTA LIQUIDADA EN SU TOTALIDAD.'),
(4, 'María Elena Domínguez Castro',  310000.00, '60', 3, '20', 'Mz 7 Lt 20', '2025-01-20', '4', '2025-01-20 16:40:00', '2381456789', 'Atraso de 3 meses. Notificado vía telefónica.'),
(5, 'Juan Manuel Hernández Ortiz',   360000.00, '36', 4, '15', 'Mz 3 Lt 9 y 10', '2025-02-15', '1', '2025-02-15 12:00:00', '2381567890', 'Adquirió 2 lotes juntos. Paga por transferencia.'),
(6, 'Claudia Beatriz Ramos Luna',     195000.00, '30', 2, '05', 'Mz 8 Lt 1',  '2025-03-05', '1', '2025-03-05 14:10:00', '2381678901', 'Sin observaciones.'),
(7, 'Jorge Alberto Flores Medina',    175000.00, '24', 5, '15', 'Mz 4 Lt 17', '2025-01-15', '3', '2025-01-15 17:30:00', '2381789012', 'CANCELADO por solicitud del cliente por cambio de residencia.'),
(8, 'Verónica Guadalupe Silva Peña',  240000.00, '36', 3, '25', 'Mz 6 Lt 11', '2025-02-25', '1', '2025-02-25 10:00:00', '2381890123', 'Solicita boleta física cada mes.'),
(9, 'Ricardo Antonio Castillo Vega',  140000.00, '14', 1, '10', 'Mz 2 Lt 2',  '2025-01-10', '2', '2025-01-10 11:20:00', '2381901234', 'LIQUIDADO ANTICIPADAMENTE con descuento.'),
(10, 'Gabriela Isabel Torres Navarro', 280000.00, '48', 5, '30', 'Mz 9 Lt 15', '2025-01-30', '4', '2025-01-30 13:45:00', '2381112233', 'Prometió regularizar pagos este mes.');
SET IDENTITY_INSERT [dbo].[PAGOS] OFF;

-- Partidas de Pago (PAGOSPARTIDAS) para dbpagolotes_teh
-- Formas de pago: 0=MIGRADO, 1=EFECTIVO, 2=TRANSFERENCIA
SET IDENTITY_INSERT [dbo].[PAGOSPARTIDAS] ON;

-- Cliente 1: Roberto Carlos Morales (Total $180,000 - Al corriente, abonos de $5,000)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(1, 1, 20000.00, 0, '2025-01-15', '2025-01-15 10:35:00', 1, 1), -- Enganche efectivo
(2, 1, 5000.00,  0, '2025-02-15', '2025-02-15 11:00:00', 2, 1),
(3, 1, 5000.00,  0, '2025-03-15', '2025-03-15 10:20:00', 2, 2), -- Transferencia
(4, 1, 5000.00,  0, '2025-04-15', '2025-04-15 12:15:00', 5, 1),
(5, 1, 5000.00,  0, '2025-05-15', '2025-05-15 15:30:00', 5, 1);

-- Cliente 2: Martha Patricia Gómez (Total $220,000 - Al corriente)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(6, 2, 30000.00, 0, '2025-02-01', '2025-02-01 11:20:00', 1, 2), -- Enganche transferencia
(7, 2, 4500.00,  0, '2025-03-01', '2025-03-01 09:40:00', 5, 1),
(8, 2, 4500.00,  0, '2025-04-01', '2025-04-01 10:10:00', 5, 1),
(9, 2, 4500.00,  0, '2025-05-01', '2025-05-01 14:00:00', 5, 2);

-- Cliente 3: Francisco Javier Mendoza (Total $120,000 - PAGADO / LIQUIDADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(10, 3, 20000.00, 0, '2025-01-10', '2025-01-10 09:05:00', 1, 1),
(11, 3, 50000.00, 0, '2025-02-10', '2025-02-10 10:00:00', 1, 2),
(12, 3, 50000.00, 0, '2025-03-10', '2025-03-10 16:30:00', 1, 2); -- Finiquito completo

-- Cliente 4: María Elena Domínguez (Total $310,000 - ATRASADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(13, 4, 25000.00, 0, '2025-01-20', '2025-01-20 16:45:00', 1, 1),
(14, 4, 5000.00,  0, '2025-02-20', '2025-02-20 17:00:00', 5, 1); -- Dejó de pagar

-- Cliente 5: Juan Manuel Hernández (Total $360,000 - Al corriente, Lotes 9 y 10)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(15, 5, 50000.00, 0, '2025-02-15', '2025-02-15 12:05:00', 1, 2),
(16, 5, 10000.00, 0, '2025-03-15', '2025-03-15 11:30:00', 5, 2),
(17, 5, 10000.00, 0, '2025-04-15', '2025-04-15 13:00:00', 5, 2),
(18, 5, 10000.00, 0, '2025-05-15', '2025-05-15 10:45:00', 5, 2);

-- Cliente 6: Claudia Beatriz Ramos (Total $195,000 - Al corriente)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(19, 6, 25000.00, 0, '2025-03-05', '2025-03-05 14:15:00', 1, 1),
(20, 6, 6000.00,  0, '2025-04-05', '2025-04-05 11:20:00', 5, 1),
(21, 6, 6000.00,  0, '2025-05-05', '2025-05-05 12:10:00', 5, 1);

-- Cliente 7: Jorge Alberto Flores (Total $175,000 - CANCELADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(22, 7, 20000.00, 0, '2025-01-15', '2025-01-15 17:35:00', 1, 1);

-- Cliente 8: Verónica Guadalupe Silva (Total $240,000 - Al corriente)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(23, 8, 30000.00, 0, '2025-02-25', '2025-02-25 10:05:00', 1, 1),
(24, 8, 6500.00,  0, '2025-03-25', '2025-03-25 15:10:00', 2, 1),
(25, 8, 6500.00,  0, '2025-04-25', '2025-04-25 16:00:00', 5, 2);

-- Cliente 9: Ricardo Antonio Castillo (Total $140,000 - PAGADO / LIQUIDADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(26, 9, 40000.00,  0, '2025-01-10', '2025-01-10 11:25:00', 1, 1),
(27, 9, 100000.00, 0, '2025-02-10', '2025-02-10 12:40:00', 1, 2); -- Liquidación total

-- Cliente 10: Gabriela Isabel Torres (Total $280,000 - ATRASADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(28, 10, 25000.00, 0, '2025-01-30', '2025-01-30 13:50:00', 1, 1),
(29, 10, 5500.00,  0, '2025-02-28', '2025-02-28 11:15:00', 5, 1);

SET IDENTITY_INSERT [dbo].[PAGOSPARTIDAS] OFF;
GO

PRINT 'Base de datos [dbpagolotes_teh] sembrada correctamente.';
GO


-- ******************************************************************************
-- SECCIÓN 2: BASE DE DATOS [dbpagolotes_aja] (AJALPAN)
-- ******************************************************************************
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'dbpagolotes_aja')
BEGIN
    PRINT 'Creando base de datos [dbpagolotes_aja]...';
    CREATE DATABASE [dbpagolotes_aja];
END
GO

USE [dbpagolotes_aja];
GO

-- 2.1 Crear Esquema en dbpagolotes_aja
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USUARIOS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[USUARIOS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Usuario] NVARCHAR(150) NOT NULL UNIQUE,
        [Password] NVARCHAR(150) NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZONAS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZONAS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nombre] NVARCHAR(200) NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VARIABLESGLOBALES]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VARIABLESGLOBALES] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [label] NVARCHAR(150) NOT NULL UNIQUE,
        [valor] NVARCHAR(MAX) NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PAGOS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PAGOS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NombreCliente] NVARCHAR(250) NOT NULL,
        [Total] DECIMAL(18,2) NOT NULL,
        [Meses] NVARCHAR(50) NULL,
        [ZonaId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[ZONAS]([Id]),
        [DiaPago] NVARCHAR(50) NULL,
        [Lotes] NVARCHAR(250) NULL,
        [FechaRegistro] DATETIME NOT NULL,
        [Estado] NVARCHAR(50) NOT NULL DEFAULT '1',
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [Telefonos] NVARCHAR(150) NULL,
        [Observacion] NVARCHAR(MAX) NULL
    );
    CREATE NONCLUSTERED INDEX [IX_PAGOS_ZonaId] ON [dbo].[PAGOS] ([ZonaId]);
    CREATE NONCLUSTERED INDEX [IX_PAGOS_Estado] ON [dbo].[PAGOS] ([Estado]);
    CREATE NONCLUSTERED INDEX [IX_PAGOS_FechaRegistro] ON [dbo].[PAGOS] ([FechaRegistro]);
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PAGOSPARTIDAS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PAGOSPARTIDAS] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [PagoId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[PAGOS]([Id]) ON DELETE CASCADE,
        [Monto] DECIMAL(18,2) NOT NULL,
        [MontoOriginal] DECIMAL(18,2) NULL DEFAULT 0,
        [Fecha] DATETIME NOT NULL,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [UsuarioId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[USUARIOS]([Id]),
        [FormaPago] INT NULL,
        [FechaModificacion] DATETIME NULL,
        [UsuarioModificoId] INT NULL FOREIGN KEY REFERENCES [dbo].[USUARIOS]([Id]),
        [FechaBaja] DATETIME NULL,
        [UsuarioBajaId] INT NULL FOREIGN KEY REFERENCES [dbo].[USUARIOS]([Id])
    );
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_PagoId] ON [dbo].[PAGOSPARTIDAS] ([PagoId]);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_Fecha] ON [dbo].[PAGOSPARTIDAS] ([Fecha]);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_FechaCreacion] ON [dbo].[PAGOSPARTIDAS] ([FechaCreacion]);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_FechaBaja] ON [dbo].[PAGOSPARTIDAS] ([FechaBaja]);
END
GO

-- Función de zonas permitidas
CREATE OR ALTER FUNCTION [dbo].[fn_ZonasPermitidasPorUsuario] (@UsuarioId INT)
RETURNS TABLE
AS
RETURN
(
    SELECT CAST(z.value AS INT) AS [ZonaId]
    FROM [dbo].[VARIABLESGLOBALES] vg
    CROSS APPLY OPENJSON(vg.valor) 
    WITH (
        [UsuarioId] INT            '$.UsuarioId',
        [ZonasId]   NVARCHAR(MAX)  '$.ZonasId' AS JSON
    ) u
    CROSS APPLY OPENJSON(u.[ZonasId]) z
    WHERE vg.label = 'FltroZonas'
      AND u.[UsuarioId] = @UsuarioId
);
GO

-- 2.2 Limpiar y Sembrar Datos en dbpagolotes_aja
PRINT 'Sembrando datos en [dbpagolotes_aja]...';

-- Limpiar tablas
DELETE FROM [dbo].[PAGOSPARTIDAS];
DELETE FROM [dbo].[PAGOS];
DELETE FROM [dbo].[VARIABLESGLOBALES];
DELETE FROM [dbo].[ZONAS];
DELETE FROM [dbo].[USUARIOS];

DBCC CHECKIDENT ('[dbo].[USUARIOS]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[ZONAS]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PAGOS]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[PAGOSPARTIDAS]', RESEED, 0);

-- Usuarios con privilegios de sistema
SET IDENTITY_INSERT [dbo].[USUARIOS] ON;
INSERT INTO [dbo].[USUARIOS] ([Id], [Usuario], [Password]) VALUES
(1, 'ADMIN', 'admin123'),
(2, 'DIANA', 'diana123'),
(3, 'DONATO', 'donato123'),
(4, 'EMMANUEL', 'emmanuel123'),
(5, 'COBRANZA_AJA', 'caja123');
SET IDENTITY_INSERT [dbo].[USUARIOS] OFF;

-- Zonas de Ajalpan
SET IDENTITY_INSERT [dbo].[ZONAS] ON;
INSERT INTO [dbo].[ZONAS] ([Id], [Nombre]) VALUES
(1, 'Valle de Ajalpan Centro'),
(2, 'Fracc. Los Manantiales (Ajalpan)'),
(3, 'San Jerónimo Pantzingo'),
(4, 'Colonia Guadalupe Ajalpan'),
(5, 'Residencial San José Obrero');
SET IDENTITY_INSERT [dbo].[ZONAS] OFF;

-- Variables Globales (Filtro de zonas por usuario)
INSERT INTO [dbo].[VARIABLESGLOBALES] ([label], [valor]) VALUES
('FltroZonas', '[{"UsuarioId":1,"ZonasId":[1,2,3,4,5]},{"UsuarioId":2,"ZonasId":[1,2,3,4,5]},{"UsuarioId":3,"ZonasId":[1,2,3,4,5]},{"UsuarioId":4,"ZonasId":[1,2,3,4,5]},{"UsuarioId":5,"ZonasId":[1,2,4]}]');

-- 10 Clientes (PAGOS) en Ajalpan
SET IDENTITY_INSERT [dbo].[PAGOS] ON;
INSERT INTO [dbo].[PAGOS] 
([Id], [NombreCliente], [Total], [Meses], [ZonaId], [DiaPago], [Lotes], [FechaRegistro], [Estado], [FechaCreacion], [Telefonos], [Observacion]) 
VALUES
(1, 'Alejandro Ramírez Cordero',     160000.00, '36', 1, '15', 'Mz A Lt 5',    '2025-01-15', '1', '2025-01-15 09:30:00', '2361019988', 'Enganche liquidado en dos partes.'),
(2, 'Silvia Leticia Morales Cruz',     190000.00, '24', 2, '01', 'Mz C Lt 12',   '2025-02-01', '1', '2025-02-01 10:15:00', '2361028877', 'Paga directo en ventanilla en efectivo.'),
(3, 'Héctor Manuel Aguilar Reyes',     120000.00, '18', 1, '10', 'Mz B Lt 1',    '2025-01-10', '2', '2025-01-10 09:00:00', '2361037766', 'LIQUIDADO COMPLETO. En proceso de escrituración.'),
(4, 'Rosa María Fuentes Méndez',       250000.00, '48', 3, '20', 'Mz D Lt 8',    '2025-01-20', '4', '2025-01-20 15:40:00', '2361046655', 'ATRASADO 2 meses. Se acordó prórroga.'),
(5, 'Carlos Eduardo Ponce Valdés',     300000.00, '36', 4, '15', 'Mz E Lt 3 y 4','2025-02-15', '1', '2025-02-15 11:00:00', '2361055544', 'Compró lote doble para taller.'),
(6, 'Ana Karen Gutiérrez Salgado',     170000.00, '30', 2, '05', 'Mz B Lt 14',   '2025-03-05', '1', '2025-03-05 13:10:00', '2361064433', 'Sin observaciones.'),
(7, 'Miguel Ángel Vargas Benítez',     145000.00, '24', 5, '15', 'Mz F Lt 7',    '2025-01-15', '3', '2025-01-15 16:30:00', '2361073322', 'CANCELADO por no convenir a intereses familiares.'),
(8, 'Laura Elena Cárdenas Rosas',      210000.00, '36', 3, '25', 'Mz C Lt 9',    '2025-02-25', '1', '2025-02-25 10:00:00', '2361082211', 'Cliente muy puntual con sus recibos.'),
(9, 'Fernando Javier Osorio León',     135000.00, '20', 1, '10', 'Mz A Lt 2',    '2025-01-10', '2', '2025-01-10 11:20:00', '2361091100', 'LIQUIDACIÓN TOTAL recibida.'),
(10, 'Teresa de Jesús Pacheco Lara',   260000.00, '48', 5, '30', 'Mz G Lt 10',   '2025-01-30', '4', '2025-01-30 14:45:00', '2361100099', 'Presenta retraso desde marzo.');
SET IDENTITY_INSERT [dbo].[PAGOS] OFF;

-- Partidas de Pago (PAGOSPARTIDAS) para dbpagolotes_aja
SET IDENTITY_INSERT [dbo].[PAGOSPARTIDAS] ON;

-- Cliente 1: Alejandro Ramírez (Total $160,000 - Al corriente)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(1, 1, 15000.00, 0, '2025-01-15', '2025-01-15 09:35:00', 1, 1),
(2, 1, 4500.00,  0, '2025-02-15', '2025-02-15 10:10:00', 5, 1),
(3, 1, 4500.00,  0, '2025-03-15', '2025-03-15 11:00:00', 5, 1),
(4, 1, 4500.00,  0, '2025-04-15', '2025-04-15 11:45:00', 5, 2),
(5, 1, 4500.00,  0, '2025-05-15', '2025-05-15 14:30:00', 5, 1);

-- Cliente 2: Silvia Leticia Morales (Total $190,000 - Al corriente)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(6, 2, 20000.00, 0, '2025-02-01', '2025-02-01 10:20:00', 1, 1),
(7, 2, 7000.00,  0, '2025-03-01', '2025-03-01 10:50:00', 5, 1),
(8, 2, 7000.00,  0, '2025-04-01', '2025-04-01 12:15:00', 5, 1),
(9, 2, 7000.00,  0, '2025-05-01', '2025-05-01 15:00:00', 5, 1);

-- Cliente 3: Héctor Manuel Aguilar (Total $120,000 - PAGADO / LIQUIDADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(10, 3, 20000.00, 0, '2025-01-10', '2025-01-10 09:10:00', 1, 1),
(11, 3, 50000.00, 0, '2025-02-10', '2025-02-10 11:30:00', 1, 2),
(12, 3, 50000.00, 0, '2025-03-10', '2025-03-10 15:00:00', 1, 2);

-- Cliente 4: Rosa María Fuentes (Total $250,000 - ATRASADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(13, 4, 25000.00, 0, '2025-01-20', '2025-01-20 15:45:00', 1, 1),
(14, 4, 5000.00,  0, '2025-02-20', '2025-02-20 16:10:00', 5, 1);

-- Cliente 5: Carlos Eduardo Ponce (Total $300,000 - Al corriente, Lotes 3 y 4)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(15, 5, 40000.00, 0, '2025-02-15', '2025-02-15 11:10:00', 1, 2),
(16, 5, 8000.00,  0, '2025-03-15', '2025-03-15 12:00:00', 5, 2),
(17, 5, 8000.00,  0, '2025-04-15', '2025-04-15 12:40:00', 5, 2),
(18, 5, 8000.00,  0, '2025-05-15', '2025-05-15 13:20:00', 5, 2);

-- Cliente 6: Ana Karen Gutiérrez (Total $170,000 - Al corriente)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(19, 6, 20000.00, 0, '2025-03-05', '2025-03-05 13:15:00', 1, 1),
(20, 6, 5000.00,  0, '2025-04-05', '2025-04-05 10:30:00', 5, 1),
(21, 6, 5000.00,  0, '2025-05-05', '2025-05-05 11:50:00', 5, 1);

-- Cliente 7: Miguel Ángel Vargas (Total $145,000 - CANCELADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(22, 7, 15000.00, 0, '2025-01-15', '2025-01-15 16:35:00', 1, 1);

-- Cliente 8: Laura Elena Cárdenas (Total $210,000 - Al corriente)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(23, 8, 25000.00, 0, '2025-02-25', '2025-02-25 10:10:00', 1, 1),
(24, 8, 6000.00,  0, '2025-03-25', '2025-03-25 14:20:00', 2, 1),
(25, 8, 6000.00,  0, '2025-04-25', '2025-04-25 15:30:00', 5, 2);

-- Cliente 9: Fernando Javier Osorio (Total $135,000 - PAGADO / LIQUIDADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(26, 9, 35000.00,  0, '2025-01-10', '2025-01-10 11:25:00', 1, 1),
(27, 9, 100000.00, 0, '2025-02-10', '2025-02-10 13:00:00', 1, 2);

-- Cliente 10: Teresa de Jesús Pacheco (Total $260,000 - ATRASADO)
INSERT INTO [dbo].[PAGOSPARTIDAS] ([Id], [PagoId], [Monto], [MontoOriginal], [Fecha], [FechaCreacion], [UsuarioId], [FormaPago]) VALUES
(28, 10, 20000.00, 0, '2025-01-30', '2025-01-30 14:50:00', 1, 1),
(29, 10, 5000.00,  0, '2025-02-28', '2025-02-28 10:40:00', 5, 1);

SET IDENTITY_INSERT [dbo].[PAGOSPARTIDAS] OFF;
GO

PRINT 'Base de datos [dbpagolotes_aja] sembrada correctamente.';
GO
