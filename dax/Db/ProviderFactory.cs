﻿using dax.Db.Connect;
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
