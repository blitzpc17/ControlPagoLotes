using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.ADOS
{
    public class UsuariosRepository
    {
        GenericRepository connection;

        public UsuariosRepository()
        {
            connection = new GenericRepository();
        }

        // Crear UsuarioL
        public int AddUsuarioL(UsuarioL UsuarioL)
        {
            var query = @"INSERT INTO USUARIOS (Usuario, Password)
                      VALUES (@Usuario, @Password);
                      SELECT CAST(SCOPE_IDENTITY() as int);";

            return connection.Execute(query, UsuarioL);
        }

        // Leer UsuarioL
        public UsuarioL GetUsuarioLById(int id)
        {
            var query = "SELECT * FROM USUARIOS WHERE Id = @Id";
            return connection.QuerySingle<UsuarioL>(query, new { Id = id });
        }

        // Actualizar UsuarioL
        public bool UpdateUsuarioL(UsuarioL UsuarioL)
        {
            var query = "UPDATE USUARIOS SET Usuario = @Usuario, Password = @Password WHERE Id = @Id";
            return connection.Execute(query, UsuarioL) > 0;
        }

        // Eliminar UsuarioL
        public bool DeleteUsuarioL(int id)
        {
            var query = "DELETE FROM USUARIOS WHERE Id = @Id";
            return connection.Execute(query, new { Id = id }) > 0;
        }

        // Leer UsuarioL
        public List<UsuarioL> GetAllUsuarios()
        {
            var query = "SELECT * FROM Usuarios";
            return connection.Query<UsuarioL>(query).ToList();
        }

        public UsuarioL ValidarUsuario(string usuario, string pass)
        {
            var query = "SELECT *FROM Usuarios WHERE Usuario = @usuario AND Password = @pass";
            return connection.QuerySingle< UsuarioL> (query, new { usuario = usuario, pass = pass });
        }
    }
}
