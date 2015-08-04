using System;

namespace dax.Utils
{
    public static class TimeUtils
    {
        public static String PrettifyTime(long ms)
        {
            if (ms >= 1000)
            {
                return String.Format("{0:F2} sec", (decimal)ms / 1000);
            }

            return String.Format("{0} ms", ms);
        }
    }
}
