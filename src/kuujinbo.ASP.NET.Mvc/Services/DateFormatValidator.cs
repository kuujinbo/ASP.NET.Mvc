using System;

namespace kuujinbo.ASP.NET.Mvc.Services
{
    public static class DateFormatValidator
    {
        public const string BAD_DATE_FORMAT = "unrecognized date format {0}";

        public static bool Parse(string format)
        {
            DateTime outDate;
            if (!DateTime.TryParse(DateTime.Now.ToString(format), out outDate))
            {
                throw new FormatException(
                    string.Format(BAD_DATE_FORMAT, format)
                );
            }

            return true;
        }
    }
}