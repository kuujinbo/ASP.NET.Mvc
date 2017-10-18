using kuujinbo.Mvc.NET.IO;
using kuujinbo.Mvc.NET.Tests._testHelpers;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.IO
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class StreamedResultTests : IDisposable
    {
        /// <summary>
        /// Located in ../_testData
        /// </summary>
        const string _testFile = "streamed-result.txt";

        string _filePath;
        string _htmlContentType;
        Mock<Stream> _stream;

        public StreamedResultTests()
        {
            _filePath = @"\\unc-path\test.txt";
            _htmlContentType = "text/html";
        }

        public void Dispose()
        {
            if (_stream != null) _stream.Object.Dispose();
        }

        [Fact]
        public void StreamedResult_NullPath_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => new StreamedResult(null, _htmlContentType)
             );

            Assert.Equal<string>(
                StreamedResult.InvalidPathParameter, exception.Message
            );
        }

        [Fact]
        public void StreamedResult_WhiteSpacePath_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => new StreamedResult("    ", _htmlContentType)
             );

            Assert.Equal<string>(
                StreamedResult.InvalidPathParameter, exception.Message
            );
        }

        [Fact]
        public void StreamedResult_WhiteSpaceContentType_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(
                 () => new StreamedResult("file://path", "    ")
             );

            Assert.Equal<string>(
                StreamedResult.InvalidContentTypeParameter, exception.Message
            );
        }

        [Fact]
        public void StreamedResult_Constructor_SetsProperties()
        {
            var result = new StreamedResult(_filePath, _htmlContentType);

            Assert.Equal<string>(_htmlContentType, result.ContentType);
            Assert.Equal<string>(Path.GetFileName(_filePath), result.FileDownloadName);
        }

        [Fact]
        public void WriteFile_DefaultBufferSize_WritesCorrectNumberOfTimes()
        {
            var testFile = TestFiles.GetTestDataPath(_testFile);
            _stream = new Mock<Stream>();
            var context = new Mock<HttpContextBase>();
            var response = new Mock<HttpResponseBase>();
            response.Setup(x => x.OutputStream).Returns(_stream.Object);
            context.Setup(x => x.Response).Returns(response.Object);
            var testResult = new TestStreamedResult(testFile, _htmlContentType);

            testResult.WriteFile(response.Object);

            _stream.Verify(
                x => x.Write(
                    It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()
                ),
                Times.Once()
            );
        }

        [Fact]
        public void WriteFile_ExplicitBufferSize_WritesCorrectNumberOfTimes()
        {
            var testFile = TestFiles.GetTestDataPath(_testFile);
            var testFileLength = (int)new FileInfo(testFile).Length;
            _stream = new Mock<Stream>();
            var context = new Mock<HttpContextBase>();
            var response = new Mock<HttpResponseBase>();
            response.Setup(x => x.OutputStream).Returns(_stream.Object);
            context.Setup(x => x.Response).Returns(response.Object);
            var testResult = new TestStreamedResult(testFile, _htmlContentType, 1);

            testResult.WriteFile(response.Object);

            _stream.Verify(
                x => x.Write(
                    It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()
                ),
                Times.Exactly(testFileLength)
            );
        }
    }

    public class TestStreamedResult : StreamedResult 
    {
        public TestStreamedResult(
            string path, 
            string contentType, 
            int bufferSize = DefaultBufferSize)
        : base(path, contentType, bufferSize) { }

        public new void WriteFile(HttpResponseBase response) { base.WriteFile(response); }
    }
}