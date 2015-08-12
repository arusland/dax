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

        public static String BuildQuery(String content, List<String> variables, Dictionary<String, String> map)
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
