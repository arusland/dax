using dax.Utils;
using dax.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace dax.Document
{
    public class QueryBasic : Query
    {
        private readonly List<String> _variables = new List<String>();

        public QueryBasic(String content)
            : this(content, false)
        {
        }

        public QueryBasic(String content, bool conditional)
        {
            Conditional = conditional;
            Content = content ?? String.Empty;
            _variables = VariableUtils.ParseVariables(Content);
        }

        public override String Content
        {
            get;
            protected set;
        }

        public override bool Conditional
        {
            get;
            protected set;
        }

        public override IEnumerable<String> Variables
        {
            get { return _variables; }
        }

        public override String BuildQuery(Dictionary<String, String> map)
        {
            return VariableUtils.BuildQuery(Content, _variables, map);
        }

        public override bool CanExecute(Dictionary<String, String> inputValues)
        {
            return _variables.Count == 0 || _variables.All(p => inputValues.ContainsKey(p));
        }

        public override String ToString()
        {
            return String.Format("Conent={0}", Content);
        }
    }
}
