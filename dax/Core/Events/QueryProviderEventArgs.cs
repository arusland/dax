using dax.Db;
using System;

namespace dax.Core.Events
{
    public class QueryProviderEventArgs : EventArgs
    {
        public QueryProviderEventArgs(DaxManager manager, bool reset = false)
        {
        	Manager = manager;
            Reset = reset;
        }
        
        public IDbProvider Provider
        {
            get;
            set;
        }

        public DaxManager Manager
        {
            get;
            private set;
        }

        public bool Reset
        {
            get;
            private set;
        }
    }
}
