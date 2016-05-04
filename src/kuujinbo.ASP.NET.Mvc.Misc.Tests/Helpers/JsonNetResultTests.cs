using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using Xunit;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Helpers
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
            _fakeController = new FakeController();
        }

        [Fact]
        public void ExecuteResult_WithNullData_ThrowsArgumentNullException()
        {
            _fakeController.SetFakeControllerContext();

            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController
                    .JsonData(null)
            );

            Assert.Equal<string>("data", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WithNullContext_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController
                    .JsonData(DATA)
                    .ExecuteResult(_fakeController.ControllerContext)
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
            var json = string.Empty;
            var fakeContext = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> response = new Mock<HttpResponseBase>();
            response.Setup(
                x => x.Write(It.IsAny<string>()))
                    .Callback<string>(y => { json = y; }
            );
            fakeContext.Setup(ctx => ctx.Response).Returns(response.Object);
            _fakeController.ControllerContext = new ControllerContext(
                new RequestContext(fakeContext.Object, new RouteData()),
                _fakeController
            );

            _fakeController
                .JsonData(DATA)
                .ExecuteResult(_fakeController.ControllerContext);

            Assert.StartsWith("{", json); 
            Assert.Equal<int>(json.Count(x => x == '"'), 4);
            Assert.Contains("one", json);
            Assert.Equal<int>(json.Count(x => x == ':'), 1);
            Assert.Contains("1", json);
            Assert.EndsWith("}", json);
        }

    }
}
