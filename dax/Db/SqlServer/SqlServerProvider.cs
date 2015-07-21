using dax.Db.Connect;
using System;
using System.Data.SqlClient;

namespace dax.Db.SqlServer
{
    public class SqlServerProvider : IDbProvider
    {
        public SqlServerProvider(IConnection connection)
        {
            Connection = connection;
        }

        public IConnection Connection
        {
            get;
            private set;
        }

        public IQueryBlock CreateBlock(String query)
        {
            return new SqlServerQueryBlock(query, Connection.ConnectionString);
        }

        public object ExecuteScalar(String query)
        {
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    Object result = command.ExecuteScalar();
                    connection.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    SqlServerErrorDispatcher.Handle(ex, query);
                    throw;
                }
            }
        }
    }
}
