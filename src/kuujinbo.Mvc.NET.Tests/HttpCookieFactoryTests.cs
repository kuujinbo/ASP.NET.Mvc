using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class HttpCookieFactoryTests
    {
        const string COOKIE_KEY = "test-cookie-key";
        HttpCookie _cookie;

        void CookieKeyExists()
        {
            Assert.Equal<string>(COOKIE_KEY, _cookie.Name);
        }

        [Fact]
        public void Create_WithNullNameParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => HttpCookieFactory.Create(null)
             );

            Assert.Equal<string>(HttpCookieFactory.InvalidCreateParameter, exception.ParamName);
        }

        [Fact]
        public void Create_WithWhiteSpaceNameParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => HttpCookieFactory.Create(" ")
             );

            Assert.Equal<string>(HttpCookieFactory.InvalidCreateParameter, exception.ParamName);
        }

        [Fact]
        public void Create_DefaultHttpOnlyAndSecureParameters_AreHttpOnlyAndSecure()
        {
            _cookie = HttpCookieFactory.Create(COOKIE_KEY);

            CookieKeyExists();
            Assert.True(_cookie.HttpOnly);
            Assert.True(_cookie.Secure);
        }

        [Fact]
        public void Create_WithoutValueParameter_CookieValueIsNull()
        {
            _cookie = HttpCookieFactory.Create(COOKIE_KEY);

            CookieKeyExists();
            Assert.Null(_cookie.Value);
        }

        [Fact]
        public void Create_WithValueParameter_CookieSetsValue()
        {
            _cookie = HttpCookieFactory.Create(COOKIE_KEY, "my value");

            CookieKeyExists();
            Assert.Equal("my value", _cookie.Value);
        }

        [Fact]
        public void Create_WithHttpOnlyAndSecureParameters_SetProperties()
        {
            _cookie = HttpCookieFactory.Create(COOKIE_KEY, httpOnly: false, secure: false);

            CookieKeyExists();
            Assert.False(_cookie.HttpOnly);
            Assert.False(_cookie.Secure);
        }

        [Fact]
        public void SetCookie_DefaultParameters_SetsCookie()
        {
            var response = new Mock<HttpResponseBase>();
            var cookies = new HttpCookieCollection();
            response.Setup(x => x.Cookies).Returns(cookies);
            response.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).Callback<HttpCookie>(x => cookies.Add(x));
            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Response).Returns(response.Object);

            HttpCookieFactory.SetCookie(response.Object, "key");

            response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Once());
        }

        [Fact]
        public void ExpireCookie_DefaultParameters_ExpiresCookie()
        {
            var response = new Mock<HttpResponseBase>();
            var cookies = new HttpCookieCollection();
            response.Setup(x => x.Cookies).Returns(cookies);
            response.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).Callback<HttpCookie>(x => cookies.Add(x));
            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Response).Returns(response.Object);

            HttpCookieFactory.ExpireCookie(response.Object, "key");

            response.Verify(x => x.SetCookie(It.IsAny<HttpCookie>()), Times.Once());
            Assert.Equal(
                (cookies["key"].Expires - DateTime.UtcNow).Days,
                HttpCookieFactory.ExpiresDays
            );
        }

    }
}