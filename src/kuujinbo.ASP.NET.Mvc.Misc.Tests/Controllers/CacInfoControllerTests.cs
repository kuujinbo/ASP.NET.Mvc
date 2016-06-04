using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Controllers;
using kuujinbo.ASP.NET.Mvc.Misc.Services;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Controllers
{
    public class CacInfoControllerTests
    {
        public const string LAST_NAME = "last";
        public const string FIRST_NAME = "first";
        public const string EDIPI = "0987654321";

        CacInfoController Controller;
        Mock<ICacInfo> CacInfo;
        ActionResult Result;

        public CacInfoControllerTests()
        {
            var httpContextBase = new Mock<HttpContextBase>(MockBehavior.Strict);
            var httpWorkerRequest = new Mock<HttpWorkerRequest>();
            httpWorkerRequest.Setup(x => x.GetRawUrl()).Returns("/");
            httpWorkerRequest.Setup(x => x.GetClientCertificate()).Returns(new byte[] { });
            HttpContext context = new HttpContext(httpWorkerRequest.Object);
            var httpRequestBase = new Mock<HttpRequestBase>(MockBehavior.Strict);
            httpRequestBase.Setup(r => r.ClientCertificate).Returns(context.Request.ClientCertificate);
            httpContextBase.Setup(x => x.Response)
                .Returns(new Mock<HttpResponseBase>().Object);
            httpContextBase.Setup(x => x.Request)
                .Returns(httpRequestBase.Object);

            CacInfo = new Mock<ICacInfo>();
            Controller = new CacInfoController(CacInfo.Object);
            CacInfo.Setup(x => x.Get(It.IsAny<byte[]>()))
                .Returns(new CacInfo() {
                    LastName = LAST_NAME,
                    FirstName = FIRST_NAME,
                    Edipi = EDIPI
                });
            Controller.ControllerContext = new ControllerContext();
            Controller.ControllerContext.HttpContext = httpContextBase.Object;
        }

        [Fact]
        public void Index_CallsCacInfoGet_PopulatesCacInfo()
        {
            Result = Controller.Index();
            var model = ((ViewResult)Result).Model as CacInfo;

            CacInfo.Verify(x => x.Get(It.IsAny<byte[]>()), Times.Once());
            Assert.IsType<ViewResult>(Result);
            Assert.NotNull(model);
            Assert.Equal(LAST_NAME, model.LastName);
            Assert.Equal(FIRST_NAME, model.FirstName);
            Assert.Equal(EDIPI, model.Edipi);
        }
    }
}