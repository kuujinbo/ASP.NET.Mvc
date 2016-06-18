using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class WebAPIAjaxFileController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(
                File.ReadAllBytes(
                    System.Web.Hosting.HostingEnvironment.MapPath(
                        "~/app_data/hello-world.pdf"
                    )
                )
            );
            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "test.pdf"
                };
            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            return response;
        }
    }
}