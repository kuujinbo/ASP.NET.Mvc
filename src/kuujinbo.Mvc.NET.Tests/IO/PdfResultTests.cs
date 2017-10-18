using kuujinbo.Mvc.NET.IO;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests.IO
{
    public class PdfResultTests
    {

        [Fact]
        public void Constructor_WithParameters_SetsProperties()
        {
            var path = "test.txt";
            var result = new PdfResult(path);

            Assert.Equal<string>(PdfResult.MIMEType, result.ContentType);
            Assert.Equal<string>(path, result.FileDownloadName);
        }
    }
}