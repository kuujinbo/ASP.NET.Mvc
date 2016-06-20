using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables.Tests
{
    public class DisplaySettingsTests
    {
        [Fact]
        public void BoolTrue_WithValue_GetsAppSettingsValue()
        {
            Assert.Equal("Yes", DisplaySettings.Settings.BoolTrue);
        }

        [Fact]
        public void BoolFalse_WithMissingValue_ReturnsDefault()
        {
            Assert.Equal(
                DisplaySettings.DEFAULT_FALSE, DisplaySettings.Settings.BoolFalse
            );
        }

        [Fact]
        public void DateFormat_WithMissingValue_ReturnsIsoFormat()
        {
            Assert.Equal(
                DisplaySettings.DEFAULT_DATE_FORMAT, DisplaySettings.Settings.DateFormat
            );
        }
    }
}