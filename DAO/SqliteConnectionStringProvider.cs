using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DAO
{
    public class SqliteConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string _sqlitePath;

        public SqliteConnectionStringProvider(string sqlitePath)
        {
            _sqlitePath = sqlitePath;
        }

        public string GetConnectionString()
        {
            var cn = new SqliteConnection($"Data Source={_sqlitePath}");
            cn.Open();

            var cmd = cn.CreateCommand();
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