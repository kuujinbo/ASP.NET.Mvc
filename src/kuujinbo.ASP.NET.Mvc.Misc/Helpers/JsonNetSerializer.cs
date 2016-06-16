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

        public static string Get(
            object value, 
            string dateFormat = AppDateFormat,
            bool displayFor = false)
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

                if (displayFor)
                {
                    settings.Converters.Add(new SimpleEnumConverter());
                    settings.Converters.Add(new BoolYesNoConverter());
                }

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

    /* =======================================================================
     * 'true' and 'false' are obviously not end-user friendly
     * =======================================================================
     */
    public class BoolYesNoConverter : JsonConverter
    {
        public const string YES = "Yes";
        public const string NO = "No";
        public string Yes { get; set; }
        public string No { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return true; } }
 
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            Object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.Value.ToString().ToLower().Trim())
            {
                case "yes":
                case "y":
                case "true":
                    return true;
                case "no":
                case "n":
                case "false":
                    return false;
            }

            // unrecognized - let Json.NET throw
            return new JsonSerializer().Deserialize(reader, objectType);
        }

        public override void WriteJson(
            JsonWriter writer, 
            object value, 
            JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? "Yes" ?? YES : "No" ?? NO);
        }
    }
}