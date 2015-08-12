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

using dax.Db.Exceptions;
using dax.Utils;
using System;
using System.Data.SqlClient;

namespace dax.Db.SqlServer
{
    public static class SqlServerErrorDispatcher
    {
        private const int ERROR_SERVER_NOT_SUPPORT_PROTOCOL = -1;
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
