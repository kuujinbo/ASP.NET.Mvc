using System;
using System.IO;
using System.Web;

namespace kuujinbo.Mvc.NET.IO
{
    /// <summary>
    /// Simple file upload file-system store.
    /// </summary>
    public abstract class FileUploadStore
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
        protected FileUploadStore(
            Uri basePath, 
            string fileNameWithoutExtension = null)
        {
            BasePath = basePath;
            FileNameWithoutExtension = fileNameWithoutExtension;
        }

        /// <summary>
        /// Save file upload to file system; return true if saved, else false
        /// </summary>
        public virtual bool Save(HttpPostedFileBase upload)
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