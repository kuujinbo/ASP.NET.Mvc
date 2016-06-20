/* ============================================================================
 * get appSettings values specifically used by JqueryDataTables 
 * ============================================================================
 */
using System.Configuration;

namespace kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables
{
    public sealed class DisplaySettings
    {
        // singleton
        public static readonly DisplaySettings Settings;

        // deault ISO format
        public const string DEFAULT_DATE_FORMAT = "yyyy-MM-dd";
        // default replacement for true
        public const string DEFAULT_TRUE = "Yes";
        // default replacement for false
        public const string DEFAULT_FALSE = "No";

        // appSettings keys
        public const string BOOL_TRUE = "BoolTrue";
        public const string BOOL_FALSE = "BoolFalse";
        public const string DATE_FORMAT = "DateFormat";

        static DisplaySettings()
        {
            Settings = new DisplaySettings();

            var settings = ConfigurationManager.AppSettings;
            if (settings.Count > 0)
            {
                Settings.BoolTrue = !string.IsNullOrWhiteSpace(settings[BOOL_TRUE]) 
                    ? settings[BOOL_TRUE] : DEFAULT_TRUE;
                Settings.BoolFalse = !string.IsNullOrWhiteSpace(settings[BOOL_FALSE])
                    ? settings[BOOL_FALSE] : DEFAULT_FALSE;
                Settings.DateFormat = !string.IsNullOrWhiteSpace(settings[DATE_FORMAT])
                    ? settings[DATE_FORMAT] : DEFAULT_DATE_FORMAT;                     
            }
            else
            {
                Settings.BoolTrue = DEFAULT_TRUE;
                Settings.BoolFalse = DEFAULT_FALSE;
                Settings.DateFormat = DEFAULT_DATE_FORMAT;
            }
        }

        public string BoolTrue { get; private set; }
        public string BoolFalse { get; private set; }
        public string DateFormat { get; private set; }
    }
}