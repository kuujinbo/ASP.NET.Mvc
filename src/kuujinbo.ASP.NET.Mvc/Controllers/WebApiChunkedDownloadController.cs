using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class WebApiChunkedDownloadController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                var file = HostingEnvironment.MapPath("~/app_data/moby-dick.pdf");
                response.Content = new StreamContent(File.OpenRead(file), 8192);
                response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(file)
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(
                    MimeMapping.GetMimeMapping(Path.GetExtension(file))
                );
            }
            catch
            {
                response.StatusCode = HttpStatusCode.Moved;
                // response.StatusCode = HttpStatusCode.Unauthorized;
                response.Headers.Location = new Uri(
                    new Uri(Request.RequestUri.GetLeftPart(UriPartial.Authority)),
                    ""
                );
            }
            return response;
        }
    }
}