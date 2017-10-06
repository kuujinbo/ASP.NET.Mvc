using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using kuujinbo.Mvc.NET.Filters;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Filters
{
    public class ConditionalFilterProviderTests
    {
        [Fact]
        public void GetFilters_NoParameters_ReturnsConditionalFilterProvider()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            context.Setup(x => x.Request).Returns(request.Object);
            var controller = new Mock<ControllerBase>();
            var actionDescriptor = new Mock<ActionDescriptor>();
            var controllerContext = new ControllerContext(context.Object, new RouteData(), controller.Object);

            var result = XsrfFilter.Get();
            var filters = result.GetFilters(controllerContext, actionDescriptor.Object);

            Assert.IsType<ConditionalFilterProvider>(result);
            // xUnit and M$ Test type checking are different:
            // System.Linq.Enumerable+WhereSelectEnumerableIterator`2[[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Web.Mvc.Filter, System.Web.Mvc, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]]
            // Assert.IsType(typeof(IEnumerable<Filter>), filters);
            Assert.IsAssignableFrom<IEnumerable<Filter>>(filters);
            Assert.Equal(0, filters.Count());
        }
    }
}