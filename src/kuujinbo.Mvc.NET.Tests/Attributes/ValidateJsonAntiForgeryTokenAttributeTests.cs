using kuujinbo.Mvc.NET.Attributes;
using Moq;
using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Attributes
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class ValidateJsonAntiForgeryTokenAttributeTests
    {
        const string BAD_VALUE = "bad";
        ValidateJsonAntiForgeryTokenAttribute _attribute;
        AuthorizationContext _authorizationContext;
        Mock<HttpContextBase> _context;
        Mock<HttpRequestBase> _request;
        Mock<ControllerBase> _controller;
        HttpCookieCollection _cookies;
        NameValueCollection _headers;

        public ValidateJsonAntiForgeryTokenAttributeTests()
        {
            _context = new Mock<HttpContextBase>();
            _controller = new Mock<ControllerBase>();
            _cookies = new HttpCookieCollection();
            _headers = new NameValueCollection();
            var requestContext = new RequestContext(_context.Object, new RouteData());
            _request = new Mock<HttpRequestBase>();
            _request.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            _request.Setup(x => x.Cookies).Returns(_cookies);
            _request.Setup(x => x.Headers).Returns(_headers);
            _context.Setup(x => x.Request).Returns(_request.Object);
            var actionDescriptor = new Mock<ActionDescriptor>();
            var controllerContext = new ControllerContext(_context.Object, new RouteData(), _controller.Object);
            _authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor.Object);

            _attribute = new ValidateJsonAntiForgeryTokenAttribute();

            // required by AntiForgery.Validate() call
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "https://invalid.test", ""),
                new HttpResponse(new StringWriter())
            );
        }

        [Fact]
        public void OnAuthorization_NoFilterContext_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => _attribute.OnAuthorization(null)
             );
        }

        [Fact]
        public void OnAuthorization_WithoutCookie_Throws()
        {
            Assert.Equal(0, _request.Object.Cookies.Count);
            Assert.Equal(0, _request.Object.Headers.Count);
            var exception = Assert.Throws<HttpAntiForgeryException>(
                 () => _attribute.OnAuthorization(_authorizationContext)
             );
        }

        [Fact]
        public void OnAuthorization_WithoutHeader_Throws()
        {
            _cookies.Add(new HttpCookie(AntiForgeryConfig.CookieName) { Value = BAD_VALUE });

            Assert.Equal(
                BAD_VALUE,
                _request.Object.Cookies[AntiForgeryConfig.CookieName].Value
            );
            var exception = Assert.Throws<HttpAntiForgeryException>(
                 () => _attribute.OnAuthorization(_authorizationContext)
             );
        }

        [Fact]
        public void OnAuthorization_WithInvalidCookieAndToken_Throws()
        {
            _cookies.Add(new HttpCookie(AntiForgeryConfig.CookieName) { Value = BAD_VALUE });
            _headers[AntiForgeryConfig.CookieName] = BAD_VALUE;

            Assert.Equal(
                BAD_VALUE, 
                _request.Object.Cookies[AntiForgeryConfig.CookieName].Value
            );
            Assert.Equal(
                BAD_VALUE, 
                _request.Object.Headers[AntiForgeryConfig.CookieName]
            );
            var exception = Assert.Throws<HttpAntiForgeryException>(
                 () => _attribute.OnAuthorization(_authorizationContext)
             );
        }

        [Fact]
        public void OnAuthorization_WithValidCookieAndToken_PassesValidation()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            _cookies.Add(new HttpCookie(AntiForgeryConfig.CookieName) { Value = cookieToken });
            _headers[AntiForgeryConfig.CookieName] = formToken;

            _attribute.OnAuthorization(_authorizationContext);

            Assert.Null(_authorizationContext.Result);
        }
    }
}