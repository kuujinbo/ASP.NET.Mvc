using System;
using System.IO;
using System.Threading;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class HomeController : Controller
    {
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

    }


    public interface IFileWriterUtility
    {
        bool WriteChunk(int? chunk, int chunks, string name, Stream stream);
    }

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