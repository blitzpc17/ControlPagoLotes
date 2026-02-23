using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace DAO
{
    public class GenericRepository : IDisposable
    {
        private readonly string _sqlitePath;
        private bool _disposed = false;
        private IDbConnection _connection; // conexión SQL Server viva (lazy)

        // -----------------------------
        // CTOR: usa SQLite local para obtener la conexión principal
        // -----------------------------
        public GenericRepository()
        {
            // Puedes ajustar "TuApp" al nombre real de tu app
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "jaadeproductions");
            Directory.CreateDirectory(dir);

            _sqlitePath = Path.Combine(dir, "db_conexiones.sqlite");

            // Asegura que exista la BD local y la tabla
            EnsureLocalDb();
        }

        // CTOR alterno si quieres pasarle manualmente dónde está el sqlite
        public GenericRepository(string sqlitePath, bool ensureDb = true)
        {
            if (string.IsNullOrWhiteSpace(sqlitePath))
                throw new ArgumentException("sqlitePath no puede venir vacío");

            _sqlitePath = sqlitePath;

            if (ensureDb)
                EnsureLocalDb();
        }

        // -----------------------------
        // Obtiene el connection string principal desde SQLite
        // -----------------------------
        private string GetDefaultConnectionString()
        {
            using (var cn = new SQLiteConnection($"Data Source={_sqlitePath};Version=3;"))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT conn_string
FROM connections
WHERE is_default = 1
LIMIT 1;";

                    var cs = cmd.ExecuteScalar() as string;

                    if (string.IsNullOrWhiteSpace(cs))
                        throw new InvalidOperationException("No hay una conexión principal configurada. Abre el formulario de Conexiones y marca una como principal.");

                    return cs;
                }
            }
        }

        // -----------------------------
        // Lazy connection a SQL Server (usa SIEMPRE la conexión principal)
        // -----------------------------
        private IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    var cs = GetDefaultConnectionString();
                    _connection = new SqlConnection(cs);
                }

                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return _connection;
            }
        }

        // Útil si cambiaste la principal y quieres que el repo "tome" la nueva
        public void ReloadConnection()
        {
            if (_connection != null)
            {
                try
                {
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                }
                catch { /* ignore */ }

                _connection.Dispose();
                _connection = null;
            }
        }

        // Para diagnóstico: ver cuál es la actual (principal) guardada en SQLite
        public string GetCurrentConnectionString()
        {
            return GetDefaultConnectionString();
        }

        // -----------------------------
        // Dapper CRUD helpers
        // -----------------------------
        public int Execute(string sql, object parameters = null)
        {
            try
            {
                return Connection.Execute(sql, parameters);
            }
            catch (SqlException ex)
            {
                LogError("Execute", sql, ex);
                throw;
            }
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            try
            {
                return Connection.Query<T>(sql, parameters).ToList();
            }
            catch (SqlException ex)
            {
                LogError("Query", sql, ex);
                throw;
            }
        }

        public T QuerySingle<T>(string sql, object parameters = null)
        {
            try
            {
                return Connection.QueryFirstOrDefault<T>(sql, parameters);
            }
            catch (SqlException ex)
            {
                LogError("QuerySingle", sql, ex);
                throw;
            }
        }

        public int ExecuteScalar(string sql, object parameters = null)
        {
            try
            {
                return Connection.ExecuteScalar<int>(sql, parameters);
            }
            catch (SqlException ex)
            {
                LogError("ExecuteScalar", sql, ex);
                throw;
            }
        }

        // -----------------------------
        // Diagnóstico / Test conexión
        // -----------------------------
        public bool TestConnection()
        {
            try
            {
                var cs = GetDefaultConnectionString();

                using (var testConn = new SqlConnection(cs))
                {
                    testConn.Open();
                    var version = testConn.ServerVersion;
                    Console.WriteLine($"Conexión exitosa. SQL Server v{version}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de conexión: {ex.Message}");
                return false;
            }
        }

        public string GetServerInfo()
        {
            try
            {
                var cs = GetDefaultConnectionString();

                using (var conn = new SqlConnection(cs))
                {
                    conn.Open();
                    return $"Server: {conn.DataSource}\n" +
                           $"Database: {conn.Database}\n" +
                           $"Version: {conn.ServerVersion}\n" +
                           $"State: {conn.State}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public void Diagnose()
        {
            Console.WriteLine("=== DIAGNÓSTICO ===");
            Console.WriteLine($"SQLite Path: {_sqlitePath}");

            try
            {
                var cs = GetDefaultConnectionString();
                Console.WriteLine($"Connection String (principal): {cs}");
                Console.WriteLine($"Database in string: {GetDatabaseFromConnectionString(cs)}");

                TestConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}: {ex.Message}");

                if (ex is SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error Number: {sqlEx.Number}");
                    Console.WriteLine($"Server: {sqlEx.Server}");
                }
            }
        }

        private string GetDatabaseFromConnectionString(string connString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connString);
                return builder.InitialCatalog;
            }
            catch
            {
                return "No se pudo parsear";
            }
        }

        private void LogError(string method, string sql, SqlException ex)
        {
            Console.WriteLine($"[ERROR] Método: {method}");
            Console.WriteLine($"[ERROR] SQL: {sql}");
            Console.WriteLine($"[ERROR] Número: {ex.Number}");
            Console.WriteLine($"[ERROR] Mensaje: {ex.Message}");
            Console.WriteLine($"[ERROR] Servidor: {ex.Server}");
        }

        // -----------------------------
        // Local SQLite bootstrap
        // -----------------------------
        private void EnsureLocalDb()
        {
            if (!File.Exists(_sqlitePath))
                SQLiteConnection.CreateFile(_sqlitePath);

            using (var cn = new SQLiteConnection($"Data Source={_sqlitePath};Version=3;"))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS connections(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  label TEXT NOT NULL,
  conn_string TEXT NOT NULL,
  is_default INTEGER NOT NULL DEFAULT 0,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_connections_default
ON connections(is_default)
WHERE is_default = 1;";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // -----------------------------
        // IDisposable
        // -----------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        try
                        {
                            if (_connection.State == ConnectionState.Open)
                                _connection.Close();
                        }
                        catch { /* ignore */ }

                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~GenericRepository()
        {
            Dispose(false);
        }
    }

}