using dax.Utils;
using System;
using System.Collections.Generic;

namespace dax.Core.Document
{
    public class Query
    {
        private readonly List<String> _variables = new List<String>();
        
        public Query(String content)
        {
            Content = (content ?? String.Empty).Trim();
            _variables = VariableUtils.ParseVariables(Content);
        }        

        public String Content
        {
            get;
            private set;
        }

        public IEnumerable<String> Variables
        {
            get { return _variables; }
        }

        public String BuildQuery(Dictionary<String, Object> map)
        {
            return VariableUtils.BuildQuery(Content, _variables, map);
        }        

        public override String ToString()
        {
            return String.Format("Conent={0}", Content);
        }
    }
}
