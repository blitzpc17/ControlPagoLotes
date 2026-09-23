using System;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace ControlPagoLotes
{
    public static class ConnectionStorage
    {
        public static bool HasDefaultConnection(string sqlitePath)
        {
            try
            {
                using (var cn = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = @"
SELECT COUNT(1)
FROM connections
WHERE is_default = 1;";
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetDefaultConnectionString(string sqlitePath)
        {
            using (var cn = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
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
                        throw new InvalidOperationException("No hay conexión principal configurada.");

                    return cs;
                }
            }
        }

        public static bool CanConnectToDefault(string sqlitePath, out string error)
        {
            error = null;

            try
            {
                var cs = GetDefaultConnectionString(sqlitePath);

                // Reducimos el tiempo de espera a 3 segundos (en lugar de los 15-30s por defecto)
                // Esto evita que la aplicación parezca congelada si Hamachi o el Servidor están apagados.
                var builder = new SqlConnectionStringBuilder(cs);
                builder.ConnectTimeout = 3;
                cs = builder.ConnectionString;

                using (var cn = new SqlConnection(cs))
                {
                    cn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}