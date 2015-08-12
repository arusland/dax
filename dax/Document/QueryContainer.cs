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

using dax.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dax.Document
{
    public class QueryContainer : Query
    {
        private readonly List<Query> _subQueries;

        public QueryContainer(String content, bool skipWhenNoInput)
        {
            Content = content;
            SkipWhenNoInput = skipWhenNoInput;
            _subQueries = ParseSubqueries(content, SkipWhenNoInput);
        }

        public override string Content
        {
            get;
            protected set;
        }

        public override bool Conditional
        {
            get;
            protected set;
        }

        public bool SkipWhenNoInput
        {
            get;
            private set;
        }


        public override IEnumerable<string> Variables
        {
            get { return _subQueries.SelectMany(p => p.Variables); }
        }

        public override string BuildQuery(Dictionary<string, string> inputValues)
        {
            StringBuilder result = new StringBuilder();

            foreach (Query query in _subQueries)
            {
                if (!query.Conditional || query.CanExecute(inputValues))
                {
                    result.Append(query.BuildQuery(inputValues));
                }
            }

            return result.ToString();
        }

        public override bool CanExecute(Dictionary<string, string> inputValues)
        {
            var notConditionalQueries = _subQueries.Where(p => !p.Conditional).ToList();

            bool canExecute = notConditionalQueries.Count == 0 || notConditionalQueries.All(p => p.CanExecute(inputValues));

            if (SkipWhenNoInput && canExecute)
            {
                var conditionalQueries = _subQueries.Where(p => p.Conditional).ToList();

                if (conditionalQueries.Count > 0)
                {
                    // we can execute block when we have at least one input value for conditional subquery in case SkipWhenNoInput=true
                    canExecute = conditionalQueries.Any(p => p.CanExecute(inputValues));
                }
            }

            return canExecute;
        }

        public static bool IsAcceptable(String value)
        {
            return IndexOfAny(value) >= 0;
        }

        private static List<Query> ParseSubqueries(String content, bool skipWhenNoInput)
        {
            List<Query> result = new List<Query>();
            String parsing = content;

            while (parsing.Length > 0)
            {
                int index = IndexOfAny(parsing);

                if (index == -1)
                {
                    // last query block found
                    result.Add(Query.NewQuery(parsing, false, false));
                    break;
                }

                if (index > 0)
                {
                    result.Add(Query.NewQuery(parsing.Substring(0, index), false, false));
                }

                if (parsing[index] == ']')
                {
                    throw new InvalidOperationException("Found closing token ']]' without opening '[[' in query");
                }

                // checking edges of closing token ']]'
                if ((index + 3) >= parsing.Length)
                {
                    throw new InvalidOperationException("Not found closing token ']]'");
                }

                index += 2;

                int indexEnd = parsing.IndexOf("]]", index);

                if (indexEnd == -1)
                {
                    throw new InvalidOperationException("Not found closing token ']]'");
                }

                if (indexEnd > index)
                {
                    String block = parsing.Substring(index, indexEnd - index);
                    result.Add(Query.NewQuery(block, true, skipWhenNoInput));
                }

                indexEnd += 2;

                if (indexEnd >= parsing.Length)
                {
                    break;
                }

                parsing = parsing.Substring(indexEnd);
            }

            return result;
        }

        private static int IndexOfAny(String value)
        {
            return value.IndexOfAny("[[", "]]");
        }
    }
}
