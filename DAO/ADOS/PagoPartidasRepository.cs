using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.ADOS
{
    public class PagoPartidasRepository
    {
        GenericRepository connection;

        public PagoPartidasRepository()
        {
            connection = new GenericRepository();
        }

        // Crear PagoPartida
        public int AddPagoPartida(PagoPartida PagoPartida)
        {
            var query = @"INSERT INTO PAGOSPARTIDAS (PagoId, Monto, Fecha, UsuarioId, FechaCreacion)
                      VALUES (@PagoId, @Monto, @Fecha, @UsuarioId, @FechaCreacion);
                      SELECT CAST(SCOPE_IDENTITY() as int);";

            return connection.Execute(query, PagoPartida);
        }

        // Leer PagoPartida
        public PagoPartida GetPagoPartidaById(int id)
        {
            var query = "SELECT * FROM PAGOSPARTIDAS WHERE Id = @Id";
            return connection.QuerySingle<PagoPartida>(query, new { Id = id });
        }

        // Actualizar PagoPartida
        public bool UpdatePagoPartida(PagoPartida PagoPartida)
        {
            var query = @"UPDATE PAGOSPARTIDAS SET PagoId = @PagoId, Monto = @Monto, Fecha = @Fecha, FechaCreacion = @FechaCreacion, 
                      UsuarioId = @UsuarioId WHERE Id = @Id";
            return connection.Execute(query, PagoPartida) > 0;
        }

        // Eliminar PagoPartida
        public bool DeletePagoPartida(int id)
        {
            var query = "DELETE FROM PAGOSPARTIDAS WHERE Id = @Id";
            return connection.Execute(query, new { Id = id }) > 0;
        }

        // Leer PagoPartida
        public List<PagoPartida> GetAllPAGOSPARTIDAS(int idPago)
        {
            var query = "SELECT * FROM PAGOSPARTIDAS WHERE PAGOId = @Id";
            return connection.Query<PagoPartida>(query, new { Id = idPago }).ToList();
        }

        public int EliminarPartidasAnteriores(int id)
        {
            var query = "DELETE FROM PAGOSPARTIDAS WHERE PAGOId = @Id";
            return connection.Execute(query, new { Id = id });
        }

        public int InsertarPartidasPago(string complemento)
        {
            var query = "INSERT INTO PAGOSPARTIDAS VALUES " + complemento;
            return connection.Execute(query);
        }

        public List<clsDATACORTE> ListarPagoPorFecha(DateTime fecha)
        {            
            var query = "SELECT \r\n" +
                "    pa.NombreCliente,\r\n" +
                "    z.Nombre as Zona,\r\n" +
                "    pa.Lotes,\r\n" +
                "    pp.Monto,\r\n" +
                "    pp.Fecha as FechaPago,\r\n" +
                "    pp.FechaCreacion as FechaMovimiento, \r\n" +
                "    u.Usuario as UsuarioRecibe \r\n" +
                "FROM pagos pa\r\n" +
                "JOIN pagospartidas pp ON pa.Id = pp.PagoId \r\n" +
                "JOIN ZONAS z ON pa.ZonaId = z.Id \r\n" +
                "JOIN USUARIOS u ON pp.UsuarioId = u.Id \r\n" +
                "WHERE CAST(pp.FechaCreacion AS DATE) = @Fecha;";

            return connection.Query<clsDATACORTE>(query, new { Fecha =  fecha.ToString("yyyy-MM-dd")}).ToList();
        }


    }
}
