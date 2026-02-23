using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{




    public static class SqlServerConnectionStringBuilderHelper
    {
        public static string Build(
            string server,
            int port,
            string database,
            bool useWindowsAuth,
            string user,
            string pass,
            bool encrypt = true,
            bool trustServerCertificate = true,
            int timeout = 30)
        {
            var b = new SqlConnectionStringBuilder
            {
                DataSource = port > 0 ? $"{server},{port}" : server,
                InitialCatalog = database,
                IntegratedSecurity = useWindowsAuth,
                Encrypt = encrypt,
                TrustServerCertificate = trustServerCertificate,
                ConnectTimeout = timeout
            };

            if (!useWindowsAuth)
            {
                b.UserID = user;
                b.Password = pass;
            }

            return b.ConnectionString;
        }
    }

}
