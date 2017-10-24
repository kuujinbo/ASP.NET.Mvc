using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Helpers;
using kuujinbo.Mvc.NET.HtmlHelpers;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.HtmlHelpers
{
    public class FileUploadFieldHelperTests
    {
        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;
        int _defaultUploadSize, _defaultUploadInMB;

        public FileUploadFieldHelperTests()
        {
            _defaultUploadSize = WebConfigurationManagerHelper.DefaultMaxRequestLength;
            _defaultUploadInMB = _defaultUploadSize / 1024;

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
                                FileUploadFieldHelper.HtmlFormat
                                , _defaultUploadSize 
                                , string.Empty
                                , FileUploadFieldHelper.DefaultButtonText
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
                                FileUploadFieldHelper.HtmlFormat
                                , _defaultUploadSize 
                                , string.Empty
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
                                FileUploadFieldHelper.HtmlFormat
                                , _defaultUploadSize 
                                , string.Format("accept='{0}'", string.Join(",", acceptExtensions))
                                , FileUploadFieldHelper.DefaultButtonText
                                , _defaultUploadInMB
                                , string.Format(
                                    FileUploadFieldHelper.AcceptFormat, 
                                    string.Join(",", acceptExtensions)
                                )
                             );

            Assert.Equal(expected, result.ToString());
        }
    }
}