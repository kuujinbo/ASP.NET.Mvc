using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Attributes;
using kuujinbo.Mvc.NET.HtmlHelpers;
using kuujinbo.Mvc.NET.Tests._testHelpers;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests
{
    public class SessionTerminatorTests
    {
        SessionTerminator _sessionTerminator;
        HttpContextBase _fakeContext;

        public SessionTerminatorTests()
        {
            _sessionTerminator = new SessionTerminator();
        }

        [Fact]
        public void GetTimeout_DefaultParameter_ReturnsNonPrivilegedTimeout()
        {
            Assert.Equal(SessionTerminator.NonPrivilegedTimeout, _sessionTerminator.GetTimeout());
        }

        [Fact]
        public void GetTimeout_TrueParameter_ReturnsPrivilegedTimeout()
        {
            Assert.Equal(SessionTerminator.PrivilegedTimeout, _sessionTerminator.GetTimeout(true));
        }

        [Fact]
        public void Logout_DefaultParameters_DoesNotSetTempDataKey()
        {
            TempDataDictionary tempData = new TempDataDictionary();
            _fakeContext = MvcMockHelpers.FakeHttpContext();
            _fakeContext.Request.SetRequestQueryString(new NameValueCollection());

            _sessionTerminator.Logout(_fakeContext.Request, _fakeContext.Response, tempData);

            Assert.Null(tempData[SessionTerminator.SessionTimedOut]);
        }

        [Fact]
        public void Logout_HasQueryStringWithNullValue_DoesNotSetTempDataKey()
        {
            TempDataDictionary tempData = new TempDataDictionary();
            _fakeContext = MvcMockHelpers.FakeHttpContext();
            var querystring = new NameValueCollection();
            querystring.Add(SessionTerminatorHelper.ShowClientModalKey, null);
            _fakeContext.Request.SetRequestQueryString(querystring);

            _sessionTerminator.Logout(_fakeContext.Request, _fakeContext.Response, tempData);

            Assert.Null(tempData[SessionTerminator.SessionTimedOut]);
        }

        [Fact]
        public void Logout_HasQueryStringWithValue_SetsTempDataKey()
        {
            TempDataDictionary tempData = new TempDataDictionary();
            _fakeContext = MvcMockHelpers.FakeHttpContext();
            var querystring = new NameValueCollection();
            querystring.Add(SessionTerminatorHelper.ShowClientModalKey, "0");
            _fakeContext.Request.SetRequestQueryString(querystring);

            _sessionTerminator.Logout(_fakeContext.Request, _fakeContext.Response, tempData);

            Assert.True((bool)tempData[SessionTerminator.SessionTimedOut]);
        }

        [Fact]
        public void Logout_Called_ExpiresCookie()
        {
            var response = new Mock<HttpResponseBase>();
            var cookies = new HttpCookieCollection();
            response.Setup(x => x.Cookies).Returns(cookies);
            response.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).Callback<HttpCookie>(x => cookies.Add(x));

            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.QueryString).Returns(new NameValueCollection());

            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Response).Returns(response.Object);
            context.Setup(x => x.Request).Returns(request.Object);

            _sessionTerminator.Logout(request.Object, response.Object, new TempDataDictionary());

            // cookie is in the past
            Assert.True(
                (response.Object.Cookies[
                    NoticeAndConsentAuthorizeAttribute.NoticeAndConsent].Expires
                    - DateTime.UtcNow).Days < 0
            );
        }
    }
}