using System;

namespace dax.Core.Document
{
    public class Query
    {
        public Query(String content)
        {
            Content = content;
        }

        public String Content
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return String.Format("Conent={0}", Content);
        }
    }
}
