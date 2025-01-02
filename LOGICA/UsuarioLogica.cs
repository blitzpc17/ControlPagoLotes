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
        public List<UsuarioL> GetAllUsuario()
        {
            return contexto.GetAllUsuarios();
        }
    }
}
