using System;

namespace dax.Core.Events
{
    public class ErrorEventArgs : EventArgs
    {
        #region Ctors
        
        public ErrorEventArgs(String message)
        {
            Message = message;
        }
        
        #endregion

        public String Message
        {
            get;
            private set;
        }
    }
}
