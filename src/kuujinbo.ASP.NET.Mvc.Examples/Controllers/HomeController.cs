using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Examples.Models;
using System.Collections.Generic;
using System.Web.Hosting;
using Newtonsoft.Json;

namespace kuujinbo.ASP.NET.Mvc.Examples.Controllers
{
    [ExcludeFromCodeCoverage]
    public class HomeController : Controller
    {
        static readonly ICollection<TestModel> _data;

        static HomeController()
        {
            _data = JsonConvert.DeserializeObject<ICollection<TestModel>>(
                System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/app_data/jsonData.json"))
            );
        }

        public ActionResult SearchUsers(string searchText)
        {
            var users = from user in _data
                        where user.Name.StartsWith(
                            searchText, StringComparison.OrdinalIgnoreCase
                        )
                        select user;

            var result = new List<dynamic>();
            foreach (var user in users)
            {
                result.Add(new
                {
                    label = user.Name
                    , value = user.Id,
                });

            }

            Thread.Sleep(1000);
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }



        FileWriterUtility _fu = new FileWriterUtility();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpAjaxPost]
        public ActionResult Index(int? chunk, int chunks, string name)
        {
            var completed = _fu.WriteChunk(
                chunk, chunks,
                Path.Combine(Server.MapPath("~/App_Data"), name),
                Request.Files[0].InputStream
            );
            return Json(completed ? 1 : 0);
        }

        [HttpGet]
        public ActionResult XhrGET(string id)
        {
            return Json(
                new { method = id, url = Request.Url, date = DateTime.Now.ToString() }, 
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpAjaxPost]
        public ActionResult XhrPOST(string method)
        {
            return Json(new { method = method, url = Request.Url, date = DateTime.Now.ToString() });
        }

        [HttpDelete]
        public ActionResult XhrDELETE(string id)
        {
            return Json(new { method = id, url = Request.Url, date = DateTime.Now.ToString() });
        }

        [HttpPut]
        public ActionResult XhrPUT(string id)
        {
            return Json(new { method = id, url = Request.Url, date = DateTime.Now.ToString() });
        }

        [HttpPost]
        // [HttpAjaxPost]
        public ActionResult Post()
        {
            TempData["result"] = string.Format(
                "POST: <b>with</b> AntiForgeryToken. URL: {0}", Request.Url
            );
            return Redirect("~/");
        }

        [HttpPost]
        [IgnoreXsrfFilter]
        public ActionResult PostIgnoreXsrfFilter()
        {
            TempData["resultPostIgnoreXsrfFilter"] = string.Format(
                "POST: <b>ignore</b> AntiForgeryToken. URL: {0}", Request.Url
            );
            return Redirect("~/");
        }
        
        [ValidateJsonAntiForgeryToken]
        [HttpPost]
        public ActionResult JsonAntiForgery(TestModel testModel)
        {
            return new JsonResult
            {
                Data = string.Format("data: {0} HTTP response @{1}", testModel.Name, DateTime.Now)
            };
        }

        [HttpPost]
        public ActionResult Upload(TestModel model, HttpPostedFileBase fileUploadField)
        {
            if (ModelState.IsValid)
            {
                if (fileUploadField != null 
                    && fileUploadField.ContentLength > 0)
                {
                    fileUploadField.SaveAs(Path.Combine(
                        Server.MapPath("~/app_data"),
                        Path.GetFileName(fileUploadField.FileName)
                    ));
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View("Index", model);
            }
            /*
             * #############################################################
             * if fluentvalidation.mvc not used/registered
             * #############################################################
            TestModelValidator validator = new TestModelValidator();
            var result = validator.Validate(model);
            if (result.IsValid)
            if (ModelState.IsValid) 
            {
                return View(model);
            }
            else {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("Index", model); 
            }
             */
        }
    }

    public interface IFileWriterUtility
    {
        bool WriteChunk(int? chunk, int chunks, string name, Stream stream);
    }

    [ExcludeFromCodeCoverage]
    public class FileWriterUtility : IFileWriterUtility
    {
        public bool WriteChunk(int? chunk, int chunks, string localPath, Stream stream)
        {
            Thread.Sleep(1000);
            var complete = false;
            chunk = chunk ?? 0;
            using (var fs = new FileStream(
                localPath,
                chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var ba = new byte[stream.Length];
                stream.Read(ba, 0, ba.Length);
                fs.Write(ba, 0, ba.Length);
            }
            complete = chunk == chunks - 1;
            return complete;
        }

        //public string GenerateHash(Stream stream, HashAlgorithm algorithm)
        //{
        //    return BitConverter.ToString(algorithm.ComputeHash(stream))
        //        .Replace("-", "")
        //        .ToLower();
        //}
    }
}