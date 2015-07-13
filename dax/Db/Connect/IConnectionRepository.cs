using System;
using System.Collections.Generic;

namespace dax.Db.Connect
{
    public interface IConnectionRepository
    {
        IEnumerable<IConnection> Connections
        {
            get;
        }

        void Add(IConnection connection);

        void Remove(IConnection connection);

        IConnection New(String serverName, String dbName, String login, String password);
    }
}
