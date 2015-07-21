using System;

namespace dax.Core.Events
{
    public class ErrorEventArgs : EventArgs
    {
        #region Ctors


        public ErrorEventArgs(String message)
            : this(message, String.Empty)
        {
        }

        public ErrorEventArgs(String message, String query)
        {
            Message = message;
            Query = query;
        }

        #endregion

        public String Message
        {
            get;
            private set;
        }

        public String Query
        {
            get;
            private set;
        }
    }
}
