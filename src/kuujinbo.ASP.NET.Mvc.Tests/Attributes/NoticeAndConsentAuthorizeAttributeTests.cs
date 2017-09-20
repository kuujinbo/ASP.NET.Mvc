using kuujinbo.ASP.NET.Mvc.Attributes;
using Moq;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Attributes
{
    public class NoticeAndConsentAuthorizeAttributeTests
    {
        const string CONTROLLER_NAME = "NoticeAndConsent";
        const string ACTION_NAME = "Index";

        NoticeAndConsentAuthorizeAttribute _attribute;
        AuthorizationContext _authorizationContext;
        Mock<HttpContextBase> _context;
        Mock<HttpRequestBase> _request;
        Mock<HttpResponseBase> _response;
        Mock<ControllerBase> _controller;
        HttpCookieCollection _cookies;

        public NoticeAndConsentAuthorizeAttributeTests()
        {
            _context = new Mock<HttpContextBase>();
            _controller = new Mock<ControllerBase>();
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

            _attribute = new NoticeAndConsentAuthorizeAttribute(CONTROLLER_NAME, ACTION_NAME);
        }

        [Fact]
        public void OnAuthorization_NoRequestOrResponseCookie_RedirectsToApplicationEntry()
        {
            _attribute.OnAuthorization(_authorizationContext);

            var result = (RedirectToRouteResult)_authorizationContext.Result;
            Assert.Equal(ACTION_NAME, result.RouteValues["action"].ToString());
            Assert.Equal(CONTROLLER_NAME, result.RouteValues["controller"].ToString());
            _response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Once());
        }

        [Fact]
        public void OnAuthorization_NoRequestCookieAndSettingControllerNameAndAction_RedirectsToApplicationEntry()
        {
            _attribute.OnAuthorization(_authorizationContext);

            var result = (RedirectToRouteResult)_authorizationContext.Result;
            Assert.Equal(ACTION_NAME, result.RouteValues["action"].ToString());
            Assert.Equal(CONTROLLER_NAME, result.RouteValues["controller"].ToString());
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
            Assert.Equal(ACTION_NAME, result.RouteValues["action"].ToString());
            Assert.Equal(CONTROLLER_NAME, result.RouteValues["controller"].ToString());
            _response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Never());
        }

        [Fact]
        public void OnAuthorization_HasRequestCookie_IsNoOp()
        {
            var cookieCollection = new HttpCookieCollection();
            cookieCollection.Add(new HttpCookie(CookieFactory.NOTICE_AND_CONSENT)
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