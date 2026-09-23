# Guía de Base de Datos: Esquema General y Seeders de Prueba

Este directorio contiene los scripts SQL diseñados específicamente para el sistema **ControlPagoLotes**.

---

## 📁 Contenido de Archivos

| Archivo | Motor | Propósito |
|---|---|---|
| [01_schema_general.sql](file:///c:/Users/crazy/Source/Repos/ControlPagoLotes/Database/01_schema_general.sql) | SQL Server | DDL completo e idempotente del esquema: tablas, restricciones, índices y funciones. |
| [02_seeder_test_databases.sql](file:///c:/Users/crazy/Source/Repos/ControlPagoLotes/Database/02_seeder_test_databases.sql) | SQL Server | Crea y siembra las bases de datos `dbpagolotes_teh` y `dbpagolotes_aja` con 10 clientes y múltiples pagos cada una. |
| [03_config_sqlite_connections.sql](file:///c:/Users/crazy/Source/Repos/ControlPagoLotes/Database/03_config_sqlite_connections.sql) | SQLite | Registra ambas conexiones en `%APPDATA%\jaadeproductions\db_conexiones.sqlite`. |

---

## 🏗️ 1. Esquema de Base de Datos (SQL Server)

El modelo de datos implementa la estructura requerida por las capas **DAO**, **Entidades** y **LOGICA**:

1. **`USUARIOS`**:
   - `Id` (INT IDENTITY PRIMARY KEY)
   - `Usuario` (NVARCHAR(150) NOT NULL UNIQUE)
   - `Password` (NVARCHAR(150) NOT NULL)

2. **`ZONAS`**:
   - `Id` (INT IDENTITY PRIMARY KEY)
   - `Nombre` (NVARCHAR(200) NOT NULL)

3. **`VARIABLESGLOBALES`**:
   - `Id` (INT IDENTITY PRIMARY KEY)
   - `label` (NVARCHAR(150) NOT NULL UNIQUE)
   - `valor` (NVARCHAR(MAX) NOT NULL)
   - *Nota*: Almacena la variable `'FltroZonas'` en formato JSON para asignar zonas visibles por cada usuario.

4. **`PAGOS`** (Cuentas de Lotes):
   - `Id` (INT IDENTITY PRIMARY KEY)
   - `NombreCliente` (NVARCHAR(250) NOT NULL)
   - `Total` (DECIMAL(18,2) NOT NULL)
   - `Meses` (NVARCHAR(50) NULL)
   - `ZonaId` (INT NOT NULL, FK -> `ZONAS(Id)`)
   - `DiaPago` (NVARCHAR(50) NULL)
   - `Lotes` (NVARCHAR(250) NULL)
   - `FechaRegistro` (DATETIME NOT NULL)
   - `Estado` (NVARCHAR(50) NOT NULL):
     - `'1'` = AL CORRIENTE
     - `'2'` = PAGADO (LIQUIDADO)
     - `'3'` = CANCELADO
     - `'4'` = ATRASADO
   - `FechaCreacion` (DATETIME NOT NULL DEFAULT GETDATE())
   - `Telefonos` (NVARCHAR(150) NULL)
   - `Observacion` (NVARCHAR(MAX) NULL)

5. **`PAGOSPARTIDAS`** (Abonos / Partidas):
   - `Id` (INT IDENTITY PRIMARY KEY)
   - `PagoId` (INT NOT NULL, FK -> `PAGOS(Id)`)
   - `Monto` (DECIMAL(18,2) NOT NULL)
   - `MontoOriginal` (DECIMAL(18,2) NULL)
   - `Fecha` (DATETIME NOT NULL)
   - `FechaCreacion` (DATETIME NOT NULL DEFAULT GETDATE())
   - `UsuarioId` (INT NOT NULL, FK -> `USUARIOS(Id)`)
   - `FormaPago` (INT NULL):
     - `0` = MIGRADO
     - `1` = EFECTIVO
     - `2` = TRANSFERENCIA
   - `FechaModificacion` (DATETIME NULL)
   - `UsuarioModificoId` (INT NULL, FK -> `USUARIOS(Id)`)
   - `FechaBaja` (DATETIME NULL)
   - `UsuarioBajaId` (INT NULL, FK -> `USUARIOS(Id)`)

6. **Función de Tabla `dbo.fn_ZonasPermitidasPorUsuario`**:
   - Desempaqueta el JSON de `VARIABLESGLOBALES` (`label = 'FltroZonas'`) usando `OPENJSON` para filtrar las zonas autorizadas del usuario que inició sesión.

---

## 🚀 2. Instrucciones de Ejecución

### Opción A: Desde SQL Server Management Studio (SSMS) o Azure Data Studio
1. Abre **SSMS** y conéctate a tu servidor SQL Server (local o remoto).
2. Abre el archivo [02_seeder_test_databases.sql](file:///c:/Users/crazy/Source/Repos/ControlPagoLotes/Database/02_seeder_test_databases.sql).
3. Presiona **Execute** (F5).
4. El script creará automáticamente las dos bases de datos (`dbpagolotes_teh` y `dbpagolotes_aja`), aplicará el esquema y sembrará los 10 clientes con sus partidas de pago correspondientes.

### Opción B: Desde Línea de Comandos (sqlcmd)
```powershell
# Usando autenticación de Windows en localhost:
sqlcmd -S . -E -i "c:\Users\crazy\Source\Repos\ControlPagoLotes\Database\02_seeder_test_databases.sql"
```

---

## 👥 3. Usuarios de Prueba Disponibles

Ambas bases de datos incluyen los siguientes usuarios:

| Usuario | Contraseña | Perfil / Permisos |
|---|---|---|
| `ADMIN` | `admin123` | Administrador total (acceso a todos los módulos y conexiones) |
| `DIANA` | `diana123` | Administrador total |
| `DONATO` | `donato123` | Administrador total |
| `EMMANUEL` | `emmanuel123` | Administrador total |
| `COBRANZA_TEH` / `COBRANZA_AJA` | `caja123` | Operativo / Caja |

---

## 🔄 4. Conectar la Aplicación a las Bases de Datos

El sistema **ControlPagoLotes** gestiona sus conexiones en un archivo SQLite ubicado en `%APPDATA%\jaadeproductions\db_conexiones.sqlite`.

Puedes configurar las conexiones de dos formas:

1. **Desde la interfaz de la aplicación**:
   - Inicia sesión como `ADMIN`.
   - Entra al botón **Conexiones** en la barra superior o menú.
   - Agrega una conexión con etiqueta `Tehuacán` y base de datos `dbpagolotes_teh`, márcala como principal.
   - Agrega otra conexión con etiqueta `Ajalpan` y base de datos `dbpagolotes_aja`.
   
2. **Mediante el script SQLite**:
   - Ejecuta [03_config_sqlite_connections.sql](file:///c:/Users/crazy/Source/Repos/ControlPagoLotes/Database/03_config_sqlite_connections.sql) directamente sobre tu archivo `%APPDATA%\jaadeproductions\db_conexiones.sqlite` usando cualquier visor de SQLite (e.g., DB Browser for SQLite).
