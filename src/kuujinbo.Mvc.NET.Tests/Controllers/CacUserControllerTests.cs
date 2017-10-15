using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Examples.Controllers;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Controllers
{
    public class CacUserControllerTests
    {
        public const string LAST_NAME = "last";
        public const string FIRST_NAME = "first";
        public const string EDIPI = "0987654321";
        public const string EMAIL = "email@domain";

        CacUserController _controller;
        Mock<HttpRequestBase> _httpRequestBase;
        Mock<HttpContextBase> _httpContextBase;
        Mock<IClientCertificate> _clientCertificate;
        ActionResult _result;

        public CacUserControllerTests()
        {
            _httpContextBase = new Mock<HttpContextBase>();
            _httpRequestBase = new Mock<HttpRequestBase>();
            _httpContextBase.Setup(x => x.Response)
                .Returns(new Mock<HttpResponseBase>().Object);
            _httpContextBase.Setup(x => x.Request)
                .Returns(_httpRequestBase.Object);

            _clientCertificate = new Mock<IClientCertificate>();
            _controller = new CacUserController(
                _clientCertificate.Object
            );
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = _httpContextBase.Object;
        }

        [Fact]
        public void Index_CacInfoGetWithEmail_ReturnsCacInfoModel()
        {
            _clientCertificate.Setup(x => x.GetCacUser())
                .Returns(new CacUser()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI,
                    Email = EMAIL
                });

            _result = _controller.Index();
            var model = (CacUser) ((ViewResult)_result).Model;

            Assert.IsType<ViewResult>(_result);
            Assert.Equal(LAST_NAME, model.LastName);
            Assert.Equal(FIRST_NAME, model.FirstName);
            Assert.Equal(EDIPI, model.Edipi);
            Assert.Equal(EMAIL, model.Email);
        }

        [Fact]
        public void Index_CacInfoGetWithoutEmail_ReturnsNullCacInfoModel()
        {
            _clientCertificate.Setup(x => x.GetCacUser())
                .Returns(new CacUser()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI
                });

            _result = _controller.Index();
            var model = (CacUser) ((ViewResult)_result).Model;

            Assert.IsType<ViewResult>(_result);
            Assert.Null(model);
        }
    }
}