using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class LoginLogica
    {
        UsuariosRepository contexto;
        public LoginLogica() { 
            
            contexto = new UsuariosRepository();
        
        }

        public async Task<List<string>> ValidarConexionesAsync()
        {
            return await DAO.GenericRepository.CheckAndDisableOfflineConnectionsAsync();
        }

        public static List<long> ObtenerConexionesInactivas()
        {
            lock (DAO.GenericRepository.InactiveConnectionIds)
            {
                return DAO.GenericRepository.InactiveConnectionIds.ToList();
            }
        }

        public static async Task<bool> PingConexionAsync(long id)
        {
            return await DAO.GenericRepository.PingConnectionAsync(id);
        }

        public static string ObtenerNombreConexion(long id)
        {
            var info = DAO.GenericRepository.GetConnectionById(id);
            return info?.Label;
        }

        public static void RemoverConexionInactiva(long id)
        {
            lock (DAO.GenericRepository.InactiveConnectionIds)
            {
                DAO.GenericRepository.InactiveConnectionIds.Remove(id);
            }
        }

        public UsuarioL ValidarAcceso(string usuario, string pass)
        {
            // Probar primero en la conexión principal
            var user = contexto.ValidarUsuario(usuario, pass);
            if (user != null) return user;

            // Si no se encuentra en la principal, buscar en las demás conexiones activas
            var connections = DAO.GenericRepository.GetAvailableConnections();
            if (connections != null && connections.Count > 1)
            {
                foreach (var conn in connections)
                {
                    if (conn.IsDefault) continue;
                    try
                    {
                        using (var repo = new UsuariosRepository(conn.ConnString, conn.Label, conn.Id))
                        {
                            user = repo.ValidarUsuario(usuario, pass);
                            if (user != null) return user;
                        }
                    }
                    catch { /* ignore */ }
                }
            }

            return null;
        }
    }
}
