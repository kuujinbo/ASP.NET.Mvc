using System;
using System.Linq;
using System.Xml.Linq;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;
using Xunit;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Services.JqueryDataTables
{
    public class ActionButtonTests
    {
        [Fact]
        public void Constructor_NullOrWhiteSpaceUrl_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => new ActionButton(null, "text")
             );

            Assert.Equal<string>("url", exception.ParamName);
        }

        [Fact]
        public void Constructor_NullOrWhiteSpaceText_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => new ActionButton("url", null)
             );

            Assert.Equal<string>("text", exception.ParamName);
        }

        [Fact]
        public void GetHtml_IsButton_ReturnsButtonHtml()
        {
            var actionButton = new ActionButton("url", "text");

            var xElement = XElement.Parse(actionButton.GetHtml());

            Assert.Equal(2, xElement.Nodes().Count());
            Assert.Equal("button", xElement.Name);
            Assert.Equal("url", xElement.Attribute("data-url").Value);
            Assert.Equal(
                "text",
                string.Concat(
                    xElement.Nodes().OfType<XText>().Select(x => x.Value.Trim())
                )
            );
            Assert.Equal(1, xElement.Elements("span").Count());
        }

        [Fact]
        public void GetHtml_IsNotButton_ReturnsHyperlinkHtml()
        {
            var actionButton = new ActionButton("url", "text") { IsButton = false };

            var xElement = XElement.Parse(actionButton.GetHtml());

            Assert.Equal(0, xElement.Elements().Count());
            Assert.Equal("a", xElement.Name);
            Assert.Equal("url", xElement.Attribute("href").Value);
            Assert.Equal(
                "text",
                string.Concat(
                    xElement.Nodes().OfType<XText>().Select(x => x.Value.Trim())
                )
            );
        }
    }
}