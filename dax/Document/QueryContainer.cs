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

        public QueryContainer(String content)
        {
            Content = content;
            _subQueries = ParseSubqueries(content);
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
            var notConditionals = _subQueries.Where(p => !p.Conditional).ToList();

            return notConditionals.Count == 0 || notConditionals.All(p => p.CanExecute(inputValues));
        }

        public static bool IsAcceptable(String value)
        {
            return IndexOfAny(value) >= 0;
        }
        
        private static List<Query> ParseSubqueries(String content)
        {
            List<Query> result = new List<Query>();
            String parsing = content;

            while (parsing.Length > 0)
            {
                int index = IndexOfAny(parsing);

                if (index == -1)
                {
                    // last query block found
                    result.Add(Query.NewQuery(parsing, false));
                    break;
                }

                if (index > 0)
                {
                    result.Add(Query.NewQuery(parsing.Substring(0, index), false));
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
                    result.Add(Query.NewQuery(block, true));
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
