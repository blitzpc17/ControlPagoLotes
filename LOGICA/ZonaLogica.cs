using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class ZonaLogica
    {
        ZonasRepository contexto;
        private readonly long? _connectionId;

        public long? ConnectionId => _connectionId ?? (contexto != null ? contexto.ConnectionId : (long?)null);
        public string Plaza => contexto != null ? contexto.Plaza : null;

        public ZonaLogica() {
            contexto = new ZonasRepository();
        }

        public ZonaLogica(long connectionId) {
            _connectionId = connectionId;
            contexto = new ZonasRepository(connectionId);
        }

        public ZonaLogica(string explicitConnectionString, string plaza = null, long connectionId = 0) {
            _connectionId = connectionId;
            contexto = new ZonasRepository(explicitConnectionString, plaza, connectionId);
        }

        // Crear Zona
        public int AddZona(Zona zona)
        {
            if (zona.ConnectionId > 0 && zona.ConnectionId != ConnectionId)
            {
                var repo = new ZonasRepository(zona.ConnectionId);
                return repo.AddZona(zona);
            }
            return contexto.AddZona(zona);
        }

        // Leer Zona
        public Zona GetZonaById(int id, long? connectionId = null)
        {
            if (connectionId.HasValue && connectionId.Value > 0)
            {
                var repo = new ZonasRepository(connectionId.Value);
                return repo.GetZonaById(id);
            }
            return contexto.GetZonaById(id);
        }

        // Actualizar Zona
        public bool UpdateZona(Zona zona)
        {
            if (zona.ConnectionId > 0 && zona.ConnectionId != ConnectionId)
            {
                var repo = new ZonasRepository(zona.ConnectionId);
                return repo.UpdateZona(zona);
            }
            return contexto.UpdateZona(zona);
        }

        // Eliminar Zona
        public bool DeleteZona(int id, long? connectionId = null)
        {
            if (connectionId.HasValue && connectionId.Value > 0)
            {
                var repo = new ZonasRepository(connectionId.Value);
                return repo.DeleteZona(id);
            }
            return contexto.DeleteZona(id);
        }

        // Leer Zona (de forma federada o por conexión específica)
        public List<Zona> GetAllZonas(int? usuarioId = null, string nombreUsuario = null, bool unificarTodas = true)
        {
            bool isAdmin = !string.IsNullOrWhiteSpace(nombreUsuario) &&
                           string.Equals(nombreUsuario.Trim(), "ADMIN", StringComparison.OrdinalIgnoreCase);

            // Si no mandan usuarioId ni nombreUsuario (ej. configuración de rutas donde el admin quiere ver todas las zonas)
            bool verTodas = (!usuarioId.HasValue || usuarioId.Value <= 0) && string.IsNullOrWhiteSpace(nombreUsuario);

            if (_connectionId.HasValue || !unificarTodas)
            {
                int? localUserId = usuarioId;
                if (!isAdmin && !verTodas && !string.IsNullOrWhiteSpace(nombreUsuario))
                {
                    // Resolve the local user ID for this specific connection
                    try
                    {
                        var connIdToUse = _connectionId ?? DAO.GenericRepository.GetDefaultConnectionInfo()?.Id ?? 0;
                        if (connIdToUse > 0)
                        {
                            var repoInfo = DAO.GenericRepository.GetConnectionById(connIdToUse);
                            if (repoInfo.HasValue && !repoInfo.Value.IsDefault)
                            {
                                using (var userRepo = new DAO.ADOS.UsuariosRepository(repoInfo.Value.ConnString, repoInfo.Value.Label, repoInfo.Value.Id))
                                {
                                    var user = userRepo.GetUsuarioByNombre(nombreUsuario);
                                    if (user != null) localUserId = user.Id;
                                    else localUserId = 0; // User doesn't exist here
                                }
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine("Error resolving local user ID: " + ex.Message);
                    }
                }
                return contexto.GetAllZonas(localUserId, isAdmin || verTodas);
            }

            var connections = DAO.GenericRepository.GetAvailableConnections();
            if (connections == null || connections.Count <= 1)
            {
                return contexto.GetAllZonas(usuarioId, isAdmin || verTodas);
            }

            var result = new List<Zona>();

            foreach (var conn in connections)
            {
                try
                {
                    int? localUserId = null;
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

                    // Si no es admin y no es "verTodas", y el usuario no existe en esta plaza, omitir
                    if (!isAdmin && !verTodas && (!localUserId.HasValue || localUserId.Value <= 0))
                    {
                        continue;
                    }

                    var repo = new ZonasRepository(conn.ConnString, conn.Label, conn.Id);
                    var list = repo.GetAllZonas(localUserId, isAdmin || verTodas);
                    if (list != null && list.Count > 0)
                    {
                        result.AddRange(list);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al consultar zonas en plaza '{conn.Label}': {ex.Message}");
                }
            }

            return result.OrderBy(x => x.Nombre).ToList();
        }
    }
}
