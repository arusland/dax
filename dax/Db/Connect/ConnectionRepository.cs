using dax.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dax.Db.Connect
{
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly IConnectionStringParser _connectionParser;
        private readonly List<IConnection> _connections = new List<IConnection>();

        public ConnectionRepository(IConnectionStringParser connectionParser)
        {
            _connectionParser = connectionParser;
        }

        public IEnumerable<IConnection> Connections
        {
            get { return _connections; }
        }

        public void Add(IConnection connection)
        {
            _connections.Add(connection);
        }

        public void Remove(IConnection connection)
        {
            _connections.Remove(connection);
        }

        public void Save()
        {
            Settings.Default.ConnectionStrings.Clear();
            Settings.Default.ConnectionStrings
                .AddRange(_connections.Select(p => p.ConnectionString).ToArray());
            Settings.Default.Save();
        }

        public void Load()
        {
            _connections.Clear();
            _connections.AddRange(Settings.Default.ConnectionStrings
                .Cast<String>()
                .Select(p => _connectionParser.Parse(p)));
        }
    }
}
