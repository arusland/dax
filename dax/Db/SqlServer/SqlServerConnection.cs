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
