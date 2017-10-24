using System;
using System.Diagnostics.CodeAnalysis;
using kuujinbo.Mvc.NET.Helpers;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Helpers
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class BoolHelperTests
    {
        [Fact]
        public void TrueCount_NullParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => BoolHelper.TrueCount(null)
            );
        }

        [Fact]
        public void TrueCount_ValidParameter_ReturnsCorrectCount()
        {
            Assert.Equal(2, BoolHelper.TrueCount(true, false, true, false, false));
        }
    }
}
