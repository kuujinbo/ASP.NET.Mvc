using kuujinbo.Mvc.NET.Helpers;
using System;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.Mvc.NET.Tests.Helpers
{
    class NameWithNumericPrefixImpl : INameWithNumericPrefix
    {
        public string Name { get; set; }
    }

    public class NameWithNumericPrefixComparerTests
    {
        private readonly ITestOutputHelper output;
        NameWithNumericPrefixComparer comparer;

        public NameWithNumericPrefixComparerTests(ITestOutputHelper output)
        {
            this.output = output;
            comparer = new NameWithNumericPrefixComparer();
        }

        [Fact]
        public void Compare_AllAlpha_Sorts()
        {
            var alphas = new NameWithNumericPrefixImpl[]
            {
                new NameWithNumericPrefixImpl() { Name = "A" },
                new NameWithNumericPrefixImpl() { Name = "a" },
                new NameWithNumericPrefixImpl() { Name = "z" },
                new NameWithNumericPrefixImpl() { Name = "x" },
            };

            Array.Sort(alphas, new NameWithNumericPrefixComparer());

            Assert.Equal("a", alphas[0].Name);
            Assert.Equal("A", alphas[1].Name);
            Assert.Equal("x", alphas[2].Name);
            Assert.Equal("z", alphas[3].Name);
        }

        [Fact]
        public void Compare_AllNumeric_Sorts()
        {
            var alphas = new NameWithNumericPrefixImpl[]
            {
                new NameWithNumericPrefixImpl() { Name = "00" },
                new NameWithNumericPrefixImpl() { Name = "21" },
                new NameWithNumericPrefixImpl() { Name = "2" },
                new NameWithNumericPrefixImpl() { Name = "0" },
            };

            Array.Sort(alphas, new NameWithNumericPrefixComparer());

            Assert.Equal("0", alphas[0].Name);
            Assert.Equal("00", alphas[1].Name);
            Assert.Equal("2", alphas[2].Name);
            Assert.Equal("21", alphas[3].Name);
        }


        [Fact]
        public void Compare_MixedAlphaNumeric_Sorts()
        {
            var alphas = new NameWithNumericPrefixImpl[]
            {
                new NameWithNumericPrefixImpl() { Name = "A0" },
                new NameWithNumericPrefixImpl() { Name = "44-N" },
                new NameWithNumericPrefixImpl() { Name = "4-A" },
                new NameWithNumericPrefixImpl() { Name = "0" },
            };

            Array.Sort(alphas, new NameWithNumericPrefixComparer());

            Assert.Equal("0", alphas[0].Name);
            Assert.Equal("4-A", alphas[1].Name);
            Assert.Equal("44-N", alphas[2].Name);
            Assert.Equal("A0", alphas[3].Name);
        }
    }
}
