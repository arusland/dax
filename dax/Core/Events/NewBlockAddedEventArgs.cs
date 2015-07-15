using dax.Db;
using dax.Document;
using System;

namespace dax.Core.Events
{
    public class NewBlockAddedEventArgs : EventArgs
    {
        public NewBlockAddedEventArgs(Block block, IQueryBlock queryBlock)
        {
            Block = block;
            QueryBlock = queryBlock;
        }

        public Block Block
        {
            get;
            private set;
        }

        public IQueryBlock QueryBlock
        {
            get;
            private set;
        }

        public bool Canceled
        {
            get;
            set;
        }
    }
}
