using kuujinbo.Mvc.NET.HtmlHelpers;
using Moq;
using System.Web.Mvc;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.HtmlHelpers
{
    public class UserFriendlyBoolHelperTests
    {
        HtmlHelper _helper;

        public UserFriendlyBoolHelperTests()
        {
            _helper = new HtmlHelper(new Mock<ViewContext>().Object, new Mock<IViewDataContainer>().Object);
        }

        [Fact]
        public void UserFriendlyBool_True_ReturnsUserFriendlyTrue()
        {
            var result = _helper.UserFriendlyBool(true);

            Assert.Equal(UserFriendlyBoolHelper.UserFriendlyTrue, result.ToString());
        }

        [Fact]
        public void UserFriendlyBool_False_ReturnsUserFriendlyFalse()
        {
            var result = _helper.UserFriendlyBool(false);

            Assert.Equal(UserFriendlyBoolHelper.UserFriendlyFalse, result.ToString());
        }

        [Fact]
        public void UserFriendlyBool_Null_ReturnsUserFriendlyFalse()
        {
            bool? nullBool = null;
            var result = _helper.UserFriendlyBool(nullBool);

            Assert.Equal(UserFriendlyBoolHelper.UserFriendlyFalse, result.ToString());
        }

        [Fact]
        public void UserFriendlyBool_NullableTrue_ReturnsUserFriendlyTrue()
        {
            bool? nullBool = true;
            var result = _helper.UserFriendlyBool(nullBool);

            Assert.Equal(UserFriendlyBoolHelper.UserFriendlyTrue, result.ToString());
        }

        [Fact]
        public void UserFriendlyBool_NullableFalse_ReturnsUserFriendlyFalse()
        {
            bool? nullBool = false;
            var result = _helper.UserFriendlyBool(nullBool);

            Assert.Equal(UserFriendlyBoolHelper.UserFriendlyFalse, result.ToString());
        }
    }
}