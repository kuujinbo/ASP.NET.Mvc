using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.IO
{
    public class StreamedResult : FileResult
    {
        private string _filePath;

        public const string InvalidPathParameter = "path";
        public const string InvalidContentTypeParameter = "contentType";

        public const int DefaultBufferSize = 4096;

        public int BufferSize { get; private set; }

        public StreamedResult(
            string path,
            string contentType,
            int bufferSize = DefaultBufferSize) 
        : base(contentType)
        {
            if (string.IsNullOrWhiteSpace(path)) 
                throw new ArgumentException(InvalidPathParameter);
            if (Regex.IsMatch(contentType, @"^\s+$")) 
                throw new ArgumentException(InvalidContentTypeParameter);

            this.FileDownloadName = Path.GetFileName(path);
            BufferSize = bufferSize;
            _filePath = path;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            var outputStream = response.OutputStream;
            using (var inputStream = File.OpenRead(_filePath))
            {
                byte[] buffer = new byte[BufferSize];
                int bytesRead;

                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}