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

using dax.Db.Connect;
using dax.Db.SqlServer;
using System;
using System.Data.SqlClient;

namespace dax.Db
{
    public sealed class ProviderFactory : IProviderFactory, IConnectionStringParser
    {
        public static readonly ProviderFactory Instance = new ProviderFactory();

        private ProviderFactory()
        {
        }

        public IDbProvider Create(IConnection connection)
        {
            return new SqlServerProvider(connection);
        }

        public IConnection NewConnection(String serverName, String dbName, String login, String password)
        {
            return new SqlServerConnection(serverName, dbName, login, password);
        }        

        IConnection IConnectionStringParser.Parse(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);

            return NewConnection(builder.DataSource, builder.InitialCatalog, builder.UserID, builder.Password);
        }
    }
}
