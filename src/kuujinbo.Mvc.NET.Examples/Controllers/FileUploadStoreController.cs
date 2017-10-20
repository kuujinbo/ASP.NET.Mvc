using kuujinbo.Mvc.NET.Examples.Models;
using kuujinbo.Mvc.NET.IO;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.Examples.Controllers
{
    public class FileUploadStoreController : Controller
    {
        private readonly IFileUploadStore _uploadStore;

        public FileUploadStoreController(IFileUploadStore uploadStore)
        {
            _uploadStore = uploadStore;
        }

       [HttpPost]
        public ActionResult Upload(TestModel model, HttpPostedFileBase fileUploadField)
        {
           _uploadStore.Save(
               fileUploadField, 
               new Uri(Path.Combine( 
                   Server.MapPath("~/app_data"),
                   Path.GetFileName(fileUploadField.FileName)
               )),
                null
           );

           return new EmptyResult();
       }
    }
}