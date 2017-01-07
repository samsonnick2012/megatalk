using System;

namespace Innostar.UI.Helpers
{
    public class UnixTimeHelper
    {
        public static DateTime GetDateFromLong(long timeTicks)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var date = start.AddMilliseconds(timeTicks);

            return date;
        }

        public static long GetLongFromDate(DateTime date)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
            var end = date.Ticks / 10000;

            return end - start;
        }
    }
}