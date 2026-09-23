using DAO;
using Dapper;
using System;


namespace DAO.ADOS {

    public class VariablesGlobalesRepository
    {
        private readonly GenericRepository connection;

        public long ConnectionId => connection.CurrentConnectionId;
        public string Plaza => connection.CurrentPlaza;

        public VariablesGlobalesRepository()
        {
            connection = new GenericRepository();
        }

        public VariablesGlobalesRepository(long connectionId)
        {
            connection = new GenericRepository(connectionId);
        }

        public VariablesGlobalesRepository(string explicitConnectionString, string plaza = null, long connectionId = 0)
        {
            connection = new GenericRepository(explicitConnectionString, plaza, connectionId);
        }

        public string GetValorByLabel(string label)
        {
            var sql = "SELECT TOP 1 valor FROM VARIABLESGLOBALES WHERE label = @label;";
            return connection.QuerySingle<string>(sql, new { label });
        }

        public void UpsertValor(string label, string valor)
        {
            // Si no existe, lo crea. Si existe, lo actualiza.
            var sql = @"
IF EXISTS(SELECT 1 FROM VARIABLESGLOBALES WHERE label = @label)
BEGIN
    UPDATE VARIABLESGLOBALES
    SET valor = @valor
    WHERE label = @label;
END
ELSE
BEGIN
    INSERT INTO VARIABLESGLOBALES(label, valor)
    VALUES(@label, @valor);
END";
            connection.Execute(sql, new { label, valor });
        }
    }

}