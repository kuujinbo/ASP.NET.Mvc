using System;
using System.Linq;
using System.Xml.Linq;
using kuujinbo.ASP.NET.Mvc.Misc.Helpers;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Helpers
{
    public class RegexUtilsTests
    {
        [Fact]
        public void PascalCaseSplit_MatchingString_InsertsSpaces()
        {
            Assert.Equal<string>(
                "Hello World World", 
                RegexUtils.PascalCaseSplit("HelloWorldWorld")
            );
        }

        [Fact]
        public void PascalCaseSplit_MatchingString_IsNoOp()
        {
            string expected = "TH T7 T^ jk $# 08 j$0";

            Assert.Equal<string>(
                expected, 
                RegexUtils.PascalCaseSplit(expected)
            );
        }

        [Fact]
        public void PascalCaseSplit_NullString_ReturnsNull()
        {
            Assert.Null(RegexUtils.PascalCaseSplit(null));
        }
    }
}