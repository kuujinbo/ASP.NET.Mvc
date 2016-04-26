using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Xunit;
using Moq;
using System.Web.Routing;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests
{
    public class FakeController : Controller
    {
        public ActionResult JsonData(object obj)
        {
            return new JsonNetResult(obj);
        }
    }

    public class JsonNetResultTests
    {
        public static readonly Dictionary<string, string> DATA = 
            new Dictionary<string, string>() { { "one", "1" } };

        private FakeController _fakeController;
        public JsonNetResultTests()
        {
            _fakeController = new FakeController();;

        }

        [Fact]
        public void ExecuteResult_WithNullData_ThrowsArgumentNullException()
        {
            _fakeController.SetFakeControllerContext();

            var result = _fakeController.JsonData(null);
            var exception = Assert.Throws<ArgumentNullException>(
                () => result.ExecuteResult(_fakeController.ControllerContext)
            );

            Assert.Equal<string>("Data", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WithNullContext_ThrowsArgumentNullException()
        {
            var result = _fakeController.JsonData(DATA);
            var exception = Assert.Throws<ArgumentNullException>(
                () => result.ExecuteResult(_fakeController.ControllerContext)
            );
            Assert.Equal<string>("context", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WithData_ReturnsCorrectTypeAndHeaders()
        {
            _fakeController.SetFakeControllerContext();

            var result = _fakeController.JsonData(DATA);
            result.ExecuteResult(_fakeController.ControllerContext);

            Assert.Equal("application/json",  _fakeController.Response.ContentType);
            Assert.IsType<JsonNetResult>(result);
        }

        [Fact]
        public void ExecuteResult_WithData_WritesJsonString()
        {
            // arrange
            var sb = new StringBuilder();
            var fakeContext = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> response = new Mock<HttpResponseBase>();
            response.Setup(
                x => x.Write(It.IsAny<string>()))
                    .Callback<string>(y => { sb.Append(y); }
            );
            fakeContext.Setup(ctx => ctx.Response).Returns(response.Object);
            ControllerContext context = new ControllerContext(
                new RequestContext(fakeContext.Object, new RouteData()),
                _fakeController
            );
            _fakeController.ControllerContext = context;

            // act
            _fakeController.JsonData(DATA).ExecuteResult(_fakeController.ControllerContext);

            Assert.True(sb.Length > 0);
            Assert.Contains("\"one\": \"1\"", sb.ToString());
        }

    }
}
