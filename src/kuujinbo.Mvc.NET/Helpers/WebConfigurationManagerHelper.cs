using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Xml.XPath;

[assembly: InternalsVisibleTo("kuujinbo.Mvc.NET.Tests")]
namespace kuujinbo.Mvc.NET.Helpers
{
    /// <summary>
    /// wrapper for testing 
    /// </summary>
    public interface IWebConfig
    {
        XDocument GetSection();
    }

    // M$ made WebConfigurationManager static not me....
    [ExcludeFromCodeCoverage]
    /// <summary>
    /// internal implementation; exclusively used by FileUploadFieldExtension
    /// </summary>
    internal class WebConfigHelper : IWebConfig
    {
        private string _path;
        public WebConfigHelper(string path)
        {
            _path = path;
        }
        public XDocument GetSection()
        {
            return XDocument.Parse(
                WebConfigurationManager.OpenWebConfiguration(_path)
                    .GetSection("system.webServer")
                    .SectionInformation.GetRawXml()
            );
        }
    }

    public static class WebConfigurationManagerHelper
    {
        internal static int _maxRequestLength;
        internal static int _maxAllowedContentLength;
        internal static int _maxUploadSize;


        /// <summary>
        /// IIS / system default value
        /// </summary>
        public const int DEFAULT_MAX_REQUEST_LENGTH = 4096;

        /// <summary>
        /// Get web application maximum allowed upload size.
        /// </summary>
        public static int GetMaxUploadSize(IWebConfig webConfig)
        {
            if (_maxUploadSize > 0) return _maxUploadSize;

            _maxUploadSize = GetMaxRequestLength() < GetMaxAllowedContentLength(webConfig)
                ? _maxRequestLength : _maxAllowedContentLength;
            return _maxUploadSize;
        }

        /// <summary>
        /// Get maximum allowed upload size from web.config
        /// </summary>
        public static int GetMaxRequestLength()
        {
            if (_maxRequestLength > 0) return _maxRequestLength;

            _maxRequestLength = ((HttpRuntimeSection) WebConfigurationManager
                .GetSection("system.web/httpRuntime")).MaxRequestLength;

            return _maxRequestLength;
        }

        public const string SYSTEM_WEBSERVER = "system.webServer";
        public const string XPATH_REQUESTLIMITS = "./security/requestFiltering/requestLimits";
        public const string MAX_ALLOWED_CONTENTLENGTH = "maxAllowedContentLength";
        /// <summary>
        /// IIS setting - **NOT** always in web application web.config
        /// </summary>
        public const int DEFAULT_MAX_ALLOWED_CONTENTLENGTH = 30000000;

        /// <summary>
        /// Get maximum allowed upload size from web.config
        /// </summary>
        public static int GetMaxAllowedContentLength(IWebConfig webConfig)
        {
            if (_maxAllowedContentLength > 0) return _maxAllowedContentLength;

            try
            {
                var xDocument = webConfig.GetSection();

                int result;
                var element = xDocument.Root.XPathSelectElement(XPATH_REQUESTLIMITS);
                _maxAllowedContentLength =
                    element != null 
                    && Int32.TryParse(element.Attribute(MAX_ALLOWED_CONTENTLENGTH).Value, out result)
                ? result : DEFAULT_MAX_ALLOWED_CONTENTLENGTH;
            }
            catch
            {
                _maxAllowedContentLength = DEFAULT_MAX_ALLOWED_CONTENTLENGTH;
            }

            return _maxAllowedContentLength;
        }
    }
}