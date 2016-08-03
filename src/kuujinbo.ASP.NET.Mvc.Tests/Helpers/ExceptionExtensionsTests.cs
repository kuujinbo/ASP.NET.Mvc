using kuujinbo.ASP.NET.Mvc.Helpers;
using System;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Helpers
{
    public class ExceptionExtensionsTests
    {
        [Fact]
        public void GetInnerExceptions_NoInnerExceptions_ReturnsEmptyList()
        {
            var e = new Exception();
            Assert.Equal(e.GetInnerExceptions().Count, 0);
        }

        [Fact]
        public void GetInnerExceptions_InnerExceptions_ReturnsListOfInnerExepctions()
        {
            var e1 = new Exception("e1");
            var e0 = new Exception("e0", e1);
            var e = new Exception("e", e0);

            var inner = e.GetInnerExceptions();
            Assert.Equal(inner.Count, 2);
            Assert.Equal(inner[0], e0);
            Assert.Equal(inner[1], e1);
        }
    }
}
