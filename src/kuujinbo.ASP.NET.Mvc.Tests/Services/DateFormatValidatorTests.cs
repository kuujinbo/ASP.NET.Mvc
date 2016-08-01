using kuujinbo.ASP.NET.Mvc.Services;
using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Services
{
    public class DateFormatValidatorTests
    {
        [Fact]
        public void TryParse_InvalidFormat_ReturnsFalse()
        {
            Assert.False(DateFormatValidator.TryParse("invalid"));
        }

        [Fact]
        public void TryParse_ValidFormat_ReturnsTrue()
        {
            Assert.True(DateFormatValidator.TryParse(TableSettings.DEFAULT_DATE_FORMAT));
        }
    }
}
