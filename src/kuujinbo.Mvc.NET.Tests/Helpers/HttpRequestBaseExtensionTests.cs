using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Attributes;
using kuujinbo.Mvc.NET.Helpers;
using kuujinbo.Mvc.NET.Tests._testHelpers;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Helpers
{
    public class HttpRequestBaseExtensionTests
    {
        Mock<HttpRequestBase> _request;

        public HttpRequestBaseExtensionTests()
        {
            _request = new Mock<HttpRequestBase>();
            _request.Setup(x => x.Url).Returns(new Uri("http://test.com"));
        }

        [Fact]
        public void AppendQueryString_RelativePathWithoutQueryStringAndNoValue_ReturnsCorrectPathAndQuery()
        {
            var testUrl = "/controllerName/actionName";
            var key = "key";

            var result = _request.Object.AppendQueryString(testUrl, key);

            Assert.Equal(string.Format("{0}?{1}=0", testUrl, key), result);
        }

        [Fact]
        public void AppendQueryString_RelativePathWithoutQueryStringValue_ReturnsCorrectPathAndQuery()
        {
            var testUrl = "/controllerName/actionName";
            var key = "key";
            var value = "value";

            var result = _request.Object.AppendQueryString(testUrl, key, value);

            Assert.Equal(string.Format("{0}?{1}={2}", testUrl, key, value), result);
        }

        [Fact]
        public void AppendQueryString_RelativePathWithQueryStringAndNoValue_ReturnsCorrectPathAndQuery()
        {
            var testUrl = "/controllerName/actionName?a=b";
            var key = "key";

            var result = _request.Object.AppendQueryString(testUrl, key);

            Assert.Equal(string.Format("{0}&{1}=0", testUrl, key), result);
        }

        [Fact]
        public void AppendQueryString_RelativePathWithQueryStringAndValue_ReturnsCorrectPathAndQuery()
        {
            var testUrl = "/controllerName/actionName?a=b";
            var key = "key";
            var value = "value";

            var result = _request.Object.AppendQueryString(testUrl, key, value);

            Assert.Equal(string.Format("{0}&{1}={2}", testUrl, key, value), result);
        }
    }
}