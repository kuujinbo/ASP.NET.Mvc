using kuujinbo.ASP.NET.Mvc.Helpers;
using kuujinbo.ASP.NET.Mvc.HtmlHelpers;
using Moq;
using System.Web;
using System.Web.Mvc;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.HtmlHelpers
{
    public class FileUploadFieldHelperTests
    {
        HtmlHelper _helper;
        static readonly int _defaultUploadSize = WebConfigurationManagerHelper.DEFAULT_MAX_REQUEST_LENGTH;
        static readonly int _defaultUploadInMB = _defaultUploadSize / 1024;

        public FileUploadFieldHelperTests()
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.ApplicationPath).Returns("/");
            
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Request).Returns(request.Object);

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _helper = new HtmlHelper(viewContext.Object, new Mock<IViewDataContainer>().Object);
        }

        [Fact]
        public void FileUploadField_DefaultParam_ReturnsHtml()
        {
            var result = _helper.FileUploadField();
            var expected = FileUploadFieldHelper.JavaScriptBlock
                           + string.Format(
                                FileUploadFieldHelper.HTML_FORMAT,
                                _defaultUploadSize, 
                                FileUploadFieldHelper.DEFAULT_BUTTON_TEXT, 
                                _defaultUploadInMB
                             );

            Assert.Equal(expected, result.ToString());
        }

        [Fact]
        public void FileUploadField_ExplicitParam_ReturnsHtml()
        {
            var buttonText = "Select File";
            var result = _helper.FileUploadField(buttonText);
            var expected = FileUploadFieldHelper.JavaScriptBlock
                           + string.Format(
                                FileUploadFieldHelper.HTML_FORMAT,
                                _defaultUploadSize,
                                buttonText,
                                _defaultUploadInMB
                             );

            Assert.Equal(expected, result.ToString());
        }
    }
}