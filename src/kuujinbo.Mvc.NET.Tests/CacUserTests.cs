using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class CacUserTests
    {
        private CacUser _cacUser;
        public CacUserTests()
        {
            _cacUser = new CacUser();
        }

        [Fact]
        public void ParseSimpleName_InvalidFormat_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacUser.Create("")
             );

            Assert.Equal(CacUser.InvalidSimpleNameParameter, exception.Message);
        }

        [Fact]
        public void ParseSimpleName_InvalidEdipi_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacUser.Create("last.first.middle.0")
             );

            Assert.Equal(CacUser.InvalidEdipi, exception.Message);
        }

        [Fact]
        public void ParseSimpleName_ThreeParts_ReturnsCacInfo()
        {
            var cac = CacUser.Create("last.first.0987654321");

            Assert.Equal("Last", cac.LastName);
            Assert.Equal("First", cac.FirstName);
            Assert.Equal(string.Empty, cac.MiddleName);
            Assert.Equal("0987654321", cac.Edipi);
        }

        [Fact]
        public void ParseSimpleName_FourParts_ReturnsCacInfo()
        {
            var cac = CacUser.Create("last.first.middle.0987654321");

            Assert.Equal("Last", cac.LastName);
            Assert.Equal("First", cac.FirstName);
            Assert.Equal("Middle", cac.MiddleName);
            Assert.Equal("0987654321", cac.Edipi);
        }

        [Fact]
        public void ParseSimpleName_MoreThanFourParts_ReturnsCacInfo()
        {
            var cac = CacUser.Create("last.first.middle.cac-office-typo.0987654321");

            Assert.Equal("Last", cac.LastName);
            Assert.Equal("First", cac.FirstName);
            Assert.Equal("Middle", cac.MiddleName);
            Assert.Equal("0987654321", cac.Edipi);
        }

        [Fact]
        public void ValidEdipi_DoesNotMatchRegex_ReturnsFalse()
        {
            Assert.False(CacUser.ValidEdipi("jkjkl"));
            Assert.False(CacUser.ValidEdipi("123e567%90"));
            Assert.False(CacUser.ValidEdipi("123456789"));
            Assert.False(CacUser.ValidEdipi("123456789 "));
            Assert.False(CacUser.ValidEdipi(" 123456789"));
        }

        [Fact]
        public void ValidEdipi_MatchesRegex_ReturnsTrue()
        {
            Assert.True(CacUser.ValidEdipi("0987654321"));
        }

        [Fact]
        public void TitleCase_Null_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacUser.TitleCase(null)
             );

            Assert.Equal(CacUser.InvalidTitleCaseParameter, exception.Message);
        }

        [Fact]
        public void TitleCase_StringEmpty_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacUser.TitleCase(string.Empty)
             );

            Assert.Equal(CacUser.InvalidTitleCaseParameter, exception.Message);
        }

        [Fact]
        public void TitleCase_AllWhiteSpace_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(
                 () => CacUser.TitleCase("   ")
             );

            Assert.Equal(CacUser.InvalidTitleCaseParameter, exception.Message);
        }

        [Fact]
        public void TitleCase_AllUppercase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal("Text", CacUser.TitleCase("TEXT"));
        }

        [Fact]
        public void TitleCase_AllLowercase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal("Text", CacUser.TitleCase("text"));
        }

        [Fact]
        public void TitleCase_MixedCase_ReturnsFirstUpperAndRemainingLower()
        {
            Assert.Equal("Text", CacUser.TitleCase("teXt"));
        }
    }
}