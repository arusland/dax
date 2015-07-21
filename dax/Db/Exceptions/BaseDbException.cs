using System;

namespace dax.Db.Exceptions
{
    public class BaseDbException : Exception
    {
        public BaseDbException(String message, Exception innerException)
            :base(message, innerException)
        {
        }
    }
}
