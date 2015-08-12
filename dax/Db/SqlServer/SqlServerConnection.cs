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
using System.Threading.Tasks;

namespace dax.Db.SqlServer
{
    public class SqlServerConnection : IConnection
    {
        private const String CONNECTION_STRING_TEMPLATE = "data source={0};initial catalog={1};persist security info=True;user id={2};password={3}";
        private const String CONNECTION_CHECK_QUERY = "select count(1) from INFORMATION_SCHEMA.TABLES";

        public SqlServerConnection(string serverName, string dbName, string login, string password)
        {
            ServerName = serverName;
            DbName = dbName;
            Login = login;
            Password = password;
        }


        public string ServerName
        {
            get;
            private set;
        }

        public string DbName
        {
            get;
            private set;
        }

        public string Login
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public string ConnectionString
        {
            get { return String.Format(CONNECTION_STRING_TEMPLATE, ServerName, DbName, Login, Password); }
        }

        public Task<string> Test()
        {
            var task = Task.Factory.StartNew<String>(() =>
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        SqlCommand command = new SqlCommand(CONNECTION_CHECK_QUERY, connection);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }

                    return String.Empty;
                });

            return task;
        }
    }
}
