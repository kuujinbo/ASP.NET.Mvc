using System;

namespace kuujinbo.ASP.NET.Mvc.Helpers
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// get the next specified day of the week, inclusive
        /// </summary>
        /// <param name="start">start date for calculation</param>
        /// <param name="wanted">wanted day of week</param>
        /// <returns>
        /// next closest wanted day of week from start date, inclusive
        /// </returns>
        public static DateTime NextDayInclusive(this DateTime start, DayOfWeek wanted)
        {
            // sanity-check - reset time part
            start = new DateTime(start.Year, start.Month, start.Day);
            int startDay = (int)start.DayOfWeek;
            int wantedDay = (int)wanted;
            if (wantedDay < startDay) wantedDay += 7;

            return start.AddDays(wantedDay - startDay);
        }
    }
}