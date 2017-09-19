using kuujinbo.ASP.NET.Mvc.Attributes;
using Moq;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Attributes
{
    public class DodBannerAuthorizeAttributeTests
    {
        DodBannerAuthorizeAttribute _attribute;
        AuthorizationContext _authorizationContext;
        Mock<HttpContextBase> _context;
        Mock<HttpRequestBase> _request;
        Mock<HttpResponseBase> _response;
        Mock<ControllerBase> _controller;
        HttpCookieCollection _cookies;

        public DodBannerAuthorizeAttributeTests()
        {
            _context = new Mock<HttpContextBase>();
            _controller = new Mock<ControllerBase>();
            var requestContext = new RequestContext(_context.Object, new RouteData());
            _request = new Mock<HttpRequestBase>();
            _request.Setup(x => x.Url).Returns(new Uri("http://invalid.test"));
            _request.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            _cookies = new HttpCookieCollection();
            _response = new Mock<HttpResponseBase>();
            _response.Setup(x => x.Cookies).Returns(_cookies);
            _response.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).Callback<HttpCookie>(x => _cookies.Add(x));
            _context.Setup(x => x.Request).Returns(_request.Object);
            _context.Setup(x => x.Response).Returns(_response.Object);
            var actionDescriptor = new Mock<ActionDescriptor>();
            var controllerContext = new ControllerContext(_context.Object, new RouteData(), _controller.Object);
            _authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor.Object);

            _attribute = new DodBannerAuthorizeAttribute();
        }

        [Fact]
        public void OnAuthorization_NoRequestOrResponseCookie_RedirectsToApplicationEntry()
        {
            _attribute.OnAuthorization(_authorizationContext);

            var result = (RedirectToRouteResult)_authorizationContext.Result;
            Assert.Equal(
                DodBannerAuthorizeAttribute.CONTROLLER_ACTION,
                result.RouteValues["action"].ToString()
            );
            Assert.Equal(
                DodBannerAuthorizeAttribute.CONTROLLER_NAME,
                result.RouteValues["controller"].ToString()
            );
            _response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Once());
        }

        [Fact]
        public void OnAuthorization_NoRequestCookieAndSettingControllerNameAndAction_RedirectsToApplicationEntry()
        {
            var controllerName = "TestNameProperty";
            var actionName = "TestActionProperty";
            _attribute.ControllerName = controllerName;
            _attribute.ControllerActionName = actionName;

            _attribute.OnAuthorization(_authorizationContext);

            var result = (RedirectToRouteResult)_authorizationContext.Result;
            Assert.Equal(actionName, result.RouteValues["action"].ToString());
            Assert.Equal(controllerName, result.RouteValues["controller"].ToString());
            _response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Once());
        }

        [Fact]
        public void OnAuthorization_NoRequestCookieResponseReturnUrlCookie_RedirectsToApplicationEntry()
        {
            _cookies.Add(new HttpCookie(CookieFactory.RETURN_URL)
            {
                Value = "http://invalid.test"
            });

            _attribute.OnAuthorization(_authorizationContext);

            var result = (RedirectToRouteResult)_authorizationContext.Result;
            Assert.Equal(
                DodBannerAuthorizeAttribute.CONTROLLER_ACTION,
                result.RouteValues["action"].ToString()
            );
            Assert.Equal(
                DodBannerAuthorizeAttribute.CONTROLLER_NAME,
                result.RouteValues["controller"].ToString()
            );
            _response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Never());
        }

        [Fact]
        public void OnAuthorization_HasRequestCookie_IsNoOp()
        {
            var cookieCollection = new HttpCookieCollection();
            cookieCollection.Add(new HttpCookie(CookieFactory.DOD_NOTICE_CONSENT)
            {
                Value = DateTime.Now.ToString()
            });
            _request.Setup(x => x.Cookies).Returns(cookieCollection);

            _attribute.OnAuthorization(_authorizationContext);

            _response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Never());
            Assert.Null(_authorizationContext.Result);
        }
    }
}