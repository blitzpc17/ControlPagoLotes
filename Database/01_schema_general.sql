-- ==============================================================================
-- SISTEMA: CONTROL DE PAGO DE LOTES (ControlPagoLotes)
-- ARCHIVO: 01_schema_general.sql
-- DESCRIPCIÓN: Esquema General de Base de Datos para Microsoft SQL Server (2016+)
-- ==============================================================================

-- 1. TABLA: USUARIOS
-- Almacena los usuarios del sistema para inicio de sesión y auditoría de movimientos.
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USUARIOS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[USUARIOS] (
        [Id]       INT IDENTITY(1,1) NOT NULL,
        [Usuario]  NVARCHAR(150)     NOT NULL,
        [Password] NVARCHAR(150)     NOT NULL,
        CONSTRAINT [PK_USUARIOS] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_USUARIOS_Usuario] UNIQUE NONCLUSTERED ([Usuario] ASC)
    );
END
GO

-- 2. TABLA: ZONAS
-- Almacena las lotificaciones, colonias, fraccionamientos o rutas.
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZONAS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZONAS] (
        [Id]     INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(200)     NOT NULL,
        CONSTRAINT [PK_ZONAS] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 3. TABLA: VARIABLESGLOBALES
-- Almacena parámetros globales de configuración, incluyendo el filtro de rutas por usuario en formato JSON.
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VARIABLESGLOBALES]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VARIABLESGLOBALES] (
        [Id]    INT IDENTITY(1,1) NOT NULL,
        [label] NVARCHAR(150)     NOT NULL,
        [valor] NVARCHAR(MAX)     NOT NULL,
        CONSTRAINT [PK_VARIABLESGLOBALES] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_VARIABLESGLOBALES_label] UNIQUE NONCLUSTERED ([label] ASC)
    );
END
GO

-- 4. TABLA: PAGOS
-- Representa el contrato o cuenta principal del cliente para uno o varios lotes.
-- Estados soportados en la aplicación:
--   '1' = AL CORRIENTE
--   '2' = PAGADO (LIQUIDADO)
--   '3' = CANCELADO
--   '4' = ATRASADO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PAGOS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PAGOS] (
        [Id]            INT IDENTITY(1,1) NOT NULL,
        [NombreCliente] NVARCHAR(250)     NOT NULL,
        [Total]         DECIMAL(18,2)     NOT NULL,
        [Meses]         NVARCHAR(50)      NULL,
        [ZonaId]        INT               NOT NULL,
        [DiaPago]       NVARCHAR(50)      NULL,
        [Lotes]         NVARCHAR(250)     NULL,
        [FechaRegistro] DATETIME          NOT NULL,
        [Estado]        NVARCHAR(50)      NOT NULL DEFAULT '1',
        [FechaCreacion] DATETIME          NOT NULL DEFAULT GETDATE(),
        [Telefonos]     NVARCHAR(150)     NULL,
        [Observacion]   NVARCHAR(MAX)     NULL,
        CONSTRAINT [PK_PAGOS] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_PAGOS_ZONAS] FOREIGN KEY ([ZonaId]) REFERENCES [dbo].[ZONAS] ([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_PAGOS_ZonaId] ON [dbo].[PAGOS] ([ZonaId] ASC);
    CREATE NONCLUSTERED INDEX [IX_PAGOS_Estado] ON [dbo].[PAGOS] ([Estado] ASC);
    CREATE NONCLUSTERED INDEX [IX_PAGOS_FechaRegistro] ON [dbo].[PAGOS] ([FechaRegistro] ASC);
    CREATE NONCLUSTERED INDEX [IX_PAGOS_NombreCliente] ON [dbo].[PAGOS] ([NombreCliente] ASC);
END
GO

-- 5. TABLA: PAGOSPARTIDAS
-- Registra cada abono, mensualidad o partida realizada a una cuenta de lote.
-- Formas de pago:
--   0 = MIGRADO
--   1 = EFECTIVO
--   2 = TRANSFERENCIA
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PAGOSPARTIDAS]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PAGOSPARTIDAS] (
        [Id]                INT IDENTITY(1,1) NOT NULL,
        [PagoId]            INT               NOT NULL,
        [Monto]             DECIMAL(18,2)     NOT NULL,
        [MontoOriginal]     DECIMAL(18,2)     NULL DEFAULT 0,
        [Fecha]             DATETIME          NOT NULL, -- Fecha del comprobante
        [FechaCreacion]     DATETIME          NOT NULL DEFAULT GETDATE(), -- Fecha de captura en sistema
        [UsuarioId]         INT               NOT NULL, -- Usuario/cajero que recibe
        [FormaPago]         INT               NULL,     -- 0=MIGRADO, 1=EFECTIVO, 2=TRANSFERENCIA
        [FechaModificacion] DATETIME          NULL,
        [UsuarioModificoId] INT               NULL,
        [FechaBaja]         DATETIME          NULL,     -- Baja lógica
        [UsuarioBajaId]     INT               NULL,
        CONSTRAINT [PK_PAGOSPARTIDAS] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_PAGOSPARTIDAS_PAGOS] FOREIGN KEY ([PagoId]) REFERENCES [dbo].[PAGOS] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PAGOSPARTIDAS_USUARIOS_RECIBE] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[USUARIOS] ([Id]),
        CONSTRAINT [FK_PAGOSPARTIDAS_USUARIOS_MODIFICA] FOREIGN KEY ([UsuarioModificoId]) REFERENCES [dbo].[USUARIOS] ([Id]),
        CONSTRAINT [FK_PAGOSPARTIDAS_USUARIOS_BAJA] FOREIGN KEY ([UsuarioBajaId]) REFERENCES [dbo].[USUARIOS] ([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_PagoId] ON [dbo].[PAGOSPARTIDAS] ([PagoId] ASC);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_Fecha] ON [dbo].[PAGOSPARTIDAS] ([Fecha] ASC);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_FechaCreacion] ON [dbo].[PAGOSPARTIDAS] ([FechaCreacion] ASC);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_UsuarioId] ON [dbo].[PAGOSPARTIDAS] ([UsuarioId] ASC);
    CREATE NONCLUSTERED INDEX [IX_PAGOSPARTIDAS_FechaBaja] ON [dbo].[PAGOSPARTIDAS] ([FechaBaja] ASC);
END
GO

-- 6. FUNCIÓN DE TABLA: dbo.fn_ZonasPermitidasPorUsuario
-- Requerida por PagosRepository, PagoPartidasRepository y ZonasRepository.
-- Lee el JSON de la variable global 'FltroZonas' y extrae las zonas asignadas al usuario.
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
