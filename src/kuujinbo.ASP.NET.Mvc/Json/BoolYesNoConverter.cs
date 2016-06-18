/* =======================================================================
 * 'true' and 'false' are obviously not end-user friendly
 * =======================================================================
 */
using System;
using Newtonsoft.Json;

namespace kuujinbo.ASP.NET.Mvc.Json
{
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
            writer.WriteValue(((bool)value) ? Yes ?? YES : No ?? NO);
        }
    }
}