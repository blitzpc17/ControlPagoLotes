-- ============================================================================
-- SCRIPT DE SINCRONIZACIÓN DE USUARIOS ENTRE PLAZAS (PRODUCCIÓN)
-- ============================================================================
-- Este script permite sincronizar los usuarios existentes entre las bases de
-- datos de dos plazas (ej. dbpagolotes_teh y dbpagolotes_aja).
-- Si un usuario existe en una plaza pero no en la otra, lo inserta conservando
-- su Usuario y Password.
-- ============================================================================

-- 1. Sincronizar de TEHUACÁN hacia AJALPAN:
USE [dbpagolotes_aja];
GO

PRINT 'Sincronizando usuarios de Tehuacán hacia Ajalpan...';
INSERT INTO [dbo].[USUARIOS] ([Usuario], [Password])
SELECT t.[Usuario], t.[Password]
FROM [dbpagolotes_teh].[dbo].[USUARIOS] t
WHERE NOT EXISTS (
    SELECT 1 
    FROM [dbo].[USUARIOS] a 
    WHERE UPPER(a.[Usuario]) = UPPER(t.[Usuario])
);
PRINT 'Usuarios sincronizados hacia Ajalpan: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

-- 2. Sincronizar de AJALPAN hacia TEHUACÁN:
USE [dbpagolotes_teh];
GO

PRINT 'Sincronizando usuarios de Ajalpan hacia Tehuacán...';
INSERT INTO [dbo].[USUARIOS] ([Usuario], [Password])
SELECT a.[Usuario], a.[Password]
FROM [dbpagolotes_aja].[dbo].[USUARIOS] a
WHERE NOT EXISTS (
    SELECT 1 
    FROM [dbo].[USUARIOS] t 
    WHERE UPPER(t.[Usuario]) = UPPER(a.[Usuario])
);
PRINT 'Usuarios sincronizados hacia Tehuacán: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

PRINT '=== SINCRONIZACIÓN COMPLETADA CON ÉXITO ===';
GO
