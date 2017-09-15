using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class CookieFactoryTests
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
                 () => CookieFactory.Create(null)
             );

            Assert.Equal<string>(CookieFactory.BAD_COOKIE_NAME, exception.ParamName);
        }

        [Fact]
        public void Create_WithWhiteSpaceNameParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => CookieFactory.Create(" ")
             );

            Assert.Equal<string>(CookieFactory.BAD_COOKIE_NAME, exception.ParamName);
        }

        [Fact]
        public void Create_DefaultHttpOnlyAndSecureParameters_AreHttpOnlyAndSecure()
        {
            _cookie = CookieFactory.Create(COOKIE_KEY);

            CookieKeyExists();
            Assert.True(_cookie.HttpOnly);
            Assert.True(_cookie.Secure);
        }

        [Fact]
        public void Create_WithoutValueParameter_CookieValueIsNull()
        {
            _cookie = CookieFactory.Create(COOKIE_KEY);

            CookieKeyExists();
            Assert.Null(_cookie.Value);
        }

        [Fact]
        public void Create_WithValueParameter_CookieSetsValue()
        {
            _cookie = CookieFactory.Create(COOKIE_KEY, "my value");

            CookieKeyExists();
            Assert.Equal("my value", _cookie.Value);
        }

        [Fact]
        public void Create_WithHttpOnlyAndSecureParameters_SetProperties()
        {
            _cookie = CookieFactory.Create(COOKIE_KEY, httpOnly: false, secure: false);

            CookieKeyExists();
            Assert.False(_cookie.HttpOnly);
            Assert.False(_cookie.Secure);
        }
    }
}