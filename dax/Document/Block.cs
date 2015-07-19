using System;
using System.Collections.Generic;
using System.Linq;

namespace dax.Document
{
    public class Block
    {
        public Block(String title, Query query, IEnumerable<Binding> bindings, bool showOnEmpty)
        {
            Title = title;
            Query = query;
            ShowOnEmpty = showOnEmpty;
            Bindings = bindings.ToList();
        }                

        public String Title
        {
            get;
            private set;
        }

        public bool ShowOnEmpty
        {
            get;
            private set;
        }

        public Query Query
        {
            get;
            private set;
        }

        public IEnumerable<Binding> Bindings
        {
            get;
            private set;
        }

        public IEnumerable<String> Variables
        {
            get { return Query.Variables; }
        }

        public bool CanExecute(Dictionary<String, String> inputValues)
        {
            return Query.CanExecute(inputValues);
        }        

        public override String ToString()
        {
            return String.Format("Title={0}", Title);
        }
    }
}
