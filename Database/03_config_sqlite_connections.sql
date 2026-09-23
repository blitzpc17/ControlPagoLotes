-- ==============================================================================
-- SISTEMA: CONTROL DE PAGO DE LOTES (ControlPagoLotes)
-- ARCHIVO: 03_config_sqlite_connections.sql
-- MOTOR: SQLite
-- RUTA LOCAL DEL ARCHIVO: %APPDATA%\jaadeproductions\db_conexiones.sqlite
-- DESCRIPCIÓN: Registra ambas bases de datos (dbpagolotes_teh y dbpagolotes_aja)
--              en el catálogo de conexiones locales de la aplicación WinForms.
-- ==============================================================================

-- 1. Crear tabla si no existe (mismo esquema que genera GenericRepository)
CREATE TABLE IF NOT EXISTS connections (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  label TEXT NOT NULL,
  conn_string TEXT NOT NULL,
  is_default INTEGER NOT NULL DEFAULT 0,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_connections_default
ON connections(is_default)
WHERE is_default = 1;

-- 2. Limpiar registros previos de prueba
DELETE FROM connections WHERE label IN ('Tehuacán (dbpagolotes_teh)', 'Ajalpan (dbpagolotes_aja)');

-- 3. Insertar conexión Tehuacán (Marcada como Principal / is_default = 1)
-- NOTA: Ajusta 'Data Source=.' o 'Data Source=localhost,1433' según tu instancia de SQL Server.
INSERT INTO connections (label, conn_string, is_default, created_at, updated_at)
VALUES (
  'Tehuacán (dbpagolotes_teh)',
  'Data Source=.;Initial Catalog=dbpagolotes_teh;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;',
  1,
  datetime('now'),
  datetime('now')
);

-- 4. Insertar conexión Ajalpan (is_default = 0)
INSERT INTO connections (label, conn_string, is_default, created_at, updated_at)
VALUES (
  'Ajalpan (dbpagolotes_aja)',
  'Data Source=.;Initial Catalog=dbpagolotes_aja;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;',
  0,
  datetime('now'),
  datetime('now')
);
