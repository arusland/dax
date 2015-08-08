using System;
using System.Collections.Generic;

namespace dax.Document
{
    public abstract class Query
    {
        public abstract String Content
        {
            get;
            protected set;
        }

        public abstract bool Conditional
        {
            get;
            protected set;
        }

        public abstract IEnumerable<String> Variables
        {
            get;
        }

        public abstract String BuildQuery(Dictionary<String, String> inputValues);

        public abstract bool CanExecute(Dictionary<String, String> inputValues);

        public override String ToString()
        {
            return String.Format("Conent={0}", Content);
        }

        public static Query NewQuery(String content, bool conditional, bool skipWhenNoInput)
        {
            if (QueryContainer.IsAcceptable(content))
            {
                return new QueryContainer(content, skipWhenNoInput);
            }

            return new QueryBasic(content, conditional);
        }
    }
}
