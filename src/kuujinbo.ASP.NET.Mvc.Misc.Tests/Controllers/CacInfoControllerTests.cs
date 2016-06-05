using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Controllers;
using kuujinbo.ASP.NET.Mvc.Misc.Services;
using Xunit;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Controllers
{
    public class CacInfoControllerTests
    {
        public const string LAST_NAME = "last";
        public const string FIRST_NAME = "first";
        public const string EDIPI = "0987654321";
        public const string EMAIL = "email@domain";

        CacInfoController _controller;
        Mock<HttpRequestBase> _httpRequestBase;
        Mock<HttpContextBase> _httpContextBase;
        Mock<ICacInfo> _cacInfo;
        Mock<IClientCertificate> _clientCertificate;
        ActionResult _result;

        public CacInfoControllerTests()
        {
            _httpContextBase = new Mock<HttpContextBase>(MockBehavior.Strict);
            _httpRequestBase = new Mock<HttpRequestBase>(MockBehavior.Strict);
            _httpContextBase.Setup(x => x.Response)
                .Returns(new Mock<HttpResponseBase>().Object);
            _httpContextBase.Setup(x => x.Request)
                .Returns(_httpRequestBase.Object);

            _cacInfo = new Mock<ICacInfo>();
            _clientCertificate = new Mock<IClientCertificate>();
            _clientCertificate.Setup(x => x.Get(_httpRequestBase.Object))
                .Returns(It.IsAny<byte[]>());
            _controller = new CacInfoController(
                _cacInfo.Object,
                _clientCertificate.Object
            );
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = _httpContextBase.Object;
        }

        [Fact]
        public void Index_CacInfoGetWithEmail_ReturnsCacInfoModel()
        {
            _cacInfo.Setup(x => x.Get(It.IsAny<byte[]>()))
                .Returns(new CacInfo()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI,
                    Email = EMAIL
                });

            _result = _controller.Index();
            var model = (CacInfo) ((ViewResult)_result).Model;

            _cacInfo.Verify(x => x.Get(It.IsAny<byte[]>()), Times.Once());
            Assert.IsType<ViewResult>(_result);
            Assert.Equal(LAST_NAME, model.LastName);
            Assert.Equal(FIRST_NAME, model.FirstName);
            Assert.Equal(EDIPI, model.Edipi);
            Assert.Equal(EMAIL, model.Email);
        }

        [Fact]
        public void Index_CacInfoGetWithoutEmail_ReturnsNullCacInfoModel()
        {
            _cacInfo.Setup(x => x.Get(It.IsAny<byte[]>()))
                .Returns(new CacInfo()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI
                });

            _result = _controller.Index();
            var model = (CacInfo) ((ViewResult)_result).Model;

            _cacInfo.Verify(x => x.Get(It.IsAny<byte[]>()), Times.Once());
            Assert.IsType<ViewResult>(_result);
            Assert.Null(model);
        }
    }
}