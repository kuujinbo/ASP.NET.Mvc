using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace kuujinbo.ASP.NET.Mvc.Json
{
    public class JsonNetSerializer
    {
        public const string AppDateFormat = "M/d/yyyy";
        public const string BAD_DATE_FORMAT = "unrecognized date format";

        JsonSerializerSettings _jsonSettings;
        IsoDateTimeConverter _isoDateTimeConverter;

        public JsonNetSerializer()
        {
            _jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            _isoDateTimeConverter = new IsoDateTimeConverter() 
            { 
                DateTimeFormat = AppDateFormat
            };
        }

        public string GetDataTable(object value, string dateFormat = null)
        {
            if (value == null) throw new System.ArgumentNullException("value");

            setDateFormat(dateFormat);
            _jsonSettings.Converters.Add(_isoDateTimeConverter);
            _jsonSettings.Converters.Add(new SimpleEnumConverter());
            _jsonSettings.Converters.Add(new BoolYesNoConverter());

            return JsonConvert.SerializeObject(
                value, Formatting.Indented, _jsonSettings
            );
        }

        public string Get(object value, string dateFormat = null)
        {
            if (value == null) throw new System.ArgumentNullException("value");

            setDateFormat(dateFormat);

            return JsonConvert.SerializeObject(
                value, Formatting.Indented, _jsonSettings
            );

        }

        private void setDateFormat(string dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateFormat) || dateFormat == AppDateFormat)
            {
                return;
            }

            DateTime outDate;
            if (DateTime.TryParse(DateTime.Now.ToString(dateFormat), out outDate))
            {
                _isoDateTimeConverter.DateTimeFormat = dateFormat;
            }

            throw new FormatException(BAD_DATE_FORMAT);
        }
    }
}