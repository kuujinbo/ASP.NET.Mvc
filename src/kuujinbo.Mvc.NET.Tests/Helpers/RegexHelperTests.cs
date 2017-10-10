using kuujinbo.Mvc.NET.Helpers;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Helpers
{
    public class RegexHelperTests
    {
        [Fact]
        public void PascalCaseSplit_MatchingString_InsertsSpaces()
        {
            Assert.Equal<string>(
                "Hello World World", 
                RegexHelper.PascalCaseSplit("HelloWorldWorld")
            );
        }

        [Fact]
        public void PascalCaseSplit_MatchingString_IsNoOp()
        {
            string expected = "TH T7 T^ jk $# 08 j$0";

            Assert.Equal<string>(
                expected, 
                RegexHelper.PascalCaseSplit(expected)
            );
        }

        [Fact]
        public void PascalCaseSplit_NullString_ReturnsNull()
        {
            Assert.Null(RegexHelper.PascalCaseSplit(null));
        }
    }
}