using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace kuujinbo.ASP.NET.Mvc.Misc.Controllers
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

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}