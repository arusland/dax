using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
