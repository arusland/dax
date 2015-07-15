using dax.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dax.Document
{
    public class Block
    {
        private readonly List<String> _variables;

        public Block(String title, Query query, bool showOnEmpty)
        {
            Title = title;
            Query = query;
            _variables = GetVariables(Title, Query);
            ShowOnEmpty = showOnEmpty;
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

        public List<String> Variables
        {
            get { return _variables; }
        }

        public bool CanExecute(Dictionary<String, String> inputValues)
        {
            return _variables.Count == 0 || _variables.All(p => inputValues.ContainsKey(p));
        }

        private static List<string> GetVariables(string Title, Query Query)
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
