using DAO;
using Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

public class ZonasRepository
{
    GenericRepository connection;

    public long ConnectionId => connection.CurrentConnectionId;
    public string Plaza => connection.CurrentPlaza;

    public ZonasRepository()
    {
        connection = new GenericRepository();
    }

    public ZonasRepository(long connectionId)
    {
        connection = new GenericRepository(connectionId);
    }

    public ZonasRepository(string explicitConnectionString, string plaza = null, long connectionId = 0)
    {
        connection = new GenericRepository(explicitConnectionString, plaza, connectionId);
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
        var zona = connection.QuerySingle<Zona>(query, new { Id = id });
        if (zona != null)
        {
            zona.ConnectionId = connection.CurrentConnectionId;
            zona.Plaza = connection.CurrentPlaza;
        }
        return zona;
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
    public List<Zona> GetAllZonas(int? usuarioId = null, bool isAdmin = false)
    {
        var query = @"
SELECT z.*
FROM ZONAS z
WHERE
    @IsAdmin = 1
    OR EXISTS (
        SELECT 1
        FROM dbo.fn_ZonasPermitidasPorUsuario(@UsuarioId) f
        WHERE f.ZonaId = z.Id
    )
ORDER BY z.Nombre;
";

        var list = connection.Query<Zona>(query, new { UsuarioId = usuarioId ?? 0, IsAdmin = isAdmin ? 1 : 0 }).ToList();
        foreach (var item in list)
        {
            item.ConnectionId = connection.CurrentConnectionId;
            item.Plaza = connection.CurrentPlaza;
        }
        return list;
    }




}
