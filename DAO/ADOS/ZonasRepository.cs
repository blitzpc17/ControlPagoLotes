using DAO;
using Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

public class ZonasRepository
{
    GenericRepository connection;

    public ZonasRepository()
    {
        connection = new GenericRepository();
    }

    // Crear Zona
    public int AddZona(Zona zona)
    {
        var query = @"INSERT INTO ZONAS (Nombre)
                      VALUES (@Nombre);
                      SELECT CAST(SCOPE_IDENTITY() as int);";
       
        return connection.Execute(query, zona);
    }    

    // Leer Zona
    public Zona GetZonaById(int id)
    {
        var query = "SELECT * FROM ZONAS WHERE Id = @Id";
        return connection.QuerySingle<Zona>(query, new { Id = id });
    }

    // Actualizar Zona
    public bool UpdateZona(Zona zona)
    {
        var query = "UPDATE ZONAS SET Nombre = @Nombre WHERE Id = @Id";
        return connection.Execute(query, zona) > 0;
    }

    // Eliminar Zona
    public bool DeleteZona(int id)
    {
        var query = "DELETE FROM ZONAS WHERE Id = @Id";
        return connection.Execute(query, new { Id = id }) > 0;
    }

    // Leer Zona
    public List<Zona> GetAllZonas()        
    {
        var query = "SELECT * FROM ZONAS";
        return connection.Query<Zona>(query).ToList();
    }




}
