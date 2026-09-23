using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class PagoLogica:IDisposable
    {
        PagosRepository contexto;
        private readonly long? _connectionId;

        public long? ConnectionId => _connectionId ?? (contexto != null ? contexto.ConnectionId : (long?)null);
        public string Plaza => contexto != null ? contexto.Plaza : null;

        public PagoLogica()
        {
            contexto = new PagosRepository();
        }

        public PagoLogica(long connectionId)
        {
            _connectionId = connectionId;
            contexto = new PagosRepository(connectionId);
        }

        public PagoLogica(string explicitConnectionString, string plaza = null, long connectionId = 0)
        {
            _connectionId = connectionId;
            contexto = new PagosRepository(explicitConnectionString, plaza, connectionId);
        }

        // Crear Pago
        public int AddPago(Pago Pago)
        {
            if (Pago.ConnectionId > 0 && Pago.ConnectionId != ConnectionId)
            {
                using (var targetRepo = new PagosRepository(Pago.ConnectionId))
                {
                    return targetRepo.AddPagos(Pago);
                }
            }
            return contexto.AddPagos(Pago);
        }

        // Leer Pago
        public Pago GetPagoById(int id, long? connectionId = null)
        {
            if (connectionId.HasValue && connectionId.Value > 0)
            {
                using (var targetRepo = new PagosRepository(connectionId.Value))
                {
                    return targetRepo.GetPagosById(id);
                }
            }
            return contexto.GetPagosById(id);
        }

        // Actualizar Pago
        public bool UpdatePago(Pago Pago)
        {
            if (Pago.ConnectionId > 0 && Pago.ConnectionId != ConnectionId)
            {
                using (var targetRepo = new PagosRepository(Pago.ConnectionId))
                {
                    return targetRepo.UpdatePagos(Pago);
                }
            }
            return contexto.UpdatePagos(Pago);
        }

        // Eliminar Pago
        public bool DeletePago(int id, long? connectionId = null)
        {
            if (connectionId.HasValue && connectionId.Value > 0)
            {
                using (var targetRepo = new PagosRepository(connectionId.Value))
                {
                    return targetRepo.DeletePagos(id);
                }
            }
            return contexto.DeletePagos(id);
        }

        // Leer Pago
        public List<Pago> GetAllPagos()
        {
            return contexto.GetAllPagos();
        }

        // Leer pagos búsqueda de forma federada (todas las conexiones disponibles)
        public List<clsPagosBusqueda> GetAllPagosBusqueda(int idUsuario, string nombreUsuario = null)
        {
            var connections = DAO.GenericRepository.GetAvailableConnections();
            bool isAdmin = !string.IsNullOrWhiteSpace(nombreUsuario) &&
                           string.Equals(nombreUsuario.Trim(), "ADMIN", StringComparison.OrdinalIgnoreCase);

            if (connections == null || connections.Count <= 1)
            {
                return contexto.GetAllPagosBusqueda(idUsuario, isAdmin);
            }

            var result = new List<clsPagosBusqueda>();

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
                        localUserId = idUsuario;
                    }

                    // Si no es admin y el usuario no existe en esta plaza, omitir esta plaza
                    if (!isAdmin && localUserId <= 0)
                    {
                        continue;
                    }

                    using (var repo = new PagosRepository(conn.ConnString, conn.Label, conn.Id))
                    {
                        var list = repo.GetAllPagosBusqueda(localUserId, isAdmin);
                        if (list != null && list.Count > 0)
                        {
                            result.AddRange(list);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al consultar pagos en plaza '{conn.Label}': {ex.Message}");
                }
            }

            return result.OrderBy(x => x.Cliente).ToList();
        }

        public void Dispose()
        {
            contexto?.Dispose();
        }

        public List<Pago> ListarPagosxZona(int zonaId, long? connectionId = null)
        {
            if (connectionId.HasValue && connectionId.Value > 0)
            {
                using (var targetRepo = new PagosRepository(connectionId.Value))
                {
                    return targetRepo.ListarPagosXZona(zonaId);
                }
            }
            return contexto.ListarPagosXZona(zonaId);
        }
    }
}
