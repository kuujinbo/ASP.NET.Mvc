using kuujinbo.Mvc.NET.HtmlHelpers;
using Moq;
using System.Web.Mvc;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.HtmlHelpers
{
    public class UserFriendlyBoolTests
    {
        string altTrue = "当たり前";
        string altFalse = "Hell NO!";

        [Fact]
        public void Convert_DefaultContructorParametersTrue_ReturnsUserFriendlyTrue()
        {
            var result = new UserFriendlyBool().Convert(true);

            Assert.Equal(UserFriendlyBool.UserFriendlyTrue, result.ToString());
        }

        [Fact]
        public void Convert_DefaultContructorParametersFalse_ReturnsUserFriendlyFalse()
        {
            var result = new UserFriendlyBool().Convert(false);

            Assert.Equal(UserFriendlyBool.UserFriendlyFalse, result.ToString());
        }

        [Fact]
        public void Convert_DefaultContructorParametersNull_ReturnsUserFriendlyFalse()
        {
            bool? nullBool = null;
            var result = new UserFriendlyBool().Convert(nullBool);

            Assert.Equal(UserFriendlyBool.UserFriendlyFalse, result.ToString());
        }

        [Fact]
        public void Convert_DefaultContructorParametersNullableTrue_ReturnsUserFriendlyTrue()
        {
            bool? nullBool = true;
            var result = new UserFriendlyBool().Convert(nullBool);

            Assert.Equal(UserFriendlyBool.UserFriendlyTrue, result.ToString());
        }

        [Fact]
        public void Convert_DefaultContructorParametersNullableFalse_ReturnsUserFriendlyFalse()
        {
            bool? nullBool = false;
            var result = new UserFriendlyBool().Convert(nullBool);

            Assert.Equal(UserFriendlyBool.UserFriendlyFalse, result.ToString());
        }


        [Fact]
        public void Convert_ExplicitContructorParametersNullableFalse_ReturnsUserFriendlyFalse()
        {
            bool? nullBool = false;
            var result = new UserFriendlyBool(altTrue, altFalse).Convert(nullBool);

            Assert.Equal(altFalse, result.ToString());
        }

        [Fact]
        public void Convert_ExplicitContructorParametersWithBool_ReturnsUserFriendlyValues()
        {
            Assert.Equal(
                altFalse,
                new UserFriendlyBool(altTrue, altFalse).Convert(false).ToString()
            );
            Assert.Equal(
                altTrue,
                new UserFriendlyBool(altTrue, altFalse).Convert(true).ToString()
            );
        }

        [Fact]
        public void Convert_ExplicitContructorParametersWithNullableBool_ReturnsUserFriendlyValues()
        {
            bool? nullBool = null;
            bool? nullBoolFalse = false;
            bool? nullBoolTrue = true;

            Assert.Equal(
                altFalse, 
                new UserFriendlyBool(altTrue, altFalse).Convert(nullBool).ToString()
            );
            Assert.Equal(
                altFalse, 
                new UserFriendlyBool(altTrue, altFalse).Convert(nullBoolFalse).ToString()
                
            );
            Assert.Equal(
                altTrue, 
                new UserFriendlyBool(altTrue, altFalse).Convert(nullBoolTrue).ToString()
            );
        }
    }
}