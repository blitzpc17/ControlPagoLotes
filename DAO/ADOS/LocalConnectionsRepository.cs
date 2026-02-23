using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DAO.ADOS
{


    public class LocalConnectionsRepository
    {
        private readonly string _dbPath;
        public LocalConnectionsRepository(string dbPath) => _dbPath = dbPath;

        private SqliteConnection Open()
        {
            var cn = new SqliteConnection($"Data Source={_dbPath}");
            cn.Open();
            return cn;
        }

        public List<(long id, string label, string connString, bool isDefault)> List()
        {
            var cn = Open();
            var cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT id,label,conn_string,is_default FROM connections ORDER BY is_default DESC, label;";
            var rd = cmd.ExecuteReader();

            var list = new List<(long, string, string, bool)>();
            while (rd.Read())
                list.Add((rd.GetInt64(0), rd.GetString(1), rd.GetString(2), rd.GetInt32(3) == 1));
            return list;
        }

        public long Insert(string label, string connString, bool setDefault)
        {
            var cn = Open();
            var tx = cn.BeginTransaction();

            if (setDefault)
            {
                var clear = cn.CreateCommand();
                clear.Transaction = tx;
                clear.CommandText = "UPDATE connections SET is_default = 0 WHERE is_default = 1;";
                clear.ExecuteNonQuery();
            }

            var cmd = cn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
INSERT INTO connections(label, conn_string, is_default)
VALUES($label,$cs,$def);
SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$label", label);
            cmd.Parameters.AddWithValue("$cs", connString);
            cmd.Parameters.AddWithValue("$def", setDefault ? 1 : 0);

            var id = (long)cmd.ExecuteScalar();
            tx.Commit();
            return id;
        }

        public void Update(long id, string label, string connString)
        {
            var cn = Open();
            var cmd = cn.CreateCommand();
            cmd.CommandText = @"
UPDATE connections
SET label=$label, conn_string=$cs, updated_at=datetime('now')
WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$label", label);
            cmd.Parameters.AddWithValue("$cs", connString);
            cmd.ExecuteNonQuery();
        }

        public void Delete(long id)
        {
            var cn = Open();
            var cmd = cn.CreateCommand();
            cmd.CommandText = "DELETE FROM connections WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void SetDefault(long id)
        {
            var cn = Open();
            var tx = cn.BeginTransaction();

            var clear = cn.CreateCommand();
            clear.Transaction = tx;
            clear.CommandText = "UPDATE connections SET is_default = 0 WHERE is_default = 1;";
            clear.ExecuteNonQuery();

            var cmd = cn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "UPDATE connections SET is_default = 1, updated_at=datetime('now') WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();

            tx.Commit();
        }
    }

}
