using dax.Db.Connect;
using System;

namespace dax.Extensions
{
    public static class ConnectionExtensions
    {
        public static String Format(this IConnection connection)
        {
            return String.Format("{0}.{1}.{2}", connection.ServerName, connection.DbName, connection.Login);
        }
    }
}
