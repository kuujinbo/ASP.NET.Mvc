/* =======================================================================
 * 'true' and 'false' are obviously not user friendly. use converter to 
 * **serialize** (display only) bool to more meaningful values
 * =======================================================================
 */
using System;
using Newtonsoft.Json;

namespace kuujinbo.ASP.NET.Mvc.Services.Json
{
    public class WriteBoolConverter : JsonConverter
    {
        /// <summary>
        /// default replacement for true
        /// </summary>
        public const string TRUE = "True";
        /// <summary>
        /// default replacement for false
        /// </summary>
        public const string FALSE = "False";

        /* --------------------------------------------------------------------
         * or set even simpler strings. e.g. true => 'Y', false => 'N'
         * --------------------------------------------------------------------
         */
        /// <summary>true</summary>
        public string True { get; set; }
        /// <summary>false</summary>
        public string False { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            Object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException(
                "CanRead is false. The type will skip the converter."
            );
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? True ?? TRUE : False ?? FALSE);
        }
    }
}