using dax.Db.Connect;
using System;

namespace dax.Db
{
    public interface IProviderFactory
    {
        IDbProvider Create();

        IConnection NewConnection(String serverName, String dbName, String login, String password);
    }
}
