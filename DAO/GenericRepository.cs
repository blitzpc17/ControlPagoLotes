using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DAO
{
    public class GenericRepository : IDisposable
    {
        private readonly string _sqlitePath;
        private bool _disposed = false;
        private IDbConnection _connection; // conexión SQL Server viva (lazy)
        private readonly string _explicitConnectionString;

        public long CurrentConnectionId { get; set; }
        public string CurrentPlaza { get; set; }

        public static string GetLocalSqlitePath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "jaadeproductions");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "db_conexiones.sqlite");
        }

        public static List<long> InactiveConnectionIds = new List<long>();

        public static List<(long Id, string Label, string ConnString, bool IsDefault)> GetAvailableConnections(bool includeInactive = false)
        {
            var path = GetLocalSqlitePath();
            if (!File.Exists(path)) return new List<(long, string, string, bool)>();

            var list = new List<(long, string, string, bool)>();
            try
            {
                using (var cn = new SQLiteConnection($"Data Source={path};Version=3;"))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT id, label, conn_string, is_default FROM connections ORDER BY is_default DESC, label;";
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                var id = rd.GetInt64(0);
                                if (includeInactive || !InactiveConnectionIds.Contains(id))
                                {
                                    list.Add((id, rd.GetString(1), rd.GetString(2), rd.GetInt32(3) == 1));
                                }
                            }
                        }
                    }
                }
            }
            catch { /* ignore */ }
            return list;
        }

        public static async Task<List<string>> CheckAndDisableOfflineConnectionsAsync()
        {
            var offlineLabels = new List<string>();
            var all = GetAvailableConnections(includeInactive: true); 

            var tasks = all.Select(async conn => 
            {
                 var builder = new SqlConnectionStringBuilder(conn.ConnString);
                 builder.ConnectTimeout = 3; 
                 try 
                 {
                     using(var sql = new SqlConnection(builder.ConnectionString)) 
                     {
                         await sql.OpenAsync();
                     }
                 }
                 catch 
                 {
                     lock(InactiveConnectionIds) {
                         if (!InactiveConnectionIds.Contains(conn.Id)) 
                             InactiveConnectionIds.Add(conn.Id);
                     }
                     lock(offlineLabels) {
                         offlineLabels.Add(conn.Label);
                     }
                 }
            });
            
            await Task.WhenAll(tasks);
            return offlineLabels;
        }

        public static async Task<bool> PingConnectionAsync(long connectionId)
        {
            var info = GetConnectionById(connectionId);
            if (info == null) return false;

            var builder = new SqlConnectionStringBuilder(info.Value.ConnString);
            builder.ConnectTimeout = 3;
            try
            {
                using (var cn = new SqlConnection(builder.ConnectionString))
                {
                    await cn.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static (long Id, string Label, string ConnString, bool IsDefault)? GetConnectionById(long connectionId)
        {
            var path = GetLocalSqlitePath();
            if (!File.Exists(path)) return null;

            try
            {
                using (var cn = new SQLiteConnection($"Data Source={path};Version=3;"))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT id, label, conn_string, is_default FROM connections WHERE id = @id LIMIT 1;";
                        cmd.Parameters.AddWithValue("@id", connectionId);
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                return (rd.GetInt64(0), rd.GetString(1), rd.GetString(2), rd.GetInt32(3) == 1);
                            }
                        }
                    }
                }
            }
            catch { /* ignore */ }
            return null;
        }

        public static (long Id, string Label, string ConnString, bool IsDefault)? GetDefaultConnectionInfo()
        {
            var path = GetLocalSqlitePath();
            if (!File.Exists(path)) return null;

            try
            {
                using (var cn = new SQLiteConnection($"Data Source={path};Version=3;"))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT id, label, conn_string, is_default FROM connections WHERE is_default = 1 LIMIT 1;";
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                return (rd.GetInt64(0), rd.GetString(1), rd.GetString(2), true);
                            }
                        }
                    }
                }
            }
            catch { /* ignore */ }
            return null;
        }

        // -----------------------------
        // CTOR: usa SQLite local para obtener la conexión principal
        // -----------------------------
        public GenericRepository()
        {
            _sqlitePath = GetLocalSqlitePath();
            EnsureLocalDb();

            var def = GetDefaultConnectionInfo();
            if (def.HasValue)
            {
                CurrentConnectionId = def.Value.Id;
                CurrentPlaza = def.Value.Label;
            }
        }

        // CTOR por connectionId configurada en SQLite
        public GenericRepository(long connectionId)
        {
            _sqlitePath = GetLocalSqlitePath();
            EnsureLocalDb();

            var info = GetConnectionById(connectionId);
            if (info.HasValue)
            {
                _explicitConnectionString = info.Value.ConnString;
                CurrentConnectionId = info.Value.Id;
                CurrentPlaza = info.Value.Label;
            }
            else
            {
                var def = GetDefaultConnectionInfo();
                if (def.HasValue)
                {
                    CurrentConnectionId = def.Value.Id;
                    CurrentPlaza = def.Value.Label;
                }
            }
        }

        // CTOR alterno con cadena de conexión SQL Server directa
        public GenericRepository(string explicitConnectionString, string plaza = null, long connectionId = 0)
        {
            _explicitConnectionString = explicitConnectionString;
            CurrentPlaza = plaza;
            CurrentConnectionId = connectionId;
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
        // Lazy connection a SQL Server
        // -----------------------------
        private IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    var cs = !string.IsNullOrWhiteSpace(_explicitConnectionString)
                        ? _explicitConnectionString
                        : GetDefaultConnectionString();
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