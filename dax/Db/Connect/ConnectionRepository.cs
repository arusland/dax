/*
 * Copyright 2015 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using dax.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            Settings.Default.ConnectionStrings = new StringCollection();
            Settings.Default.ConnectionStrings
                .AddRange(_connections.Select(p => p.ConnectionString).ToArray());
            Settings.Default.Save();            
        }

        public void Load()
        {
            _connections.Clear();

            if (Settings.Default.ConnectionStrings != null)
            {
                _connections.AddRange(Settings.Default.ConnectionStrings
                    .Cast<String>()
                    .Select(p => _connectionParser.Parse(p)));
            }
        }
    }
}
