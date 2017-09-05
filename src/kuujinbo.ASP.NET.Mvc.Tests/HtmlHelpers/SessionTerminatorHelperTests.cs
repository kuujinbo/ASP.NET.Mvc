using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.HtmlHelpers;
using Moq;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.HtmlHelpers
{
    public class SessionTerminatorHelperTests
    {
        HtmlHelper _helper;
        Mock<ControllerBase> _controller;
        const string _testUrl = "/controllerName/actionName";

        public SessionTerminatorHelperTests()
        {
            _controller = new Mock<ControllerBase>();
            _controller.Object.TempData = new TempDataDictionary();
            _controller.Object.TempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] = true;

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.Controller).Returns(_controller.Object);

            _helper = new HtmlHelper(viewContext.Object, new Mock<IViewDataContainer>().Object);
        }

        [Fact]
        public void SessionTerminatorJavaScript_IgnoreTimeout_ReturnsEmptyString()
        {
            var result = _helper.TerminateSession(SessionTerminator.PrivilegedTimeout, "");

            Assert.Equal(string.Empty, result.ToString());
        }

        [Fact]
        public void SessionTerminatorJavaScript_SessionTimedOut_ReturnsModalScriptBlock()
        {
            _controller.Object.TempData[SessionTerminator.SESSION_TIMED_OUT] = true;

            var result = _helper.TerminateSession(SessionTerminator.PrivilegedTimeout, _testUrl);

            Assert.Equal(
                SessionTerminatorHelper.JavaScriptBlock
                + SessionTerminatorHelper.ShowLogout, 
                result.ToString()
            );
        }

        [Fact]
        public void SessionTerminatorJavaScript_Init_ReturnsInitScriptBlock()
        {
            _controller.Object.TempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] = null;

            var result = _helper.TerminateSession(SessionTerminator.PrivilegedTimeout, _testUrl);

            Assert.Equal(
                SessionTerminatorHelper.JavaScriptBlock
                + string.Format(
                    SessionTerminatorHelper.InitFormat,
                    SessionTerminator.PrivilegedTimeout,
                    _testUrl
                ),
                result.ToString()
            );
        }

    }
}
