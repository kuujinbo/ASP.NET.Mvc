using System;
using System.Web.Configuration;
using System.Xml.XPath;
using System.Xml.Linq;

namespace kuujinbo.ASP.NET.Mvc.Helpers
{
    public static class WebConfigurationManagerHelper
    {
        private static int _maxRequestLength;
        private static int _maxAllowedContentLength;
        private static int _maxUploadSize;

        /// <summary>
        /// Get web application maximum allowed upload size.
        /// </summary>
        public static int GetMaxUploadSize(string path)
        {
            if (_maxUploadSize > 0) return _maxUploadSize;

            _maxUploadSize = GetMaxRequestLength() < GetMaxAllowedContentLength(path)
                ? _maxRequestLength : _maxAllowedContentLength;
            return _maxUploadSize;
        }

        public static int GetMaxRequestLength()
        {
            if (_maxRequestLength > 0) return _maxRequestLength;

            _maxRequestLength = ((HttpRuntimeSection) WebConfigurationManager.GetSection("system.web/httpRuntime")).MaxRequestLength;
            return _maxRequestLength;
        }


        public const string SYSTEM_WEBSERVER = "system.webServer";
        public const string XPATH_REQUESTLIMITS = "./security/requestFiltering/requestLimits";
        public const string MAX_ALLOWED_CONTENTLENGTH_KEY_NAME = "maxAllowedContentLength";
        /// <summary>
        /// IIS setting - **NOT** always in web application web.config
        /// </summary>
        public const int DEFAULT_MAX_ALLOWED_CONTENTLENGTH = 30000000;

        public static int GetMaxAllowedContentLength(string path)
        {
            if (_maxAllowedContentLength > 0) return _maxAllowedContentLength;

            try
            {
                var config = WebConfigurationManager.OpenWebConfiguration(path);
                var section = config.GetSection(SYSTEM_WEBSERVER);
                var xml = section.SectionInformation.GetRawXml();
                var doc = XDocument.Parse(xml);

                int result;
                var element = doc.Root.XPathSelectElement(XPATH_REQUESTLIMITS);
                if (element != null 
                    && Int32.TryParse(
                        element.Attribute(MAX_ALLOWED_CONTENTLENGTH_KEY_NAME).Value, out result)
                    )
                {
                    _maxAllowedContentLength = result;
                }
            }
            catch
            {
                _maxAllowedContentLength = DEFAULT_MAX_ALLOWED_CONTENTLENGTH;
            }

            return _maxAllowedContentLength;
        }
    }
}