using kuujinbo.Mvc.NET.Examples.Controllers;
using Moq;
using System.Web;
using System.Web.Mvc;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Controllers
{
    public class DodCacControllerTests
    {
        public const string LAST_NAME = "last";
        public const string FIRST_NAME = "first";
        public const string EDIPI = "0987654321";
        public const string EMAIL = "email@domain";

        DodCacController _controller;
        Mock<HttpRequestBase> _httpRequestBase;
        Mock<HttpContextBase> _httpContextBase;
        Mock<IDodCac> _dodCac;
        Mock<IClientCertificate> _clientCertificate;
        ActionResult _result;

        public DodCacControllerTests()
        {
            _httpContextBase = new Mock<HttpContextBase>();
            _httpRequestBase = new Mock<HttpRequestBase>();
            _httpContextBase.Setup(x => x.Response)
                .Returns(new Mock<HttpResponseBase>().Object);
            _httpContextBase.Setup(x => x.Request)
                .Returns(_httpRequestBase.Object);

            _dodCac = new Mock<IDodCac>();
            _clientCertificate = new Mock<IClientCertificate>();
            _clientCertificate.Setup(x => x.Get(_httpRequestBase.Object))
                .Returns(It.IsAny<byte[]>());
            _controller = new DodCacController(
                _dodCac.Object,
                _clientCertificate.Object
            );
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = _httpContextBase.Object;
        }

        [Fact]
        public void Index_CacInfoGetWithEmail_ReturnsCacInfoModel()
        {
            _dodCac.Setup(x => x.Get(It.IsAny<byte[]>()))
                .Returns(new DodCac()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI,
                    Email = EMAIL
                });

            _result = _controller.Index();
            var model = (DodCac) ((ViewResult)_result).Model;

            _dodCac.Verify(x => x.Get(It.IsAny<byte[]>()), Times.Once());
            Assert.IsType<ViewResult>(_result);
            Assert.Equal(LAST_NAME, model.LastName);
            Assert.Equal(FIRST_NAME, model.FirstName);
            Assert.Equal(EDIPI, model.Edipi);
            Assert.Equal(EMAIL, model.Email);
        }

        [Fact]
        public void Index_CacInfoGetWithoutEmail_ReturnsNullCacInfoModel()
        {
            _dodCac.Setup(x => x.Get(It.IsAny<byte[]>()))
                .Returns(new DodCac()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI
                });

            _result = _controller.Index();
            var model = (DodCac) ((ViewResult)_result).Model;

            _dodCac.Verify(x => x.Get(It.IsAny<byte[]>()), Times.Once());
            Assert.IsType<ViewResult>(_result);
            Assert.Null(model);
        }
    }
}