using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.ADOS
{
    public class PagosRepository
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
            var query = @"INSERT INTO PAGOS (NombreCliente, Total, Meses, ZonaId, DiaPago, Lotes, FechaRegistro)
                      VALUES (@NombreCliente, @Total, @Meses, @ZonaId, @DiaPago, @Lotes, @FechaRegistro);
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
                      ZonaId = @ZonaId, DiaPago = @DiaPago, Lotes = @Lotes WHERE Id = @Id";
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
    }
}
