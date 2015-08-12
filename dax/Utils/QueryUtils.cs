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
using System.Text.RegularExpressions;

namespace dax.Utils
{
    public static class QueryUtils
    {
        private readonly static Regex CLEAR_PATTERN = new Regex(@"[\t ]+");
        private readonly static Regex CLEAR_PATTERN2 = new Regex(@"\n +");

        public static String Normalize(String query)
        {
            String result = CLEAR_PATTERN.Replace(query, " ").Trim();
            return CLEAR_PATTERN2.Replace(result, Environment.NewLine);
        }
    }
}
