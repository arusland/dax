using System;
using System.Data.SqlClient;

namespace dax.Db.SqlServer
{
    public class SqlServerProvider : IDbProvider
    {
        private readonly String _connectionString;

        public SqlServerProvider(String connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryBlock CreateBlock(String query)
        {
            return new SqlServerQueryBlock(query, _connectionString);
        }

        public object ExecuteScalar(String query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    Object result = command.ExecuteScalar();
                    connection.Close();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
