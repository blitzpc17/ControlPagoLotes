// Archivo: LOGICA/RutasLogica.cs
using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace LOGICA
{
    public class RutasLogica
    {
        private readonly VariablesGlobalesRepository varsRepo;
        private readonly JavaScriptSerializer json;

        private const string LABEL = "FltroZonas";

        public RutasLogica()
        {
            varsRepo = new VariablesGlobalesRepository();
            json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        }

        public RutasLogica(long connectionId)
        {
            varsRepo = new VariablesGlobalesRepository(connectionId);
            json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        }

        public RutasLogica(string explicitConnectionString, string plaza = null, long connectionId = 0)
        {
            varsRepo = new VariablesGlobalesRepository(explicitConnectionString, plaza, connectionId);
            json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        }

        public List<FiltroZonasEntry> GetAll()
        {
            var raw = varsRepo.GetValorByLabel(LABEL);

            if (string.IsNullOrWhiteSpace(raw))
                return new List<FiltroZonasEntry>();

            raw = raw.Trim();
            if (raw == "[]") return new List<FiltroZonasEntry>();

            try
            {
                var list = json.Deserialize<List<FiltroZonasEntry>>(raw);
                return list ?? new List<FiltroZonasEntry>();
            }
            catch
            {
                // Si está corrupto, no revienta la app
                return new List<FiltroZonasEntry>();
            }
        }

        public List<int> GetZonasForUser(int usuarioId)
        {
            var all = GetAll();
            return all.FirstOrDefault(x => x.UsuarioId == usuarioId)?.ZonasId ?? new List<int>();
        }

        public void SetZonasForUser(int usuarioId, List<int> zonasId)
        {
            zonasId = (zonasId ?? new List<int>())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var all = GetAll();

            var entry = all.FirstOrDefault(x => x.UsuarioId == usuarioId);
            if (entry == null)
            {
                all.Add(new FiltroZonasEntry
                {
                    UsuarioId = usuarioId,
                    ZonasId = zonasId
                });
            }
            else
            {
                entry.ZonasId = zonasId;
            }

            // Limpieza: si un usuario se queda sin zonas, puedes decidir quitarlo del JSON:
            all = all.Where(x => x.ZonasId != null && x.ZonasId.Count > 0).ToList();

            var raw = json.Serialize(all);
            varsRepo.UpsertValor(LABEL, raw);
        }

        public HashSet<string> GetZonasAsignadasPorUsuario(string nombreUsuario)
        {
            var assignedSet = new HashSet<string>();
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return assignedSet;

            var connections = DAO.GenericRepository.GetAvailableConnections();
            if (connections == null || connections.Count == 0) return assignedSet;

            foreach (var conn in connections)
            {
                try
                {
                    int localUserId = 0;
                    using (var uRepo = new UsuariosRepository(conn.ConnString, conn.Label, conn.Id))
                    {
                        var localUser = uRepo.GetUsuarioByNombre(nombreUsuario);
                        if (localUser != null) localUserId = localUser.Id;
                    }

                    if (localUserId > 0)
                    {
                        var rutLogic = new RutasLogica(conn.ConnString, conn.Label, conn.Id);
                        var userZonas = rutLogic.GetZonasForUser(localUserId);
                        if (userZonas != null)
                        {
                            foreach (var zid in userZonas)
                            {
                                assignedSet.Add($"{conn.Id}_{zid}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al consultar permisos de {nombreUsuario} en {conn.Label}: {ex.Message}");
                }
            }

            return assignedSet;
        }

        public List<string> SaveZonasAsignadasPorUsuario(string nombreUsuario, string password, Dictionary<long, List<int>> zonasPorConexion)
        {
            var savedPlazas = new List<string>();
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return savedPlazas;

            var connections = DAO.GenericRepository.GetAvailableConnections();
            if (connections == null || connections.Count == 0) return savedPlazas;

            foreach (var conn in connections)
            {
                try
                {
                    int localUserId = 0;
                    using (var uRepo = new UsuariosRepository(conn.ConnString, conn.Label, conn.Id))
                    {
                        var localUser = uRepo.GetUsuarioByNombre(nombreUsuario);
                        if (localUser != null)
                        {
                            localUserId = localUser.Id;
                        }
                        else
                        {
                            if (zonasPorConexion != null && zonasPorConexion.ContainsKey(conn.Id) && zonasPorConexion[conn.Id].Count > 0)
                            {
                                localUserId = uRepo.AddUsuarioL(new UsuarioL
                                {
                                    Usuario = nombreUsuario,
                                    Password = password ?? ""
                                });
                            }
                        }
                    }

                    if (localUserId > 0)
                    {
                        var rutLogic = new RutasLogica(conn.ConnString, conn.Label, conn.Id);
                        List<int> zonasParaEstaPlaza;
                        if (zonasPorConexion == null || !zonasPorConexion.TryGetValue(conn.Id, out zonasParaEstaPlaza))
                        {
                            zonasParaEstaPlaza = new List<int>();
                        }

                        rutLogic.SetZonasForUser(localUserId, zonasParaEstaPlaza);
                        savedPlazas.Add(conn.Label);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al guardar en plaza '{conn.Label}': {ex.Message}");
                }
            }

            return savedPlazas;
        }
    }
}
