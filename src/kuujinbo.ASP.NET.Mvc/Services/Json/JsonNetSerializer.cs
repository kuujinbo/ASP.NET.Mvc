/* ============================================================================

 * ============================================================================
 */
using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace kuujinbo.ASP.NET.Mvc.Services.Json
{
    public class JsonNetSerializer
    {
        private JsonSerializerSettings _jsonSettings;

        private static readonly IsoDateTimeConverter _isoDateTimeConverter;
        private static readonly WriteBoolConverter _writeBoolConverter;
        private static readonly string _appDateFormat;
        static JsonNetSerializer()
        {
            _writeBoolConverter = new WriteBoolConverter(
                TableSettings.Settings.BoolTrue,
                TableSettings.Settings.BoolFalse 
            );

            _appDateFormat = TableSettings.Settings.DateFormat;
            DateFormatValidator.Parse(_appDateFormat);
            _isoDateTimeConverter = new IsoDateTimeConverter()
            {
                DateTimeFormat = _appDateFormat
            };
        }

        public JsonNetSerializer()
        {
            _jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            _jsonSettings.Converters.Add(_isoDateTimeConverter);
        }

        public string Get(object value, bool addConverters = false)
        {
            if (value == null) throw new System.ArgumentNullException("value");

            if (addConverters)
            {
                _jsonSettings.Converters.Add(new WriteEnumConverter());
                _jsonSettings.Converters.Add(_writeBoolConverter);
            }

            return JsonConvert.SerializeObject(
                value, Formatting.Indented, _jsonSettings
            );
        }
    }
}