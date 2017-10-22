using kuujinbo.Mvc.NET.Examples.Models;
using kuujinbo.Mvc.NET.IO;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace kuujinbo.Mvc.NET.Examples.Controllers
{
    public class FileUploadStoreController : Controller
    {
        private readonly IFileUploadStore _uploadStore;

        public FileUploadStoreController(IFileUploadStore uploadStore)
        {
            _uploadStore = uploadStore;
        }

        [ExcludeFromCodeCoverage]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(TestModel model, HttpPostedFileBase fileUploadField)
        {
            _uploadStore.Save(
                fileUploadField,
                new Uri(Server.MapPath("~/app_data")),
                 null
            );

            TempData["Success"] = "File uploaded";

            return RedirectToAction("Index");
        }
    }
}