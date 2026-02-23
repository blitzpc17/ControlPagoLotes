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

        public PagosRepository()
        {
            connection = new GenericRepository();
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
            return connection.QuerySingle<Pago>(query, new { Id = id });
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
            return connection.Query<Pago>(query).ToList();
        }

        public List<clsPagosBusqueda> GetAllPagosBusqueda(int usuarioId)
        {
            var query = @"
                            DECLARE @HasFilter BIT =
                                CASE WHEN EXISTS (SELECT 1 FROM dbo.fn_ZonasPermitidasPorUsuario(@UsuarioId)) THEN 1 ELSE 0 END;

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
                                (@HasFilter = 0 OR EXISTS (
                                    SELECT 1
                                    FROM dbo.fn_ZonasPermitidasPorUsuario(@UsuarioId) f
                                    WHERE f.ZonaId = p.ZonaId
                                ));
                            ";

            return connection.Query<clsPagosBusqueda>(query, new { UsuarioId = usuarioId }).ToList();
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
