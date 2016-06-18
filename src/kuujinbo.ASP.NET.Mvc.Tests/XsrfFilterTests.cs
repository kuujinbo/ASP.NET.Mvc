using System.Web;
using System.Web.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.ASP.NET.Mvc.Tests
{
    public class XsrfFilterTests
    {
        private HttpContextBase _fakeContext;
        private readonly ITestOutputHelper _output;
        private readonly string[] _nonXsrfMethods =  
        { 
            HttpVerbs.Get.ToString(), HttpVerbs.Head.ToString(), 
            HttpVerbs.Options.ToString(), HttpVerbs.Patch.ToString() 
        };

        public XsrfFilterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void IsXsrfMethod_Methods_MatchCaseInsensitive()
        {
            Assert.True(XsrfFilter.IsXsrfMethod("post"));
            Assert.True(XsrfFilter.IsXsrfMethod("POst"));
            Assert.True(XsrfFilter.IsXsrfMethod("delete"));
            Assert.True(XsrfFilter.IsXsrfMethod("DElete"));
            Assert.True(XsrfFilter.IsXsrfMethod("put"));
            Assert.True(XsrfFilter.IsXsrfMethod("PUt"));
        }

        [Fact]
        public void GetFilter_NonXsrfMethods_ReturnsNull()
        {
            foreach (var method in _nonXsrfMethods)
            {
                _fakeContext = MvcMockHelpers.FakeHttpContext();
                _fakeContext.Request.SetHttpMethodResult(method);
                _output.WriteLine("HttpMethod => {0}", method);
                var postResult = XsrfFilter.GetFilter(_fakeContext.Request, new object[0]);
                Assert.False(_fakeContext.Request.IsAjaxRequest());
                Assert.Equal(method, _fakeContext.Request.HttpMethod);
                Assert.Null(postResult);
            }
        }

        [Fact]
        public void GetFilter_XsrfMethods_ReturnsValidateAntiForgeryTokenAttribute()
        {
            foreach (var method in XsrfFilter.XsrfMethods)
            {
                _fakeContext = MvcMockHelpers.FakeHttpContext();
                _fakeContext.Request.SetHttpMethodResult(method);
                _output.WriteLine("HttpMethod => {0}", method);
                var postResult = XsrfFilter.GetFilter(_fakeContext.Request, new object[0]);
                Assert.False(_fakeContext.Request.IsAjaxRequest());
                Assert.Equal(method, _fakeContext.Request.HttpMethod);
                Assert.IsType<ValidateAntiForgeryTokenAttribute>(postResult);
            }
        }

        [Fact]
        public void GetFilter_NonXsrfMethodsIsAjax_ReturnsNull()
        {
            foreach (var method in _nonXsrfMethods)
            {
                _fakeContext = MvcMockHelpers.FakeHttpContext();
                _fakeContext.Request.SetHttpMethodResult(method);
                _fakeContext.Request.SetAjaxHeaders();
                _output.WriteLine("HttpMethod => {0}", method);
                var postResult = XsrfFilter.GetFilter(_fakeContext.Request, new object[0]);
                Assert.True(_fakeContext.Request.IsAjaxRequest());
                Assert.Equal(method, _fakeContext.Request.HttpMethod);
                Assert.Null(postResult);
            }
        }

        [Fact]
        public void GetFilter_XsrfMethodsIsAjax_ReturnsValidateJsonAntiForgeryTokenAttribute()
        {
            foreach (var method in XsrfFilter.XsrfMethods)
            {
                _fakeContext = MvcMockHelpers.FakeHttpContext();
                _fakeContext.Request.SetHttpMethodResult(method);
                _fakeContext.Request.SetAjaxHeaders();
                _output.WriteLine("HttpMethod => {0}", method);
                var postResult = XsrfFilter.GetFilter(_fakeContext.Request, new object[0]);
                Assert.True(_fakeContext.Request.IsAjaxRequest());
                Assert.Equal(method, _fakeContext.Request.HttpMethod);
                Assert.IsType<ValidateJsonAntiForgeryTokenAttribute>(postResult);
            }
        }

        [Fact]
        public void GetFilter_XsrfMethodsWithIgnoreAttribute_ReturnsNull()
        {
            foreach (var method in XsrfFilter.XsrfMethods)
            {
                _fakeContext = MvcMockHelpers.FakeHttpContext();
                _fakeContext.Request.SetHttpMethodResult(method);
                _output.WriteLine("HttpMethod => {0}", method);
                var postResult = XsrfFilter.GetFilter(
                    _fakeContext.Request, 
                    new object[] { new IgnoreXsrfFilterAttribute()}
                );
                Assert.False(_fakeContext.Request.IsAjaxRequest());
                Assert.Equal(method, _fakeContext.Request.HttpMethod);
                Assert.Null(postResult);
            }
        }

        [Fact]
        public void GetFilter_XsrfMethodsIsAjaxWithIgnoreAttribute_ReturnsNull()
        {
            foreach (var method in XsrfFilter.XsrfMethods)
            {
                _fakeContext = MvcMockHelpers.FakeHttpContext();
                _fakeContext.Request.SetHttpMethodResult(method);
                _fakeContext.Request.SetAjaxHeaders();
                _output.WriteLine("HttpMethod => {0}", method);
                var postResult = XsrfFilter.GetFilter(
                    _fakeContext.Request,
                    new object[] { new IgnoreXsrfFilterAttribute() }
                );
                Assert.True(_fakeContext.Request.IsAjaxRequest());
                Assert.Equal(method, _fakeContext.Request.HttpMethod);
                Assert.Null(postResult);
            }
        }


        [Fact]
        public void Get_XsrfMethods_ReturnsConditionalFilterProvider()
        {
            foreach (var method in XsrfFilter.XsrfMethods)
            {
                _fakeContext = MvcMockHelpers.FakeHttpContext();
                _fakeContext.Request.SetHttpMethodResult(method);
                _output.WriteLine("HttpMethod => {0}", method);
                var filter = XsrfFilter.Get();

                Assert.IsType(typeof(ConditionalFilterProvider), filter);
            }
        }


    }
}