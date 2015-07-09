using System;
using System.Data;
using System.Data.SqlClient;

namespace dax.Db.SqlServer
{
    class SqlServerQueryBlock : IQueryBlock
    {
        private readonly String _query;
        private string _connectionString;
        private DataTable _table;

        public SqlServerQueryBlock(String query, String _connectionString)
        {
            this._query = query;
            this._connectionString = _connectionString;
        }

        public System.Data.DataTable Table
        {
            get { return _table; }
        }

        public int PageSize
        {
            get { return 25; }
        }

        public int PageIndex
        {
            get;
            private set;
        }

        public void Update()
        {
            UpdateInternal();
        }

        public void NextPage()
        {
            PageIndex++;
            UpdateInternal();
        }

        public void PrevPage()
        {
            PageIndex = Math.Max(0, PageIndex - 1);
            UpdateInternal();
        }

        private void UpdateInternal()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(_query, connection);
                int rowBegin = PageSize * PageIndex;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    var table = CreateTable(reader);

                    int rowCount = 0;

                    while (reader.Read())
                    {
                        if (rowCount++ >= rowBegin)
                        {
                            var row = table.NewRow();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i];
                            }

                            table.Rows.Add(row);

                            if (table.Rows.Count >= PageSize)
                            {
                                break;
                            }
                        }
                    }

                    _table = table;

                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private DataTable CreateTable(SqlDataReader reader)
        {
            var ds = new DataSet();
            DataTable table = ds.Tables.Add("temp");

            // Add the table columns.
            for (int i = 0; i < reader.FieldCount; i++)
                table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));

            return table;
        }
    }
}
