using System;

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

        public object ExecuteScalar(string query)
        {
            throw new NotImplementedException();
        }
    }
}
