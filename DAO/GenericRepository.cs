using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

public class GenericRepository : IDisposable
{
    private readonly string _connectionString;
    private bool _disposed = false;
    private IDbConnection _connection; // Campo para manejar la conexión

    public GenericRepository()
    {
        // ASIGNACIÓN CORRECTA al campo de clase
        _connectionString = @"Data Source=192.168.1.15;Initial Catalog=DBPAGOSLOTES;User Id=jaade;Password=ohT627R2;Encrypt=False;Connect Timeout=30";

        // NOTA: Estás usando "master" en tu código, probablemente quieres "DBPAGOSLOTES"
        // Si realmente quieres conectar a master:
        // _connectionString = @"Data Source=192.168.1.15;Initial Catalog=master;User Id=spartan;Password=ohT627R2;Encrypt=False;Connect Timeout=30";
    }

    // Constructor que acepta cadena de conexión como parámetro
    public GenericRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("La cadena de conexión no puede estar vacía");

        _connectionString = connectionString;
    }

    // Propiedad para obtener conexión (LAZY initialization)
    private IDbConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }
    }

    // Método para ejecutar un INSERT, UPDATE o DELETE
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

    // Método genérico para realizar un SELECT que devuelve una lista de entidades
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

    // Método para realizar un SELECT que devuelve una sola entidad
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

    // Método para ejecutar un INSERT, UPDATE o DELETE Y REGRESAR ID DEL INSERTADO
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

    // Método para probar la conexión
    public bool TestConnection()
    {
        try
        {
            using (var testConn = new SqlConnection(_connectionString))
            {
                testConn.Open();
                var version = testConn.ServerVersion; // Esto debería funcionar ahora
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

    // Método para obtener información del servidor
    public string GetServerInfo()
    {
        try
        {
            using (var conn = new SqlConnection(_connectionString))
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

    // Método para diagnóstico
    public void Diagnose()
    {
        Console.WriteLine("=== DIAGNÓSTICO ===");
        Console.WriteLine($"Connection String: {_connectionString}");
        Console.WriteLine($"Database in string: {GetDatabaseFromConnectionString()}");

        try
        {
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

    private string GetDatabaseFromConnectionString()
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
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

    // Implementación de IDisposable mejorada
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
                // Liberar recursos administrados
                if (_connection != null)
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                    _connection.Dispose();
                    _connection = null;
                }
            }
            _disposed = true;
        }
    }

    // Destructor (finalizador) por si se olvida llamar a Dispose
    ~GenericRepository()
    {
        Dispose(false);
    }
}