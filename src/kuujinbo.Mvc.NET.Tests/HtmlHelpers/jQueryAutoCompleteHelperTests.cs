using kuujinbo.Mvc.NET.HtmlHelpers;
using Moq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.Mvc.NET.Tests.HtmlHelpers
{
    public class JQueryAutoCompleteHelperTests
    {
        readonly ITestOutputHelper _output;
        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;

        public JQueryAutoCompleteHelperTests(ITestOutputHelper output)
        {
            _output = output;

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Items).Returns(new Dictionary<string, object>());

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _viewData = new Mock<IViewDataContainer>();
            _viewData.Setup(x => x.ViewData).Returns(new ViewDataDictionary());

            _helper = new HtmlHelper(viewContext.Object, _viewData.Object);
        }

        private string CreateInputElement(
            string cssIdSelector
            , string searchUrl
            , string minSearchLength = "1")
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttribute(JQueryAutoCompleteHelper.IdAttribute, cssIdSelector, true);
            tagBuilder.MergeAttribute(JQueryAutoCompleteHelper.UrlAttribute, searchUrl, true);
            tagBuilder.MergeAttribute(JQueryAutoCompleteHelper.MinLengthAttribute, minSearchLength, true);

            return tagBuilder.ToString();
        }

        [Fact]
        public void JQueryAutoComplete_DefaultParameters_ReturnsHtml()
        {
            var cssSelector = "#selector";
            var url = "/url";

            var result = _helper.JQueryAutoComplete(cssSelector, url);
            var expected = CreateInputElement(cssSelector, url);

            Assert.Equal(expected, result.ToString());
        }
    }
}