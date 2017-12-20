using System;
using System.Diagnostics.CodeAnalysis;
using kuujinbo.Mvc.NET.Helpers;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Helpers
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class CsvHelperTests
    {
        [Fact]
        public void RemoveEmptyDuplicates_Null_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => CsvHelper.RemoveEmptyDuplicates(null)
            );
        }

        [Fact]
        public void RemoveEmptyDuplicates_Empty_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => CsvHelper.RemoveEmptyDuplicates("")
            );
        }

        [Fact]
        public void RemoveEmptyDuplicates_AllWhitespace_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => CsvHelper.RemoveEmptyDuplicates("         ")
            );
        }

        [Fact]
        public void RemoveEmptyDuplicates_Single_ReturnsOriginal()
        {
            var original = " a ";
            Assert.Equal("a", CsvHelper.RemoveEmptyDuplicates(original));
        }

        [Fact]
        public void RemoveEmptyDuplicates_NoWhitespaceOrDuplicates_ReturnsOriginal()
        {
            var original = "a,b,c";
            Assert.Equal(original, CsvHelper.RemoveEmptyDuplicates(original));
        }

        [Fact]
        public void RemoveEmptyDuplicates_WhitespaceNoDuplicates_ReturnsParsedString()
        {
            var original = "a, b ,c";
            Assert.Equal("a,b,c", CsvHelper.RemoveEmptyDuplicates(original));
        }

        [Fact]
        public void RemoveEmptyDuplicates_NoWhitespaceDuplicates_ReturnsParsedString()
        {
            var original = "a,b,c,x,x";
            Assert.Equal("a,b,c,x", CsvHelper.RemoveEmptyDuplicates(original));
        }

        [Fact]
        public void RemoveEmptyDuplicates_WhitespaceAndDuplicates_ReturnsParsedString()
        {
            var original = "a , b    ,c,j   , j";
            Assert.Equal("a,b,c,j", CsvHelper.RemoveEmptyDuplicates(original));
        }
    }
}