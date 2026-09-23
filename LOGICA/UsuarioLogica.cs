using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class UsuarioLogica
    {
        UsuariosRepository contexto;
        public UsuarioLogica()
        {

            contexto = new UsuariosRepository();
        }

        // Crear Usuario
        public int AddUsuario(UsuarioL Usuario)
        {
            return contexto.AddUsuarioL(Usuario);
        }

        // Leer Usuario
        public UsuarioL GetUsuarioById(int id)
        {
            return contexto.GetUsuarioLById(id);
        }

        // Actualizar Usuario
        public bool UpdateUsuario(UsuarioL Usuario)
        {
            return contexto.UpdateUsuarioL(Usuario);
        }

        // Eliminar Usuario
        public bool DeleteUsuario(int id)
        {
            return contexto.DeleteUsuarioL(id);
        }

        // Leer Usuario
        public List<UsuarioL> GetAllUsuario(bool unificarTodas = false)
        {
            if (!unificarTodas) return contexto.GetAllUsuarios();

            var connections = DAO.GenericRepository.GetAvailableConnections();
            if (connections == null || connections.Count <= 1) return contexto.GetAllUsuarios();

            var list = new List<UsuarioL>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var conn in connections)
            {
                try
                {
                    using (var repo = new UsuariosRepository(conn.ConnString, conn.Label, conn.Id))
                    {
                        var users = repo.GetAllUsuarios();
                        if (users != null)
                        {
                            foreach (var u in users)
                            {
                                if (!string.IsNullOrWhiteSpace(u.Usuario) && seen.Add(u.Usuario))
                                {
                                    list.Add(u);
                                }
                            }
                        }
                    }
                }
                catch { /* ignore */ }
            }

            return list.OrderBy(u => u.Usuario).ToList();
        }
    }
}
