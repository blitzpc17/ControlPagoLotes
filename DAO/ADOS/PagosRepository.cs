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

        public List<clsPagosBusqueda> GetAllPagosBusqueda()
        {
            var query = "SELECT \r\n    CAST(p.Id AS NVARCHAR(MAX)) AS Id,\r\n    " +
                "CAST(p.NombreCliente AS NVARCHAR(MAX)) AS Cliente,\r\n    " +
                "CAST(zn.Nombre AS NVARCHAR(MAX)) AS Zona,\r\n    " +
                "CAST(p.Lotes AS NVARCHAR(MAX)) AS Lotes,\r\n    " +
                "FORMAT(p.Total, '$0.00') AS Total,\r\n    " +
                "FORMAT(p.FechaRegistro, 'dd/MM/yyyy') AS Fecha,\r\n    " +
                "CAST(p.Estado AS NVARCHAR(MAX)) AS ClaveEstado,\r\n    CASE \r\n        " +
                "WHEN p.Estado = '1' THEN 'AL CORRIENTE'\r\n        " +
                "WHEN p.Estado = '2' THEN 'PAGADO'\r\n        " +
                "WHEN p.Estado = '3' THEN 'CANCELADO'\r\n        " +
                "WHEN p.Estado = '4' THEN 'ATRASADO'\r\n        " +
                "ELSE 'DESCONOCIDO'\r\n    " +
                "END AS NombreEstado\r\nFROM PAGOS p\r\nJOIN ZONAS zn ON p.ZonaId = zn.Id;";

            return connection.Query<clsPagosBusqueda>(query).ToList();
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
