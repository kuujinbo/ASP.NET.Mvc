using System;
using System.IO;
using System.Web;

namespace kuujinbo.Mvc.NET.IO
{
    public interface IFileUploadStore
    {
        bool Save(
            HttpPostedFileBase upload, 
            Uri basePath,
            string fileNameWithoutExtension
        );
    }

    /// <summary>
    /// Simple file upload file-system store.
    /// </summary>
    public class FileUploadStore : IFileUploadStore
    {
        /// <summary>
        /// Save file upload to file system; return true if saved, else false
        /// </summary>
        public virtual bool Save(
            HttpPostedFileBase upload, 
            Uri basePath,
            string fileNameWithoutExtension = null)
        {
            if (upload != null && upload.ContentLength > 0)
            {
                var filename = string.IsNullOrWhiteSpace(fileNameWithoutExtension)
                    ? Path.GetFileName(upload.FileName)
                    : string.Format(
                          "{0}{1}",
                          fileNameWithoutExtension, 
                          Path.GetExtension(upload.FileName)
                      );

                upload.SaveAs(Path.Combine(basePath.LocalPath, filename));

                return true;
            }

            return false;
        }
    }
}