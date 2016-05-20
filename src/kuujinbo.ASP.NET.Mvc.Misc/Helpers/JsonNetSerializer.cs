using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Web.Mvc
{
    public class JsonNetSerializer
    {
        public const string AppDateFormat = "M/d/yyyy";
        public const string BAD_DATE_FORMAT = "unrecognized date format";

        public static string Get(object value)
        {
            return Get(value, AppDateFormat);
        }
        public static string Get(object value, string dateFormat)
        {
            if (value == null) throw new System.ArgumentNullException("value");

            if (string.IsNullOrWhiteSpace(dateFormat)) dateFormat = AppDateFormat;

            if (dateFormat == AppDateFormat || IsValidDateFormat(dateFormat))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.Converters.Add(
                    new IsoDateTimeConverter() { DateTimeFormat = dateFormat }
                );

                    // MVC cannot handle microsoft's JSON date serialization
                return JsonConvert.SerializeObject(
                    value, Formatting.Indented, settings
                );
            }

            throw new FormatException(BAD_DATE_FORMAT);

        }

        private static bool IsValidDateFormat(string dateFormat)
        {
            DateTime outDate;
            return DateTime.TryParse(
                DateTime.Now.ToString(dateFormat), out outDate
            );
        }
    }
}