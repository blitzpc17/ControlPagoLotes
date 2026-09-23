using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.ADOS
{
    public class UsuariosRepository : IDisposable
    {
        GenericRepository connection;

        public long ConnectionId => connection.CurrentConnectionId;
        public string Plaza => connection.CurrentPlaza;

        public void Dispose()
        {
        }

        public UsuariosRepository()
        {
            connection = new GenericRepository();
        }

        public UsuariosRepository(long connectionId)
        {
            connection = new GenericRepository(connectionId);
        }

        public UsuariosRepository(string explicitConnectionString, string plaza = null, long connectionId = 0)
        {
            connection = new GenericRepository(explicitConnectionString, plaza, connectionId);
        }

        public UsuarioL GetUsuarioByNombre(string nombreUsuario)
        {
            var query = "SELECT TOP 1 * FROM Usuarios WHERE UPPER(Usuario) = UPPER(@usuario)";
            return connection.QuerySingle<UsuarioL>(query, new { usuario = nombreUsuario });
        }

        // Crear UsuarioL
        public int AddUsuarioL(UsuarioL UsuarioL)
        {
            var query = @"INSERT INTO USUARIOS (Usuario, Password)
                      VALUES (@Usuario, @Password);
                      SELECT CAST(SCOPE_IDENTITY() as int);";

            return connection.ExecuteScalar(query, UsuarioL);
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
