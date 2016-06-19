using System;
using Newtonsoft.Json;
using Xunit;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Services.Json.Tests
{
    public class WriteBoolConverterTests
    {
        private WriteBoolConverter _converter;

        public WriteBoolConverterTests()
        {
            _converter = new WriteBoolConverter();
        }

        [Fact]
        public void CanConvert_TypeParameter_OnlyConvertsBool()
        {
            Assert.False(_converter.CanConvert(typeof(int)));
            Assert.False(_converter.CanConvert(typeof(string)));
            Assert.True(_converter.CanConvert(typeof(bool)));
        }

        [Fact]
        public void CanRead_TypeParameter_ReturnsFalse()
        {
            Assert.False(_converter.CanRead);
        }

        [Fact]
        public void CanWrite_ReturnsTrue()
        {
            Assert.True(_converter.CanWrite);
        }

        [Fact]
        public void ReadJson_ThrowsNotImplementedException()
        {
            var reader = new Mock<JsonReader>();

            var exception = Assert.Throws<NotImplementedException>(
                () => _converter.ReadJson(
                    reader.Object, typeof(bool), null, new JsonSerializer()
                )
            );
        }

        [Fact]
        public void WriteJson_Defaults_WritesValues()
        {
            string jsonTrue = JsonConvert.SerializeObject(
                true, Formatting.None, _converter
            ).Replace("\"", "");
            string jsonFalse = JsonConvert.SerializeObject(
                false, Formatting.None, _converter
            ).Replace("\"", "");

            Assert.Equal(WriteBoolConverter.TRUE, jsonTrue);
            Assert.Equal(WriteBoolConverter.FALSE, jsonFalse);
        }

        [Fact]
        public void WriteJson_PropertySetters_WriteTrueAndFalseValues()
        {
            _converter.True = "Y";
            _converter.False = "N";
            string jsonTrue = JsonConvert.SerializeObject(
                true, Formatting.None, _converter
            ).Replace("\"", "");
            string jsonFalse = JsonConvert.SerializeObject(
                false, Formatting.None, _converter
            ).Replace("\"", "");

            Assert.Equal(_converter.True, jsonTrue);
            Assert.Equal(_converter.False, jsonFalse);
        }
    }
}