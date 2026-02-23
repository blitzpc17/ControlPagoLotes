using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DAO
{

    public static class ConnectionsDb
    {
        public static void EnsureCreated(string sqlitePath)
        {
            var cn = new SqliteConnection($"Data Source={sqlitePath}");
            cn.Open();

            var cmd = cn.CreateCommand();
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
WHERE is_default = 1;
";
            cmd.ExecuteNonQuery();
        }
    }

}
