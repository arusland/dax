using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace dax.Db.SqlServer
{
    class SqlServerQueryBlock : IQueryBlock
    {
        private readonly String _query;
        private string _connectionString;
        private DataTable _table;
        private int _pageIndex;

        public SqlServerQueryBlock(String query, String _connectionString)
        {
            this._query = query;
            this._connectionString = _connectionString;
        }

        public DataTable Table
        {
            get { return _table; }
        }

        public bool IsEmpty
        {
            get { return Table == null || Table.Rows.Count == 0; }
        }

        public int PageSize
        {
            get { return 25; }
        }

        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = Math.Max(0, value);
            }
        }

        public string QueryText
        {
            get { return _query; }
        }

        public void Update()
        {
            UpdateInternal();
        }

        public async Task UpdateAsync()
        {
            Task task = Task.Factory.StartNew(Update);
            await task;
        }

        public void NextPage()
        {
            PageIndex++;
            UpdateInternal();
        }

        public async Task NextPageAsync()
        {
            Task task = Task.Factory.StartNew(NextPage);
            await task;
        }

        public void PrevPage()
        {
            PageIndex = PageIndex - 1;
            UpdateInternal();
        }

        public async Task PrevPageAsync()
        {
            Task task = Task.Factory.StartNew(PrevPage);
            await task;
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
                    SqlServerErrorDispatcher.Handle(ex, _query);
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
