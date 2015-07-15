using dax.Db.Connect;
using System;

namespace dax.Db
{
    public interface IDbProvider
    {
        IConnection Connection
        {
            get;
        }

        IQueryBlock CreateBlock(String query);

        Object ExecuteScalar(String query);
    }
}
