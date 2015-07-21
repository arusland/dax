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
