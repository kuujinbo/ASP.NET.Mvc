using System;
using kuujinbo.ASP.NET.Mvc.Misc.Services;
using kuujinbo.ASP.NET.Mvc.Misc.Tests.Properties;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Services
{
    public class CacInfoTests
    {
        private CacInfo _cacInfo;
        public CacInfoTests()
        {
            _cacInfo = new CacInfo();
        }

        [Fact]
        public void Get_NullRawData_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => _cacInfo.Get(null)
             );

            Assert.Equal<string>(CacInfo.NULL_GET_PARAM, exception.ParamName);
        }

        [Fact]
        public void Get_ValidRawDataCertificate_ReturnsCacPopulatedCacInfo()
        {
            var cacInfo = _cacInfo.Get(Resources.CacInfo_cert);

            Assert.Equal<string>("Last", cacInfo.LastName);
            Assert.Equal<string>("First", cacInfo.FirstName);
            Assert.Equal<string>("0987654321", cacInfo.Edipi);
            Assert.Equal<string>("email@domain", cacInfo.Email);
        }

        [Fact]
        public void ParseSimpleName_InvalidFormat_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacInfo.GetSimpleName("")
             );

            Assert.Equal<string>(CacInfo.BAD_SIMPLE_NAME, exception.Message);
        }

        [Fact]
        public void ParseSimpleName_InvalidEdipi_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacInfo.GetSimpleName("last.first.middle.0")
             );

            Assert.Equal<string>(CacInfo.BAD_EDIPI, exception.Message);
        }

        [Fact]
        public void ParseSimpleName_ThreeParts_ReturnsCacInfo()
        {
            var cacInfo = CacInfo.GetSimpleName("last.first.0987654321");

            Assert.Equal<string>("Last", cacInfo.LastName);
            Assert.Equal<string>("First", cacInfo.FirstName);
            Assert.Equal<string>("0987654321", cacInfo.Edipi);
        }

        [Fact]
        public void ParseSimpleName_FourParts_ReturnsCacInfo()
        {
            var cacInfo = CacInfo.GetSimpleName("last.first.middle.0987654321");

            Assert.Equal<string>("Last", cacInfo.LastName);
            Assert.Equal<string>("First", cacInfo.FirstName);
            Assert.Equal<string>("0987654321", cacInfo.Edipi);
        }

        [Fact]
        public void ParseSimpleName_MoreThanFourParts_ReturnsCacInfo()
        {
            var cacInfo = CacInfo.GetSimpleName("last.first.middle.cac-office-typo.0987654321");

            Assert.Equal<string>("Last", cacInfo.LastName);
            Assert.Equal<string>("First", cacInfo.FirstName);
            Assert.Equal<string>("0987654321", cacInfo.Edipi);
        }

        [Fact]
        public void ValidEdipi_DoesNotMatchRegex_ReturnsFalse()
        {
            Assert.False(CacInfo.ValidEdipi("jkjkl"));
            Assert.False(CacInfo.ValidEdipi("123e567%90"));
            Assert.False(CacInfo.ValidEdipi("123456789"));
            Assert.False(CacInfo.ValidEdipi("123456789 "));
            Assert.False(CacInfo.ValidEdipi(" 123456789"));
        }

        [Fact]
        public void ValidEdipi_MatchesRegex_ReturnsTrue()
        {
            Assert.True(CacInfo.ValidEdipi("0987654321"));
        }

        [Fact]
        public void TitleCase_Null_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacInfo.TitleCase(null)
             );

            Assert.Equal<string>(CacInfo.BAD_TITLE_CASE_TEXT, exception.Message);
        }

        [Fact]
        public void TitleCase_StringEmpty_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacInfo.TitleCase(string.Empty)
             );

            Assert.Equal<string>(CacInfo.BAD_TITLE_CASE_TEXT, exception.Message);
        }

        [Fact]
        public void TitleCase_AllWhiteSpace_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacInfo.TitleCase("   ")
             );

            Assert.Equal<string>(CacInfo.BAD_TITLE_CASE_TEXT, exception.Message);
        }

        [Fact]
        public void TitleCase_AllUppercase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal<string>("Text", CacInfo.TitleCase("TEXT"));
        }

        [Fact]
        public void TitleCase_AllLowercase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal<string>("Text", CacInfo.TitleCase("text"));
        }

        [Fact]
        public void TitleCase_MixedCase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal<string>("Text", CacInfo.TitleCase("teXt"));
        }
    }
}