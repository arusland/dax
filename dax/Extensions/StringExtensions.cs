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
