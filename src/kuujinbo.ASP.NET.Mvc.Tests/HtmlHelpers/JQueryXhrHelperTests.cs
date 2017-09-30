﻿using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.HtmlHelpers;
using Moq;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.HtmlHelpers
{
    public class JQueryXhrHelperTests
    {
        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;

        public JQueryXhrHelperTests()
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Items).Returns(new Dictionary<string, object>());

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _viewData = new Mock<IViewDataContainer>();
            _viewData.Setup(x => x.ViewData).Returns(new ViewDataDictionary());

            _helper = new HtmlHelper(viewContext.Object, _viewData.Object);
        }

        [Fact]
        public void XhrProcessingDomElement_WhenCalled_ReturnsStringEmpty()
        {
            var result = _helper.JQueryXhr();

            Assert.Equal(string.Empty, result.ToString());
        }
    }
}