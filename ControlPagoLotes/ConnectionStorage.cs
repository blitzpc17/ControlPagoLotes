using System;
using System.Data.SQLite;

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
            catch(Exception ex)
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
    }
}