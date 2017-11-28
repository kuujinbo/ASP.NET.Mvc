using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.HtmlHelpers;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.HtmlHelpers
{
    public class JQueryConfirmHelperTests
    {

        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;

        public JQueryConfirmHelperTests()
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Items).Returns(new Dictionary<string, object>());

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _viewData = new Mock<IViewDataContainer>();
            _viewData.Setup(x => x.ViewData).Returns(new ViewDataDictionary());

            _helper = new HtmlHelper(viewContext.Object, _viewData.Object);
        }

        [Fact]
        public void JQueryConfirm_WhenCalled_ReturnsStringEmpty()
        {
            var result = _helper.JQueryConfirm();

            Assert.Equal(string.Empty, result.ToString());
        }
    }
}
