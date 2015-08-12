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
using System;
using System.Data.SqlClient;

namespace dax.Db.SqlServer
{
    public class SqlServerProvider : IDbProvider
    {
        public SqlServerProvider(IConnection connection)
        {
            Connection = connection;
        }

        public IConnection Connection
        {
            get;
            private set;
        }

        public IQueryBlock CreateBlock(String query)
        {
            return new SqlServerQueryBlock(query, Connection.ConnectionString);
        }

        public object ExecuteScalar(String query)
        {
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    Object result = command.ExecuteScalar();
                    connection.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    SqlServerErrorDispatcher.Handle(ex, query);
                    throw;
                }
            }
        }
    }
}
