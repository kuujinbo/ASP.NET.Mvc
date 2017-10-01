using System;
using System.Diagnostics.CodeAnalysis;
using kuujinbo.Mvc.NET.Tests.Properties;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class DodCacTests
    {
        private DodCac _dodCac;
        public DodCacTests()
        {
            _dodCac = new DodCac();
        }

        [Fact]
        public void Get_NullRawData_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => _dodCac.Get(null)
             );

            Assert.Equal<string>(DodCac.BAD_GET_PARAM, exception.ParamName);
        }

        [Fact]
        public void Get_ValidRawDataCertificate_ReturnsPopulatedCacInfo()
        {
            var cac = _dodCac.Get(Resources.DodCac_cert);

            Assert.Equal<string>("Last", cac.LastName);
            Assert.Equal<string>("First", cac.FirstName);
            Assert.Equal<string>("0987654321", cac.Edipi);
            Assert.Equal<string>("email@domain", cac.Email);
        }

        [Fact]
        public void ParseSimpleName_InvalidFormat_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => DodCac.GetDodCac("")
             );

            Assert.Equal<string>(DodCac.BAD_SIMPLE_NAME, exception.Message);
        }

        [Fact]
        public void ParseSimpleName_InvalidEdipi_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => DodCac.GetDodCac("last.first.middle.0")
             );

            Assert.Equal<string>(DodCac.BAD_EDIPI, exception.Message);
        }

        [Fact]
        public void ParseSimpleName_ThreeParts_ReturnsCacInfo()
        {
            var cac = DodCac.GetDodCac("last.first.0987654321");

            Assert.Equal<string>("Last", cac.LastName);
            Assert.Equal<string>("First", cac.FirstName);
            Assert.Equal<string>("0987654321", cac.Edipi);
        }

        [Fact]
        public void ParseSimpleName_FourParts_ReturnsCacInfo()
        {
            var cac = DodCac.GetDodCac("last.first.middle.0987654321");

            Assert.Equal<string>("Last", cac.LastName);
            Assert.Equal<string>("First", cac.FirstName);
            Assert.Equal<string>("0987654321", cac.Edipi);
        }

        [Fact]
        public void ParseSimpleName_MoreThanFourParts_ReturnsCacInfo()
        {
            var cac = DodCac.GetDodCac("last.first.middle.cac-office-typo.0987654321");

            Assert.Equal<string>("Last", cac.LastName);
            Assert.Equal<string>("First", cac.FirstName);
            Assert.Equal<string>("0987654321", cac.Edipi);
        }

        [Fact]
        public void ValidEdipi_DoesNotMatchRegex_ReturnsFalse()
        {
            Assert.False(DodCac.ValidEdipi("jkjkl"));
            Assert.False(DodCac.ValidEdipi("123e567%90"));
            Assert.False(DodCac.ValidEdipi("123456789"));
            Assert.False(DodCac.ValidEdipi("123456789 "));
            Assert.False(DodCac.ValidEdipi(" 123456789"));
        }

        [Fact]
        public void ValidEdipi_MatchesRegex_ReturnsTrue()
        {
            Assert.True(DodCac.ValidEdipi("0987654321"));
        }

        [Fact]
        public void TitleCase_Null_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => DodCac.TitleCase(null)
             );

            Assert.Equal<string>(DodCac.BAD_TITLE_CASE_TEXT, exception.Message);
        }

        [Fact]
        public void TitleCase_StringEmpty_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => DodCac.TitleCase(string.Empty)
             );

            Assert.Equal<string>(DodCac.BAD_TITLE_CASE_TEXT, exception.Message);
        }

        [Fact]
        public void TitleCase_AllWhiteSpace_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => DodCac.TitleCase("   ")
             );

            Assert.Equal<string>(DodCac.BAD_TITLE_CASE_TEXT, exception.Message);
        }

        [Fact]
        public void TitleCase_AllUppercase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal<string>("Text", DodCac.TitleCase("TEXT"));
        }

        [Fact]
        public void TitleCase_AllLowercase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal<string>("Text", DodCac.TitleCase("text"));
        }

        [Fact]
        public void TitleCase_MixedCase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal<string>("Text", DodCac.TitleCase("teXt"));
        }
    }
}