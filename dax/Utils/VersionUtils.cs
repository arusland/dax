using System;

namespace dax.Utils
{
    public class VersionUtils
    {
        internal static String GetVersion()
        {
            var version = typeof(VersionUtils).Assembly.GetName().Version;

            return String.Format("{0}.{1}", version.Major, version.Minor);
        }
    }
}
