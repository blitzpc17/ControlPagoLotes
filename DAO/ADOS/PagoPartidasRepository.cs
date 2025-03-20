using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.ADOS
{
    public class PagoPartidasRepository:IDisposable
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
            var query = "SELECT * FROM PAGOSPARTIDAS " +
                "WHERE PAGOId = @Id AND FechaBaja is null AND UsuarioBajaId is null";
            return connection.Query<PagoPartida>(query, new { Id = idPago }).ToList();
        }

        public int EliminarPartidasAnteriores(int id)
        {
            var query = "DELETE FROM PAGOSPARTIDAS WHERE PAGOId = @Id";
            return connection.Execute(query, new { Id = id });
        }

        public int InsertarPartidasPago(string complemento)
        {
            return connection.Execute(complemento);
        }

        public List<clsDATACORTE> ListarPagoPorFecha(DateTime fecha)
        {
            var query = "SELECT \r\n" +
                        "    pa.NombreCliente,\r\n" +
                        "    z.Nombre as Zona,\r\n" +
                        "    pa.Lotes,\r\n" +
                        "    pp.Monto,\r\n" +
                        "    (CASE WHEN (pp.MontoOriginal IS NULL) THEN 0 ELSE pp.MontoOriginal END) AS CantidadOriginal, \r\n" +
                        "    pp.Fecha AS FechaPago,\r\n" +
                        "    pp.FechaCreacion AS FechaMovimiento, \r\n" +
                        "    u.Usuario AS UsuarioRecibe, \r\n" +
                        "    pp.FechaModificacion AS FechaModifico, \r\n" +
                        "    um.Usuario AS UsuarioModifico, \r\n" +
                        "    pp.FechaBaja AS FechaElimino, \r\n" +
                        "    ue.Usuario AS UsuarioElimino, \r\n" + 
                        "    pp.FormaPago AS FormaPagoTipo, \r\n" +
                        "    CASE WHEN pp.FormaPago = 0 THEN 'MIGRADO' ELSE (CASE WHEN pp.FormaPago = 1 THEN 'EFECTIVO' ELSE 'TRANSFERENCIA' END) END AS FormaPago \r\n" +
                        "FROM pagos pa\r\n" +
                        "JOIN pagospartidas pp ON pa.Id = pp.PagoId \r\n" +
                        "JOIN ZONAS z ON pa.ZonaId = z.Id \r\n" +
                        "JOIN USUARIOS u ON pp.UsuarioId = u.Id \r\n" +
                        "LEFT JOIN USUARIOS um ON pp.UsuarioModificoId = um.Id \r\n" +
                        "LEFT JOIN USUARIOS ue ON pp.UsuarioBajaId = ue.Id \r\n" +
                        "WHERE CAST(pp.FechaCreacion AS DATE) = @Fecha \r\n" + 
                        "   OR CAST(pp.FechaModificacion AS DATE) = @Fecha \r\n" +
                        "   OR CAST(pp.FechaBaja AS DATE) = @Fecha;";


            return connection.Query<clsDATACORTE>(query, new { Fecha =  fecha.ToString("yyyy-MM-dd")}).ToList();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
