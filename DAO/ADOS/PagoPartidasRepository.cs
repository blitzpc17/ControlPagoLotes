using Dapper;
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

        public List<clsDATACORTE> ListarPagoPorFecha(PeriodoConsulta obj)
        {
            string condiciones = "";

            switch (obj.Tipo.ToUpper())
            {
                case "DIA":
                    string fecha = Convert.ToDateTime(obj.Fecha).ToString("yyyy-MM-dd");
                    condiciones += " (CAST(pp.FechaCreacion AS DATE) = '" + fecha + "'" +
                                 " OR CAST(pp.FechaModificacion AS DATE) = '" + fecha + "'" +
                                 " OR CAST(pp.FechaBaja AS DATE) = '" + fecha + "')";
                    break;

                case "SEMANA":
                    if (obj.NumeroSemana.HasValue && obj.Anio.HasValue)
                    {
                        // Usar funciones de SQL para manejar la semana
                        condiciones += $@"( (DATEPART(YEAR, pp.FechaCreacion) = {obj.Anio.Value} 
                                AND DATEPART(WEEK, pp.FechaCreacion) = {obj.NumeroSemana.Value}
                                OR DATEPART(YEAR, pp.FechaModificacion) = {obj.Anio.Value} 
                                AND DATEPART(WEEK, pp.FechaModificacion) = {obj.NumeroSemana.Value}
                                OR DATEPART(YEAR, pp.FechaBaja) = {obj.Anio.Value} 
                                AND DATEPART(WEEK, pp.FechaBaja) = {obj.NumeroSemana.Value})) ";
                    }
                    break;

                case "MES":
                    if (obj.Mes.HasValue && obj.Anio.HasValue)
                    {
                        condiciones += $@" ((DATEPART(YEAR, pp.FechaCreacion) = {obj.Anio.Value} 
                                AND DATEPART(MONTH, pp.FechaCreacion) = {obj.Mes.Value}
                                OR DATEPART(YEAR, pp.FechaModificacion) = {obj.Anio.Value} 
                                AND DATEPART(MONTH, pp.FechaModificacion) = {obj.Mes.Value}
                                OR DATEPART(YEAR, pp.FechaBaja) = {obj.Anio.Value} 
                                AND DATEPART(MONTH, pp.FechaBaja) = {obj.Mes.Value}))";
                    }
                    break;
            }

           
            // Lotificaciones
            if (!obj.todas)
            {
                condiciones += " AND z.Id = " + obj.LotificacionId.Value;
            }

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
                        "WHERE " + condiciones;

            return connection.Query<clsDATACORTE>(query).ToList();
        }

        public List<clsDATACORTE> ListarPagoPorPeriodo(PeriodoConsulta periodo, int? idZona)
        {
            var query = new StringBuilder();
            query.AppendLine("SELECT ..."); // mismo SELECT anterior

            query.AppendLine("WHERE 1 = 1");

            var parameters = new DynamicParameters();

            switch (periodo.Tipo.ToUpper())
            {
                case "DIA":
                    query.AppendLine("AND (CAST(pp.FechaCreacion AS DATE) = @Fecha");
                    query.AppendLine("   OR CAST(pp.FechaModificacion AS DATE) = @Fecha");
                    query.AppendLine("   OR CAST(pp.FechaBaja AS DATE) = @Fecha)");
                    parameters.Add("Fecha", periodo.Fecha.Value.ToString("yyyy-MM-dd"));
                    break;

                case "SEMANA":
                    query.AppendLine("AND (DATEPART(WEEK, pp.FechaCreacion) = @NumeroSemana AND DATEPART(YEAR, pp.FechaCreacion) = @Anio");
                    query.AppendLine("   OR DATEPART(WEEK, pp.FechaModificacion) = @NumeroSemana AND DATEPART(YEAR, pp.FechaModificacion) = @Anio");
                    query.AppendLine("   OR DATEPART(WEEK, pp.FechaBaja) = @NumeroSemana AND DATEPART(YEAR, pp.FechaBaja) = @Anio)");
                    parameters.Add("NumeroSemana", periodo.NumeroSemana.Value);
                    parameters.Add("Anio", periodo.Anio.Value);
                    break;

                case "MES":
                    query.AppendLine("AND (MONTH(pp.FechaCreacion) = @NumeroMes AND YEAR(pp.FechaCreacion) = @AnioMes");
                    query.AppendLine("   OR MONTH(pp.FechaModificacion) = @NumeroMes AND YEAR(pp.FechaModificacion) = @AnioMes");
                    query.AppendLine("   OR MONTH(pp.FechaBaja) = @NumeroMes AND YEAR(pp.FechaBaja) = @AnioMes)");
                    parameters.Add("NumeroMes", periodo.Mes);
                    parameters.Add("AnioMes", periodo.Anio);
                    break;
            }

            return connection.Query<clsDATACORTE>(query.ToString(), parameters).ToList();
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public List<PagoPartida> ListarPartidasPagos(string idsRelacionados)
        {
            var query = "SELECT * FROM PAGOSPARTIDAS " +
               "WHERE PAGOId in ("+idsRelacionados+") AND FechaBaja is null AND UsuarioBajaId is null";
            return connection.Query<PagoPartida>(query).ToList();
        }
    }
}
