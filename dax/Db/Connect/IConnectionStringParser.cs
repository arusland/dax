using System;

namespace dax.Db.Connect
{
    public interface IConnectionStringParser
    {
        IConnection Parse(String connectionString);
    }
}
