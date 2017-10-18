namespace kuujinbo.Mvc.NET.IO
{
    public class PdfResult : StreamedResult
    {
        public const string MIMEType = "application/pdf";

        public PdfResult(string path, int bufferSize = DefaultBufferSize)
            : base(path, MIMEType, bufferSize)
        { }
    }
}