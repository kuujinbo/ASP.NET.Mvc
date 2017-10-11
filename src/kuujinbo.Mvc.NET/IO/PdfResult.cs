namespace kuujinbo.Mvc.NET.IO
{
    public class PdfResult : StreamedResult
    {
        public PdfResult(string path, int bufferSize = DefaultBufferSize)
            : base(path, "application/pdf", bufferSize)
        { }
    }
}