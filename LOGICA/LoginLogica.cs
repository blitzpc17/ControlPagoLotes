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

        public UsuarioL ValidarAcceso(string usuario, string pass)
        {
            return contexto.ValidarUsuario(usuario, pass);
        }
    }
}
