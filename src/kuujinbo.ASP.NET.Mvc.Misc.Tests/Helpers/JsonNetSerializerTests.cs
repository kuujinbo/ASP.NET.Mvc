using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using Xunit;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Helpers
{
    public class JsonNetSerializerTests
    {

        [Fact]
        public void Get_WithNullObject_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => JsonNetSerializer.Get(null)
            );

            Assert.Equal<string>("o", exception.ParamName);
        }

        [Fact]
        public void Get_WithDateTimeObject_ReturnsJsonString()
        {
            var date = DateTime.Now;

            var json = JsonNetSerializer.Get(date);
            var dateParts = json.Trim(new char[] { '"' })
                .Split(new char[] { '/' }, StringSplitOptions.None)
                .Select(x => Int32.Parse(x))
                .ToArray();

            Assert.StartsWith("\"", json);
            Assert.Equal(3, dateParts.Length);
            Assert.Equal(date.Month, dateParts[0]);
            Assert.Equal(date.Day, dateParts[1]);
            Assert.Equal(date.Year, dateParts[2]);
            Assert.EndsWith( "\"", json);
        }
    }
}