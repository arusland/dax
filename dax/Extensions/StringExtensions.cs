using System;
using System.Linq;
using System.Security;

namespace dax.Extensions
{
    public static class StringExtensions
    {
        public static void ToSecureString(this String value, SecureString secureString)
        {
            secureString.Clear();

            if (value != null)
            {
                Array.ForEach(value.ToArray(), secureString.AppendChar);
            }
        }
    }
}
