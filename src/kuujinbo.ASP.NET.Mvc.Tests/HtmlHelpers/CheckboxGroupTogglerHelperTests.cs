using kuujinbo.ASP.NET.Mvc.HtmlHelpers;
using Moq;
using System.Web.Mvc;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.HtmlHelpers
{
    public class CheckboxGroupTogglerHelperTests
    {
        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;

        public CheckboxGroupTogglerHelperTests()
        {
            _viewData = new Mock<IViewDataContainer>();
            _viewData.Setup(x => x.ViewData).Returns(new ViewDataDictionary());

            _helper = new HtmlHelper(new Mock<ViewContext>().Object, _viewData.Object);
        }

        [Fact]
        public void CheckboxGroupToggler_CalledOnce_ReturnsHtmlWithOneScriptBlock()
        {
            var cssSelector = "#selector";

            var result = _helper.CheckboxGroupToggler(cssSelector);
            var expected = CheckboxGroupTogglerHelper.JavaScriptBlock
                           + string.Format(
                                CheckboxGroupTogglerHelper.JAVASCRIPT_FORMAT,
                                cssSelector, 
                                false.ToString().ToLower()
                             );

            Assert.Equal(expected, result.ToString());
            Assert.Equal<bool>(
                true, 
                (bool)_viewData.Object.ViewData[CheckboxGroupTogglerHelper.VIEW_DATA]
            );
        }

        [Fact]
        public void CheckboxGroupToggler_CalledMoreThanOnce_ReturnsHtmlWithOneScriptBlock()
        {
            var cssSelector = "#selector";

            _helper.CheckboxGroupToggler(cssSelector);
            var result = _helper.CheckboxGroupToggler(cssSelector);

            var expected = string.Format(
                               CheckboxGroupTogglerHelper.JAVASCRIPT_FORMAT,
                               cssSelector,
                               false.ToString().ToLower()
                           );

            Assert.Equal(expected, result.ToString());
        }
    }
}