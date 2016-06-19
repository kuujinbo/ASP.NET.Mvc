using System;
using kuujinbo.ASP.NET.Mvc.Services;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Services
{
    public class DateFormatValidatorTests
    {
        [Fact]
        public void Parse_InvalidFormat_ThrowsArgumentNullException()
        {
            var invalid = "invalid";
            var exception = Assert.Throws<FormatException>(
                () => DateFormatValidator.Parse(invalid)
            );
            var expected = string.Format(DateFormatValidator.BAD_DATE_FORMAT, invalid);

            Assert.Equal<string>(expected, exception.Message);
        }

        [Fact]
        public void Parse_ValidFormat_ThrowsArgumentNullException()
        {
            Assert.True(DateFormatValidator.Parse("yyy-MM-dd"));
        }
    }
}
