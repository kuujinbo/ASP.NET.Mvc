using kuujinbo.Mvc.NET.Helpers;
using kuujinbo.Mvc.NET.HtmlHelpers;
using Moq;
using System.Web;
using System.Web.Mvc;
using Xunit;
using System.Collections.Generic;

namespace kuujinbo.Mvc.NET.Tests.HtmlHelpers
{
    public class FileUploadFieldHelperTests
    {
        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;
        static readonly int _defaultUploadSize = WebConfigurationManagerHelper.DEFAULT_MAX_REQUEST_LENGTH;
        static readonly int _defaultUploadInMB = _defaultUploadSize / 1024;

        public FileUploadFieldHelperTests()
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.ApplicationPath).Returns("/");
            
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Items).Returns(new Dictionary<string, object>());
            httpContext.Setup(x => x.Request).Returns(request.Object);

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _viewData = new Mock<IViewDataContainer>();
            _viewData.Setup(x => x.ViewData).Returns(new ViewDataDictionary());

            _helper = new HtmlHelper(viewContext.Object, _viewData.Object);
        }

        [Fact]
        public void FileUploadField_DefaultParam_ReturnsHtml()
        {
            var result = _helper.FileUploadField();
            var expected = string.Format(
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
            var expected = string.Format(
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
        public void FileUploadField_AcceptExtensions_ReturnsHtml()
        {
            var acceptExtensions = new string[] { ".pdf", ".jpg", ".png" };
            var result = _helper.FileUploadField(accept: acceptExtensions);
            var expected = string.Format(
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