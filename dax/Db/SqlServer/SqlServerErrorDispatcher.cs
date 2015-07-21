using dax.Db.Exceptions;
using dax.Utils;
using System;
using System.Data.SqlClient;

namespace dax.Db.SqlServer
{
    public static class SqlServerErrorDispatcher
    {
        private const int ERROR_SERVER_NOT_SUPPORT_PROTOCOL = -1;
        private const int ERROR_SERVER_TIMEOUT = -2;
        private const int ERROR_COULD_NOT_OPEN_CONNECTION = 2;
        private const int ERROR_COULD_NOT_OPEN_CONNECTION2 = 53;
        private const int ERROR_COULD_NOT_OPEN_DB = 4060;
        private const int ERROR_LOGIN_FAILED = 18456;

        public static void Handle(Exception exception, String query)
        {
            SqlException sqlError = exception as SqlException;

            if (sqlError != null)
            {
                switch (sqlError.Number)
                {
                    case ERROR_SERVER_NOT_SUPPORT_PROTOCOL:
                    case ERROR_SERVER_TIMEOUT:
                    case ERROR_COULD_NOT_OPEN_CONNECTION:
                    case ERROR_COULD_NOT_OPEN_CONNECTION2:
                    case ERROR_COULD_NOT_OPEN_DB:
                    case ERROR_LOGIN_FAILED:
                        throw exception;
                    default:
                        throw new QueryExecuteException(exception, QueryUtils.Normalize(query));
                }
            }
            else
            {
                throw exception;
            }
        }
    }
}
