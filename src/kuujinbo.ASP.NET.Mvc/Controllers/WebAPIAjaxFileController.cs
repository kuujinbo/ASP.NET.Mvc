using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class WebAPIAjaxFileController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var file = HostingEnvironment.MapPath("~/app_data/hello-world.pdf");
            var response = Request.CreateResponse(HttpStatusCode.OK);
            var stream = File.OpenRead(file);
            response.Content = new StreamContent(stream, 8192);
            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "test.pdf"
                };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(
                MimeMapping.GetMimeMapping(Path.GetExtension(file))
            );

            return response;
        }
    }
}