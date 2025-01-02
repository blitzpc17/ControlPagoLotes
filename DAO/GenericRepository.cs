using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

public class GenericRepository
{
    private readonly string _connectionString;

    public GenericRepository(/*string connectionString*/)
    {
        _connectionString = @"Data Source=USER-PC\SQLEXPRESS;Initial Catalog=DBPAGOSLOTES;Integrated Security=True;Encrypt=False";
    }

    private IDbConnection Connection => new SqlConnection(_connectionString);

    // Método para ejecutar un INSERT, UPDATE o DELETE
    public int Execute(string sql, object parameters = null)
    {
        using (var db = Connection)
        {
            return db.Execute(sql, parameters);
        }
    }

    // Método genérico para realizar un SELECT que devuelve una lista de entidades
    public IEnumerable<T> Query<T>(string sql, object parameters = null)
    {
        using (var db = Connection)
        {
            return db.Query<T>(sql, parameters).ToList();
        }
    }

    // Método para realizar un SELECT que devuelve una sola entidad (o null si no existe)
    public T QuerySingle<T>(string sql, object parameters = null)
    {
        using (var db = Connection)
        {
            return db.QueryFirstOrDefault<T>(sql, parameters);
        }
    }

    // Método para ejecutar un INSERT, UPDATE o DELETE Y REGRESAR ID DEL INSERTADO
    public int ExecuteScalar(string sql, object parameters = null)
    {
        using (var db = Connection)
        {
            return db.ExecuteScalar<int>(sql, parameters);
        }
    }
}
