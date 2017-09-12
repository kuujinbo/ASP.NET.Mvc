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
                                FileUploadFieldHelper.HTML_FORMAT
                                , _defaultUploadSize 
                                , FileUploadFieldHelper.ACCEPT_ALL
                                , FileUploadFieldHelper.DEFAULT_BUTTON_TEXT
                                , _defaultUploadInMB
                                , string.Empty
                             );

            Assert.Equal(expected, result.ToString());
        }

        [Fact]
        public void FileUploadField_ButtonText_ReturnsHtml()
        {
            var buttonText = "Select File";
            var result = _helper.FileUploadField(buttonText);
            var expected = FileUploadFieldHelper.JavaScriptBlock
                           + string.Format(
                                FileUploadFieldHelper.HTML_FORMAT
                                , _defaultUploadSize 
                                , FileUploadFieldHelper.ACCEPT_ALL
                                , buttonText
                                , _defaultUploadInMB
                                , string.Empty
                             );

            Assert.Equal(expected, result.ToString());
        }

        [Fact]
        public void FileUploadField_AccetExtensions_ReturnsHtml()
        {
            var acceptExtensions = new string[] { ".pdf", ".jpg", ".png" };
            var result = _helper.FileUploadField(accept: acceptExtensions);
            var expected = FileUploadFieldHelper.JavaScriptBlock
                           + string.Format(
                                FileUploadFieldHelper.HTML_FORMAT
                                , _defaultUploadSize 
                                , string.Join(",", acceptExtensions)
                                , FileUploadFieldHelper.DEFAULT_BUTTON_TEXT
                                , _defaultUploadInMB
                                , string.Format(FileUploadFieldHelper.ACCEPT_FORMAT, string.Join(",", acceptExtensions))
                             );

            Assert.Equal(expected, result.ToString());
        }
    }
}