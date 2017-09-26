using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.HtmlHelpers;
using Moq;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.HtmlHelpers
{
    public class SessionTerminatorHelperTests
    {
        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;
        Mock<ControllerBase> _controller;
        const string _testUrl = "/controllerName/actionName";

        public SessionTerminatorHelperTests()
        {
            _controller = new Mock<ControllerBase>();
            _controller.Object.TempData = new TempDataDictionary();
            _controller.Object.TempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] = true;

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Items).Returns(new Dictionary<string, object>());

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.Controller).Returns(_controller.Object);
            viewContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _viewData = new Mock<IViewDataContainer>();
            _viewData.Setup(x => x.ViewData).Returns(new ViewDataDictionary());

            _helper = new HtmlHelper(viewContext.Object, _viewData.Object);
        }

        [Fact]
        public void TerminateSession_IgnoreTimeout_ReturnsEmptyString()
        {
            var result = _helper.TerminateSession(SessionTerminator.PrivilegedTimeout, "");

            Assert.Equal(string.Empty, result.ToString());
        }

        [Fact]
        public void TerminateSession_SessionTimedOut_ReturnsEmptyString()
        {
            _controller.Object.TempData[SessionTerminator.SESSION_TIMED_OUT] = true;

            var result = _helper.TerminateSession(SessionTerminator.PrivilegedTimeout, _testUrl);

            Assert.Equal(string.Empty, result.ToString());
        }

        [Fact]
        public void TerminateSession_Init_ReturnsEmptyString()
        {
            _controller.Object.TempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] = null;

            var result = _helper.TerminateSession(SessionTerminator.PrivilegedTimeout, _testUrl);

            Assert.Equal(string.Empty, result.ToString());
        }
    }
}