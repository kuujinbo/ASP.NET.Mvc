using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.IO;
using kuujinbo.Mvc.NET.Tests._testHelpers;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.Mvc.NET.Tests.IO
{
    public class FakeController : Controller
    {
        public ActionResult GetResults(string path, string contentType)
        {
            return new StreamedResult(path, contentType);
        }
    }

    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class StreamedResultTests
    {
        FakeController _fakeController;

        readonly ITestOutputHelper _output;
        string _htmlContentType = "text/html";

        public StreamedResultTests(ITestOutputHelper output)
        {
            _fakeController = new FakeController();
            _output = output;
        }

        [Fact]
        public void StreamedResult_NullPath_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => new FakeController().GetResults(null, _htmlContentType)
             );

            Assert.Equal<string>(StreamedResult.InvalidPathParameter, exception.Message);
        }

        [Fact]
        public void StreamedResult_WhiteSpacePath_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => new FakeController().GetResults("    ", _htmlContentType)
             );

            Assert.Equal<string>(StreamedResult.InvalidPathParameter, exception.Message);
        }

        [Fact]
        public void StreamedResult_WhiteSpaceContentType_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => new FakeController().GetResults("file://path", "    ")
             );

            Assert.Equal<string>(StreamedResult.InvalidContentTypeParameter, exception.Message);
        }

        //[Fact]
        //public void Path()
        //{
        //    _output.WriteLine(TestFiles.GetTestDataPath("StreamedResult.txt"));
        //    Assert.True(true);
        //}

    }
}