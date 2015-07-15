using System;
using System.Collections.Generic;
using System.Linq;
using dax.Extensions;
using System.Text;
using System.Threading.Tasks;

namespace dax.Document
{
    public class QueryContainer : Query
    {
        private readonly List<Query> _subQueries;

        protected QueryContainer(String content)
            :base(content)
        {
            _subQueries = ParseSubqueries(content);
        }
        
        private static List<Query> ParseSubqueries(String content)
        {
            List<Query> result = new List<Query>();
            String parsing = content;

            while (parsing.Length > 0)
            {
                int index = content.IndexOfAny("[[", "]]");

                if (index == -1)
                {
                    // last query block found
                    result.Add(new Query(parsing));
                    break;
                }

                if (index > 0)
                {
                    result.Add(new Query(parsing.Substring(0, index)));
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
                    result.Add(new Query(block, true));
                }

                indexEnd += 2;

                if (indexEnd >= parsing.Length)
                {
                    break;
                }

                parsing = parsing.Substring(indexEnd + 2);
            }

            return result;
        }
    }
}
