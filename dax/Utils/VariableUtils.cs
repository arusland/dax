using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace dax.Utils
{
    public static class VariableUtils
    {
        private readonly static Regex VAR_PATTERN = new Regex(@"{{([\w_]+)}}");

        public static List<string> ParseVariables(string content)
        {
            List<string> result = new List<string>();

            if (!String.IsNullOrWhiteSpace(content))
            {
                content = content.Trim();
                var matches = VAR_PATTERN.Matches(content);

                foreach (Match mc in matches)
                {
                    result.Add(mc.Groups[1].Value);
                }
            }

            return result.Distinct().ToList();
        }

        public static String BuildQuery(String content, List<String> variables, Dictionary<String, Object> map)
        {
            var varNotFound = variables.FirstOrDefault(p => !map.ContainsKey(p));

            if (varNotFound != null)
            {
                throw new InvalidOperationException("Value not found for variable: " + varNotFound);
            }

            String query = content;

            foreach (var variable in variables)
            {
                query = query.Replace("{{" + variable + "}}", map[variable].ToString());
            }

            return query;
        }
    }
}
