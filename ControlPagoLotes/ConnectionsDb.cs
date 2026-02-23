using System.Data.SQLite;
using System.IO;

namespace ControlPagoLotes
{
    public static class ConnectionsDb
    {
        public static void EnsureCreated(string sqlitePath)
        {
            var dir = Path.GetDirectoryName(sqlitePath);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(sqlitePath))
                SQLiteConnection.CreateFile(sqlitePath);

            using (var cn = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
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
    }
}
