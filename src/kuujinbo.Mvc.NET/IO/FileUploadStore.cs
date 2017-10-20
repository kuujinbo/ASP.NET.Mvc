using System;
using System.IO;
using System.Web;

namespace kuujinbo.Mvc.NET.IO
{
    public interface IFileUploadStore
    {
        bool Save(HttpPostedFileBase upload);
    }

    /// <summary>
    /// Simple file upload file-system store.
    /// </summary>
    public class FileUploadStore : IFileUploadStore
    {
        /// <summary>
        /// Base path to store the file upload
        /// </summary>
        public Uri BasePath { get; private set; }

        /// <summary>
        /// Optional file name **WITHOUT** extension
        /// </summary>
        public string FileNameWithoutExtension { get; private set; }

        /// <summary>
        /// Initialize new instance; parameterless constructor **NOT** defined.
        /// </summary>
        public FileUploadStore(
            Uri basePath, 
            string fileNameWithoutExtension = null)
        {
            BasePath = basePath;
            FileNameWithoutExtension = fileNameWithoutExtension;
        }

        /// <summary>
        /// Save file upload to file system; return true if saved, else false
        /// </summary>
        public bool Save(HttpPostedFileBase upload)
        {
            if (upload != null && upload.ContentLength > 0)
            {
                var filename = string.IsNullOrWhiteSpace(FileNameWithoutExtension)
                    ? Path.GetFileName(upload.FileName)
                    : string.Format(
                          "{0}{1}", 
                          FileNameWithoutExtension, 
                          Path.GetExtension(upload.FileName)
                      );

                upload.SaveAs(Path.Combine(BasePath.LocalPath, filename));

                return true;
            }

            return false;
        }
    }
}