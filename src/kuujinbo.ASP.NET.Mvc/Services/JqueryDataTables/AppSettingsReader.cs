/* ============================================================================
 * get appSettings values specifically used by JqueryDataTables 
 * ============================================================================
 */
using System.Collections.Specialized;
using System.Configuration;

namespace kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables
{
    public class AppSettingsReader
    {
        public const string BOOL_TRUE = "BoolTrue";
        public const string BOOL_FALSE = "BoolFalse";
        public const string DATE_FORMAT = "DateFormat";

        public static readonly string[] KEYS =
        {
            BOOL_TRUE, BOOL_FALSE, DATE_FORMAT
        };

        private static readonly NameValueCollection _settings;

        static AppSettingsReader()
        {
            _settings = new NameValueCollection();

            var settings = ConfigurationManager.AppSettings;
            if (settings.Count > 0)
            {
                foreach (var key in KEYS)
                {
                    var val = !string.IsNullOrWhiteSpace(settings[key])
                        ? settings[key] : null;
                    _settings.Add(key, val);
                }
            }
        }

        public string BoolTrue 
        {
            get { return _settings[BOOL_TRUE]; }
        }

        public string BoolFalse
        {
            get { return _settings[BOOL_FALSE]; }
        }

        public string DateFormat
        {
            get { return _settings[DATE_FORMAT]; }
        }
    }
}