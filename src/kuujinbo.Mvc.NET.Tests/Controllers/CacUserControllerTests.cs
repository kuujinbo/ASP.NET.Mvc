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
        Mock<HttpRequestBase> _request;
        Mock<HttpContextBase> _context;
        Mock<IClientCertificate> _clientCertificate;
        ActionResult _result;

        public CacUserControllerTests()
        {
            _context = new Mock<HttpContextBase>();
            _request = new Mock<HttpRequestBase>();
            _context.Setup(x => x.Response)
                .Returns(new Mock<HttpResponseBase>().Object);
            _context.Setup(x => x.Request)
                .Returns(_request.Object);

            _clientCertificate = new Mock<IClientCertificate>();
            _controller = new CacUserController(
                _clientCertificate.Object
            );
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = _context.Object;
        }

        [Fact]
        public void Index_CacInfoGetWithEmail_ReturnsCacInfoModel()
        {
            _clientCertificate.Setup(x => x.GetCacUser(_request.Object, It.IsAny<bool>()))
                .Returns(new CacUser()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI,
                    Email = EMAIL
                });

            _result = _controller.Index();
            var model = (CacUser) ((ViewResult)_result).Model;

            _clientCertificate.Verify(
                x => x.GetCacUser(
                    It.IsAny<HttpRequestBase>(), 
                    It.IsAny<bool>()
                ), 
                Times.Once()
            );
            Assert.IsType<ViewResult>(_result);
            Assert.Equal(LAST_NAME, model.LastName);
            Assert.Equal(FIRST_NAME, model.FirstName);
            Assert.Equal(EDIPI, model.Edipi);
            Assert.Equal(EMAIL, model.Email);
        }

        [Fact]
        public void Index_CacInfoGetWithoutEmail_ReturnsNullCacInfoModel()
        {
            _clientCertificate.Setup(x => x.GetCacUser(_request.Object, It.IsAny<bool>()))
                .Returns(new CacUser()
                {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI
                });

            _result = _controller.Index();
            var model = (CacUser) ((ViewResult)_result).Model;

            _clientCertificate.Verify(
                x => x.GetCacUser(
                    It.IsAny<HttpRequestBase>(), 
                    It.IsAny<bool>()), 
                Times.Once()
            );
            Assert.IsType<ViewResult>(_result);
            Assert.Null(model);
        }
    }
}