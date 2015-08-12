/*
 * Copyright 2015 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using dax.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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
