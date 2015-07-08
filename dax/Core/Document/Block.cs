using System;

namespace dax.Core.Document
{
    public class Block
    {
        public Block(String title, Query query)
        {
            Title = title;
            Query = query;
        }        

        public String Title
        {
            get;
            private set;
        }

        public Query Query
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return String.Format("Title={0}", Title);
        }
    }
}
