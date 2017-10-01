using kuujinbo.Mvc.NET.Tests._testHelpers;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
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
        public void Logout_DefaultParameters_SetsTempDataKey()
        {
            TempDataDictionary tempData = new TempDataDictionary();
            _fakeContext = MvcMockHelpers.FakeHttpContext();
            _fakeContext.Request.SetRequestQueryString(new NameValueCollection());

            _sessionTerminator.Logout(_fakeContext.Request, _fakeContext.Response, tempData);

            Assert.Equal(true, (bool)tempData[SessionTerminator.SESSION_TIMED_OUT]);
        }

        [Fact]
        public void Logout_HasQueryString_DoesNotSetTempDataKey()
        {
            TempDataDictionary tempData = new TempDataDictionary();
            _fakeContext = MvcMockHelpers.FakeHttpContext();
            var querystring = new NameValueCollection();
            querystring.Add("logoutClick", "true");
            _fakeContext.Request.SetRequestQueryString(querystring);

            _sessionTerminator.Logout(_fakeContext.Request, _fakeContext.Response, tempData);

            Assert.Null(tempData[SessionTerminator.SESSION_TIMED_OUT]);
        }

        [Fact]
        public void Logout_NoticeConsentCookie_RemovesCookie()
        {
            _fakeContext = MvcMockHelpers.FakeHttpContext();
            _fakeContext.Request.SetRequestQueryString(new NameValueCollection());
            var consentCookie = new HttpCookie(HttpCookieFactory.NOTICE_AND_CONSENT);
            var now = DateTime.Now;
            consentCookie.Value = now.ToString();
            var cookieCollection = new HttpCookieCollection();
            cookieCollection.Add(consentCookie);
            _fakeContext.Request.SetRequestCookies(cookieCollection);

            _sessionTerminator.Logout(_fakeContext.Request, _fakeContext.Response, new TempDataDictionary());

            Assert.Equal(1, 1);
            Assert.Equal(
                _fakeContext.Request.Cookies[HttpCookieFactory.NOTICE_AND_CONSENT].Expires.Day,
                now.AddDays(-1).Day
            );
        }
    }
}