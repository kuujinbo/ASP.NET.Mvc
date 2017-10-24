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

    [ExcludeFromCodeCoverage]
    /// <summary>
    /// Internal implementation; exclusively used by FileUploadFieldExtension,
    /// since WebConfigurationManager is static....
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
        public const int DefaultMaxRequestLength = 4096;

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

        public const string SystemWebServer = "system.webServer";
        public const string XPathRequestLimits = "./security/requestFiltering/requestLimits";
        public const string MaxAllowedContentLength = "maxAllowedContentLength";
        /// <summary>
        /// IIS setting - **NOT** always in web application web.config
        /// </summary>
        public const int DefaultMaxAllowedContentLength = 30000000;

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
                var element = xDocument.Root.XPathSelectElement(XPathRequestLimits);
                _maxAllowedContentLength =
                    element != null 
                    && Int32.TryParse(element.Attribute(MaxAllowedContentLength).Value, out result)
                ? result : DefaultMaxAllowedContentLength;
            }
            catch
            {
                _maxAllowedContentLength = DefaultMaxAllowedContentLength;
            }

            return _maxAllowedContentLength;
        }
    }
}