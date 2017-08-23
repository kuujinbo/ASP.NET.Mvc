using kuujinbo.ASP.NET.Mvc.Helpers;
using System.Xml.Linq;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Helpers
{
    class TestWebConfig : IWebConfig
    {
        private string _xml;
        public TestWebConfig(string xml)
        {
            _xml = xml;
        }
        public XDocument GetSection()
        {
            return XDocument.Parse(_xml);
        }
    }

    public class WebConfigurationManagerHelperTests
    {
        IWebConfig _webConfig;

        const int MAX_CONTENTLENGTH_LESS_THAN = 1;
        static readonly int MAX_CONTENTLENGTH_GREATER = WebConfigurationManagerHelper.DEFAULT_MAX_REQUEST_LENGTH + 1;
        static readonly string XML_FORMAT = @"
<system.webServer>
    <security>
        <requestFiltering>
            <requestLimits maxAllowedContentLength='{0}' />
        </requestFiltering>
    </security>
</system.webServer>";

        public WebConfigurationManagerHelperTests()
        {
            _webConfig = null;
            WebConfigurationManagerHelper._maxRequestLength = 0;
            WebConfigurationManagerHelper._maxAllowedContentLength = 0;
            WebConfigurationManagerHelper._maxUploadSize = 0;
        }
 
        #region GetMaxRequestLength
        [Fact]
        public void GetMaxRequestLength_ValueSet_ReturnsValue()
        {
            var setValue = 1;
            WebConfigurationManagerHelper._maxRequestLength = setValue;

            Assert.Equal(setValue, WebConfigurationManagerHelper.GetMaxRequestLength());
        }

        [Fact]
        public void GetMaxRequestLength_NotSet_GetsValue()
        {
            WebConfigurationManagerHelper._maxRequestLength = 0;

            Assert.Equal(
                WebConfigurationManagerHelper.DEFAULT_MAX_REQUEST_LENGTH,
                WebConfigurationManagerHelper.GetMaxRequestLength()
            );
        }
        #endregion

        #region GetMaxAllowedContentLength
        [Fact]
        public void GetMaxAllowedContentLength_ValueSet_ReturnsValue()
        {
            var setValue = 10;
            WebConfigurationManagerHelper._maxAllowedContentLength = setValue;

            Assert.Equal(
                setValue,
                WebConfigurationManagerHelper.GetMaxAllowedContentLength(_webConfig)
            );
        }

        [Fact]
        public void GetMaxAllowedContentLength_InvalidPath_ReturnsDefault()
        {
            WebConfigurationManagerHelper._maxAllowedContentLength = 0;

            Assert.Equal(
                WebConfigurationManagerHelper.DEFAULT_MAX_ALLOWED_CONTENTLENGTH,
                WebConfigurationManagerHelper.GetMaxAllowedContentLength(_webConfig)
            );
        }
        #endregion

        #region GetMaxUploadSize
        [Fact]
        public void GetMaxUploadSize_ValueSet_ReturnsValue()
        {
            var maxUploadSize = 20;
            WebConfigurationManagerHelper._maxUploadSize = maxUploadSize;

            Assert.Equal(maxUploadSize, WebConfigurationManagerHelper.GetMaxUploadSize(_webConfig));
        }

        [Fact]
        public void GetMaxUploadSize_MaxRequestLengthLessThan_ReturnsMaxRequestLength()
        {
            WebConfigurationManagerHelper._maxAllowedContentLength = MAX_CONTENTLENGTH_GREATER;
            _webConfig = new TestWebConfig(string.Format(XML_FORMAT, MAX_CONTENTLENGTH_LESS_THAN));

            Assert.Equal(
                WebConfigurationManagerHelper.DEFAULT_MAX_REQUEST_LENGTH,
                WebConfigurationManagerHelper.GetMaxUploadSize(_webConfig)
            );
        }

        [Fact]
        public void GetMaxUploadSize_MaxRequestLengthNotLessThan_ReturnsMaxContentLength()
        {
            WebConfigurationManagerHelper._maxRequestLength = WebConfigurationManagerHelper.DEFAULT_MAX_REQUEST_LENGTH;
            var xml = string.Format(XML_FORMAT, MAX_CONTENTLENGTH_LESS_THAN);
            _webConfig = new TestWebConfig(xml);

            var result = WebConfigurationManagerHelper.GetMaxUploadSize(_webConfig);

            Assert.Equal(MAX_CONTENTLENGTH_LESS_THAN, result);
        }

        [Fact]
        public void GetMaxUploadSize_InvalidXml_ReturnsMaxRequestLength()
        {
            _webConfig = new TestWebConfig("<empty></empty>");

            var result = WebConfigurationManagerHelper.GetMaxUploadSize(_webConfig);

            Assert.Equal(
                WebConfigurationManagerHelper.DEFAULT_MAX_REQUEST_LENGTH,
                result
            );
        }
        #endregion
    }
}