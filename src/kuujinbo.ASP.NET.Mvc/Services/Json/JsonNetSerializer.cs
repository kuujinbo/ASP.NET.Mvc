/* ============================================================================

 * ============================================================================
 */
using System;
using System.Collections.Generic;
using kuujinbo.ASP.NET.Mvc.Helpers;
using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace kuujinbo.ASP.NET.Mvc.Services.Json
{
    public class JsonNetSerializer
    {
        public const string DEFAULT_DATE_FORMAT = "yyyy-MM-dd";

        private JsonSerializerSettings _jsonSettings;
        private IsoDateTimeConverter _isoDateTimeConverter;
        
        private static readonly string _appDateFormat;
        private static readonly string _boolTrue, _boolFalse;
        static JsonNetSerializer()
        {
            var appSettings = new AppSettingsReader();
            _appDateFormat = appSettings.DateFormat;
            _boolTrue = appSettings.BoolTrue;
            _boolFalse = appSettings.BoolFalse;

            if (string.IsNullOrWhiteSpace(_appDateFormat))
            {
                _appDateFormat = DEFAULT_DATE_FORMAT;
                return;
            }

            DateFormatValidator.Parse(_appDateFormat);
        }


        public JsonNetSerializer()
        {
            _isoDateTimeConverter = new IsoDateTimeConverter()
            {
                DateTimeFormat = _appDateFormat
            };

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
                _jsonSettings.Converters.Add(
                    new WriteBoolConverter() { True = _boolTrue, False = _boolFalse }
                );
            }

            return JsonConvert.SerializeObject(
                value, Formatting.Indented, _jsonSettings
            );
        }
    }
}