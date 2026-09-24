using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class PagoPartidaLogica:IDisposable
    {
        PagoPartidasRepository contexto;
        ZonasRepository contextoZonas;
        private readonly long? _connectionId;

        public long? ConnectionId => _connectionId ?? (contexto != null ? contexto.ConnectionId : (long?)null);
        public string Plaza => contexto != null ? contexto.Plaza : null;

        public List<Zona> LstZona;
        public PeriodoConsulta objConsulta;

        public PagoPartidaLogica()
        {
            contexto = new PagoPartidasRepository();
            contextoZonas = new ZonasRepository();
        }

        public PagoPartidaLogica(long connectionId)
        {
            _connectionId = connectionId;
            contexto = new PagoPartidasRepository(connectionId);
            contextoZonas = new ZonasRepository(connectionId);
        }

        public PagoPartidaLogica(string explicitConnectionString, string plaza = null, long connectionId = 0)
        {
            _connectionId = connectionId;
            contexto = new PagoPartidasRepository(explicitConnectionString, plaza, connectionId);
            contextoZonas = new ZonasRepository(explicitConnectionString, plaza, connectionId);
        }

        public void InicializarObjConsulta()
        {
            objConsulta = new PeriodoConsulta();
        }

        // Crear PagoPartida
        public int AddPagoPartida(PagoPartida PagoPartida)
        {
            return contexto.AddPagoPartida(PagoPartida);
        }

        // Leer PagoPartida
        public PagoPartida GetPagoPartidaById(int id)
        {
            return contexto.GetPagoPartidaById(id);
        }

        // Actualizar PagoPartida
        public bool UpdatePagoPartida(PagoPartida PagoPartida)
        {
            return contexto.UpdatePagoPartida(PagoPartida);
        }

        // Eliminar PagoPartida
        public bool DeletePagoPartida(int id)
        {
            return contexto.DeletePagoPartida(id);
        }

        // Leer PagoPartida
        public List<PagoPartida> GetAllPagoPartidas(int idPago)
        {
            return contexto.GetAllPAGOSPARTIDAS(idPago);
        }

        public int EliminarPartidasAnteriores(int id)
        {
            return contexto.EliminarPartidasAnteriores(id);
        }

        public int InsertarPartidasPago(string query)
        {
            return contexto.InsertarPartidasPago(query);
        }
        public List<clsDATACORTE> ListarPagoPorFecha(PeriodoConsulta obj, int usuarioId, string nombreUsuario = null)
        {
            if (_connectionId.HasValue)
            {
                return contexto.ListarPagoPorFecha(obj, usuarioId);
            }

            var allData = new List<clsDATACORTE>();
            var connections = DAO.GenericRepository.GetAvailableConnections();

            if (connections == null || connections.Count == 0)
            {
                return contexto.ListarPagoPorFecha(obj, usuarioId);
            }

            foreach (var conn in connections)
            {
                try
                {
                    int localUserId = 0;
                    if (!string.IsNullOrWhiteSpace(nombreUsuario))
                    {
                        using (var userRepo = new UsuariosRepository(conn.ConnString, conn.Label, conn.Id))
                        {
                            var user = userRepo.GetUsuarioByNombre(nombreUsuario);
                            if (user != null) localUserId = user.Id;
                        }
                    }
                    else if (conn.IsDefault)
                    {
                        localUserId = usuarioId;
                    }

                    if (localUserId > 0 || string.Equals(nombreUsuario, "ADMIN", StringComparison.OrdinalIgnoreCase))
                    {
                        var repo = new PagoPartidasRepository(conn.ConnString, conn.Label, conn.Id);
                        var results = repo.ListarPagoPorFecha(obj, localUserId);
                        allData.AddRange(results);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error querying PagoPartidas en plaza {conn.Label}: {ex.Message}");
                }
            }

            return allData.OrderByDescending(x => x.FechaPago).ToList();
        }

        public void Dispose()
        {
            contexto?.Dispose();
        }

        public async Task<List<string>> CheckAndDisableOfflineConnectionsAsync()
        {
            return await DAO.GenericRepository.CheckAndDisableOfflineConnectionsAsync();
        }

        public List<PagoPartida> ListarPartidasPagos(string idsRelacionados)
        {
            return contexto.ListarPartidasPagos(idsRelacionados);
        }

        public void ListarLotificaciones()
        {
            if (_connectionId.HasValue)
            {
                LstZona = contextoZonas.GetAllZonas();
            }
            else
            {
                var zonaLogic = new ZonaLogica();
                LstZona = zonaLogic.GetAllZonas(unificarTodas: true);
            }
        }
    }
}
