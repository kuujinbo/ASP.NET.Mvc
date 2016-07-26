using System;
using kuujinbo.ASP.NET.Mvc.Services;
using Xunit;
using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;

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
