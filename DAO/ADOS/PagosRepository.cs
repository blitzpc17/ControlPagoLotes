using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.ADOS
{
    public class PagosRepository:IDisposable
    {
        GenericRepository connection;

        public long ConnectionId => connection.CurrentConnectionId;
        public string Plaza => connection.CurrentPlaza;

        public PagosRepository()
        {
            connection = new GenericRepository();
        }

        public PagosRepository(long connectionId)
        {
            connection = new GenericRepository(connectionId);
        }

        public PagosRepository(string explicitConnectionString, string plaza = null, long connectionId = 0)
        {
            connection = new GenericRepository(explicitConnectionString, plaza, connectionId);
        }

        // Crear Pagos
        public int AddPagos(Pago Pagos)
        //, FechaPago, FechaRegistro
        //, @FechaPago, @FechaRegistro
        {
            var query = @"INSERT INTO PAGOS (NombreCliente, Total, Meses, ZonaId, DiaPago, Lotes, FechaRegistro, Estado, FechaCreacion, Telefonos, Observacion)
                      VALUES (@NombreCliente, @Total, @Meses, @ZonaId, @DiaPago, @Lotes, @FechaRegistro, @Estado, @FechaCreacion, @Telefonos, @Observacion);
                      SELECT CAST(SCOPE_IDENTITY() as int);";

            return connection.ExecuteScalar(query, Pagos);
        }

        // Leer Pagos
        public Pago GetPagosById(int id)
        {
            var query = "SELECT * FROM PAGOS WHERE Id = @Id";
            var pago = connection.QuerySingle<Pago>(query, new { Id = id });
            if (pago != null)
            {
                pago.ConnectionId = connection.CurrentConnectionId;
                pago.Plaza = connection.CurrentPlaza;
            }
            return pago;
        }

        // Actualizar Pagos
        public bool UpdatePagos(Pago Pagos)
        {
            var query = @"UPDATE PAGOS SET NombreCliente = @NombreCliente, Total = @Total, Meses = @Meses, 
                      ZonaId = @ZonaId, DiaPago = @DiaPago, Lotes = @Lotes, Estado = @Estado, 
                      FechaRegistro = @FechaRegistro, Telefonos = @Telefonos, Observacion = @Observacion 
                      WHERE Id = @Id";
            return connection.Execute(query, Pagos) > 0;
        }

        // Eliminar Pagos
        public bool DeletePagos(int id)
        {
            var query = "DELETE FROM PAGOS WHERE Id = @Id";
            return connection.Execute(query, new { Id = id }) > 0;
        }

        // Leer Pagos
        public List<Pago> GetAllPagos()
        {
            var query = "SELECT * FROM Pagos";
            var list = connection.Query<Pago>(query).ToList();
            foreach (var item in list)
            {
                item.ConnectionId = connection.CurrentConnectionId;
                item.Plaza = connection.CurrentPlaza;
            }
            return list;
        }

        public List<clsPagosBusqueda> GetAllPagosBusqueda(int usuarioId, bool isAdmin = false)
        {
            var query = @"
                            SELECT 
                                CAST(p.Id AS NVARCHAR(MAX)) AS Id,
                                CAST(p.NombreCliente AS NVARCHAR(MAX)) AS Cliente,
                                CAST(zn.Nombre AS NVARCHAR(MAX)) AS Zona,
                                CAST(p.Lotes AS NVARCHAR(MAX)) AS Lotes,
                                FORMAT(p.Total, '$0.00') AS Total,
                                FORMAT(p.FechaRegistro, 'dd/MM/yyyy') AS Fecha,
                                CAST(p.Estado AS NVARCHAR(MAX)) AS ClaveEstado,
                                CASE 
                                    WHEN p.Estado = '1' THEN 'AL CORRIENTE'
                                    WHEN p.Estado = '2' THEN 'PAGADO'
                                    WHEN p.Estado = '3' THEN 'CANCELADO'
                                    WHEN p.Estado = '4' THEN 'ATRASADO'
                                    ELSE 'DESCONOCIDO'
                                END AS NombreEstado
                            FROM PAGOS p
                            JOIN ZONAS zn ON p.ZonaId = zn.Id
                            WHERE
                                @IsAdmin = 1
                                OR EXISTS (
                                    SELECT 1
                                    FROM dbo.fn_ZonasPermitidasPorUsuario(@UsuarioId) f
                                    WHERE f.ZonaId = p.ZonaId
                                );
                            ";

            var list = connection.Query<clsPagosBusqueda>(query, new { UsuarioId = usuarioId, IsAdmin = isAdmin ? 1 : 0 }).ToList();
            foreach (var item in list)
            {
                item.ConnectionId = connection.CurrentConnectionId;
                item.Plaza = connection.CurrentPlaza;
            }
            return list;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public List<Pago> ListarPagosXZona(int zonaId)
        {
            var query = "SELECT * FROM Pagos WHERE ZonaId = "+zonaId;
            return connection.Query<Pago>(query).ToList();
        }
    }
}
