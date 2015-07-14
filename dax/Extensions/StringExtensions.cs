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
    }
}
