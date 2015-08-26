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

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace dax.Db.SqlServer
{
    public class SqlServerQueryBlock : IQueryBlock
    {
        private readonly String _query;
        private readonly string _connectionString;
        private DataTable _table;
        private int _pageIndex;

        public SqlServerQueryBlock(String query, String connectionString)
        {
            this._query = query;
            this._connectionString = connectionString;
        }

        public DataTable Table
        {
            get { return _table; }
        }

        public bool IsEmpty
        {
            get { return Table == null || Table.Rows.Count == 0; }
        }

        public bool IsSkipped
        {
            get { return false; }
        }

        public int PageSize
        {
            get { return 25; }
        }

        public long ElapsedTime
        {
            get;
            private set;
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
                    var watch = Stopwatch.StartNew();

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

                    watch.Stop();
                    ElapsedTime = watch.ElapsedMilliseconds;
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
