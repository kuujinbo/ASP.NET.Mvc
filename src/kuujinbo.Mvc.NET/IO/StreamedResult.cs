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

        /// <summary>
        /// Exception message
        /// </summary>
        public const string InvalidPathParameter = "path";
        
        /// <summary>
        /// Exception message
        /// </summary>
        public const string InvalidContentTypeParameter = "contentType";

        /// <summary>
        /// Default read / write buffer size
        /// </summary>
        public const int DefaultBufferSize = 4096;

        /// <summary>
        /// Validate constructor 'contentType' parameter 
        /// </summary>
        public static readonly Regex WhiteSpace = new Regex(@"^\s+$");

        /// <summary>
        /// Read / write buffer size
        /// </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// Initialize new instance.
        /// </summary>
        public StreamedResult(
            string path,
            string contentType,
            int bufferSize = DefaultBufferSize) 
        : base(contentType)
        {
            if (string.IsNullOrWhiteSpace(path)) 
                throw new ArgumentException(InvalidPathParameter);
            // FileResult throws ArgumentException => string.IsNullOrEmpty(contentType)
            if (WhiteSpace.IsMatch(contentType)) 
                throw new ArgumentException(InvalidContentTypeParameter);

            this.FileDownloadName = Path.GetFileName(path);
            BufferSize = bufferSize;
            _filePath = path;
        }

        /// <summary>
        /// Send streamed / buffered file to client.
        /// </summary>
        protected override void WriteFile(HttpResponseBase response)
        {
            using (var inputStream = File.OpenRead(_filePath))
            {
                byte[] buffer = new byte[BufferSize];
                int bytesRead;

                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    response.OutputStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}