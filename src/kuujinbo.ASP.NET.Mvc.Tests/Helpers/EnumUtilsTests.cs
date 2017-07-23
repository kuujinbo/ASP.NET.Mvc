using System.ComponentModel.DataAnnotations;
using kuujinbo.ASP.NET.Mvc.Helpers;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Tests.Helpers
{
    public enum TestEnum
    {
        One,
        [Display(Name = "2")]
        Two,
        ThreeFour
    }

    public class EnumUtilsTests
    {
        [Fact]
        public void DisplayText_Default_ReturnsEnumToString()
        {
            Assert.Equal(
                TestEnum.One.ToString(), 
                EnumUtils.DisplayText(TestEnum.One)
            );
        }

        [Fact]
        public void DisplayText_DisplayAttribute_ReturnsName()
        {
            Assert.Equal(
                "2",
                EnumUtils.DisplayText(TestEnum.Two)
            );
        }

        [Fact]
        public void DisplayText_PascalCase_ReturnsEnumWithSpaces()
        {
            Assert.Equal(
                "Three Four",
                EnumUtils.DisplayText(TestEnum.ThreeFour)
            );
        }
    }
}