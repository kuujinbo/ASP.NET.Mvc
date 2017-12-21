using Moq;
using System;
using System.Web;
using Xunit;

namespace kuujinbo.Mvc.NET.Helpers
{
    public class HttpRequestBaseExtensionTests
    {
        const string BaseUrl = "http://localhost";
        Mock<HttpRequestBase> _request;

        public HttpRequestBaseExtensionTests()
        {
            _request = new Mock<HttpRequestBase>();
            _request.Setup(x => x.Url).Returns(new Uri(BaseUrl));
        }

        [Fact]
        public void GetAbsoluteUrl_RootAppPathNullRelativeUrl_ReturnsAbsoluteUrl()
        {
            var root = "/";
            _request.Setup(x => x.ApplicationPath).Returns(root);

            Assert.Equal(BaseUrl + root, _request.Object.GetAbsoluteUrl());
        }

        [Fact]
        public void GetAbsoluteUrl_SubAppPathNullRelativeUrl_ReturnsAbsoluteUrl()
        {
            var subPath = "//test";
            _request.Setup(x => x.ApplicationPath).Returns(subPath);

            Assert.Equal(
                string.Format("{0}/test/", BaseUrl),
                _request.Object.GetAbsoluteUrl()
            );
        }

        [Fact]
        public void GetAbsoluteUrl_SubAppPathRelativeUrl_ReturnsAbsoluteUrl()
        {
            var subPath = "test";
            _request.Setup(x => x.ApplicationPath).Returns(subPath);
            var relativeUrl = "//relative-url";

            Assert.Equal(
                string.Format("{0}/test/relative-url", BaseUrl),
                _request.Object.GetAbsoluteUrl(relativeUrl)
            );
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