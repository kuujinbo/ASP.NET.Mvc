using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables.Tests
{
    public class AppSettingsReaderTests
    {
        // see app.config in **this** project
        AppSettingsReader _reader;

        public AppSettingsReaderTests()
        {
            _reader = new AppSettingsReader();
        }

        [Fact]
        public void BoolTrue_WithValue_GetsAppSettingsValue()
        {
            Assert.Equal("Yes", _reader.BoolTrue);
        }

        [Fact]
        public void BoolFalse_WithMissingValue_ReturnsNull()
        {
            Assert.Equal(null, _reader.BoolFalse);
        }

        [Fact]
        public void DateFormat_WithMissingValue_ReturnsNull()
        {
            Assert.Equal(null, _reader.DateFormat);
        }
    }
}
