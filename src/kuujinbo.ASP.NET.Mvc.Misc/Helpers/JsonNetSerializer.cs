using kuujinbo.ASP.NET.Mvc.Misc.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;

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
                settings.Converters.Add(new SimpleEnumConverter());

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

    public class SimpleEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(RegexUtils.PascalCaseSplit(value.ToString()));
        }
    }
}