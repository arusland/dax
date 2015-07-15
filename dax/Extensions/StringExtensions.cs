using System;
using System.Linq;
using System.Runtime.InteropServices;
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
                secureString.MakeReadOnly();
            }
        }

        public static String FromSecureString(this SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;

            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static int IndexOfAny(this String value, params String[] subStrs)
        {
            var idx = subStrs.Select(p => value.IndexOf(p))
                .Where(p => p != -1);

            if (idx.Any())
            {
                return idx.Min();
            }

            return -1;
        }
    }
}
