using kuujinbo.Mvc.NET.Helpers;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.Helpers
{
    public class EnumExtensionTests
    {
        const string AttributeOne = "Attribute One";
        const string AttributeTwo = "Attribute Two";

        enum _testEnum
        {
            [DisplayFormat(DataFormatString = AttributeOne)]
            One,
            [DisplayFormat(DataFormatString = AttributeTwo)]
            Two
        }

        [Fact]
        public void GetAttributeValue_ValidEnum_ReturnsValue()
        {
            Assert.Equal(
                AttributeTwo,
                _testEnum.Two.GetAttributeValue<DisplayFormatAttribute>()
                    .DataFormatString
            );
        }
    }
}
