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
        Mock<Stream> _outputStream;
        Mock<HttpResponseBase> _response;

        public StreamedResultTests()
        {
            _filePath = TestFiles.GetTestDataPath(_testFile);
            _htmlContentType = "text/html";

            _outputStream = new Mock<Stream>();
            var context = new Mock<HttpContextBase>();
            _response = new Mock<HttpResponseBase>();
            _response.Setup(x => x.OutputStream).Returns(_outputStream.Object);
            context.Setup(x => x.Response).Returns(_response.Object);
        }

        public void Dispose()
        {
            if (_outputStream != null) _outputStream.Object.Dispose();
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
            var testResult = new TestStreamedResult(_filePath, _htmlContentType);

            testResult.WriteFile(_response.Object);

            _outputStream.Verify(
                x => x.Write(
                    It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()
                ),
                Times.Once()
            );
        }

        [Fact]
        public void WriteFile_ExplicitBufferSize_WritesCorrectNumberOfTimes()
        {
            var testFileLength = (int)new FileInfo(_filePath).Length;
            var testResult = new TestStreamedResult(_filePath, _htmlContentType, 1);

            testResult.WriteFile(_response.Object);

            _outputStream.Verify(
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