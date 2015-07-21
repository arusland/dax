using System;

namespace dax.Db.Exceptions
{
    public class QueryExecuteException : BaseDbException
    {
        public QueryExecuteException(Exception innerException, String query)
            : base("Query executing failed", innerException)
        {
            Query = query;
        }

        public String Query
        {
            get;
            private set;
        }
    }
}
