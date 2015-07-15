using dax.Utils;
using dax.Extensions;
using System;
using System.Collections.Generic;

namespace dax.Document
{
    // TODO: Make Query pure abstract class!!!
    public class Query
    {
        private readonly List<String> _variables = new List<String>();

        protected Query(String content)
            : this(content, false)
        {
        }

        protected Query(String content, bool conditional)
        {
            Conditional = conditional;
            Content = content ?? String.Empty;
            _variables = VariableUtils.ParseVariables(Content);
        }

        public virtual String Content
        {
            get;
            private set;
        }

        public virtual bool Conditional
        {
            get;
            private set;
        }

        public virtual IEnumerable<String> Variables
        {
            get { return _variables; }
        }

        public virtual String BuildQuery(Dictionary<String, String> map)
        {
            return VariableUtils.BuildQuery(Content, _variables, map);
        }

        public override String ToString()
        {
            return String.Format("Conent={0}", Content);
        }       

        public static Query NewQuery(String content)
        {

        }
    }
}
