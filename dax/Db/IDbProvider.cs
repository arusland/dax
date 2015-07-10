using System;

namespace dax.Db
{
    public interface IDbProvider
    {
        IQueryBlock CreateBlock(String query);

        Object ExecuteScalar(String query);
    }
}
