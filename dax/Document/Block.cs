using dax.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dax.Document
{
    public class Block
    {
        private readonly List<String> _variables;
        
        public Block(String title, Query query)
        {
            Title = title;
            Query = query;
            _variables = GetVariables(Title, Query);
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

        public List<String> Variables
        {
            get { return _variables; }
        }

        public bool CanAccept(Dictionary<String, String> inputValues)
        {
            return _variables.All(p => inputValues.ContainsKey(p));
        }

        private static List<string> GetVariables(string Title, Document.Query Query)
        {
            var vars = VariableUtils.ParseVariables(Title);

            vars.AddRange(Query.Variables);

            return vars.Distinct().ToList();
        }

        public override String ToString()
        {
            return String.Format("Title={0}", Title);
        }
    }
}
