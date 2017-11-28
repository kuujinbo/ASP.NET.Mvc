using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.HtmlHelpers;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.HtmlHelpers
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class ScriptManagerHelperTests : IDisposable
    {
        HtmlHelper _helper;
        Mock<IViewDataContainer> _viewData;
        Mock<TextWriter> _textWriter;

        public ScriptManagerHelperTests()
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Items).Returns(new Dictionary<string, object>());

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(x => x.HttpContext).Returns(httpContext.Object);
            _textWriter = new Mock<TextWriter>();
            viewContext.Setup(x => x.Writer).Returns(_textWriter.Object);

            _viewData = new Mock<IViewDataContainer>();
            _viewData.Setup(x => x.ViewData).Returns(new ViewDataDictionary());

            _helper = new HtmlHelper(viewContext.Object, _viewData.Object);
        }

        public void Dispose()
        {
            _textWriter.Object.Dispose();
        }

        [Fact]
        public void AddViewScript_BadScriptParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => _helper.AddInlineScript(null)
             );

            Assert.Equal(ScriptManagerHelper.InvalidAddInlineScriptParameter, exception.Message);
        }

        [Fact]
        public void AddViewScript_RenderViewScripts_WritesScripts()
        {
            _helper.AddInlineScript("a");
            _helper.AddInlineScript("b");
            _helper.RenderViewScripts();

            // 2 extra calls for opening and closing <script> tag
            _textWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public void AddViewScript_WithOptionalScriptKey_RenderViewScriptsWritesScriptOnce()
        {
            var scriptKey = "script.key";
            var scriptBlock = "<script type='text/javascript'>console.log('test');</script>";

            _helper.AddInlineScript("a");
            _helper.AddInlineScript(scriptBlock, scriptKey);
            // no-op - scriptKey already seen, so scriptBlock **NOT** written
            _helper.AddInlineScript(scriptBlock, scriptKey);
            _helper.RenderViewScripts();

            // 2 extra calls for opening and closing <script> tag
            _textWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public void AddScriptSrc_RenderViewScripts_WritesScripts()
        {
            _helper.AddScriptSrc("/scripts/a.js");
            _helper.AddScriptSrc("/scripts/b.js");
            _helper.RenderViewScripts();

            _textWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}