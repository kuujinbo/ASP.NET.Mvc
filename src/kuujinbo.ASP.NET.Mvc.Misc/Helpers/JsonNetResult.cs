using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Web.Mvc
{
    public class JsonNetResult : ContentResult
    {
		public object Data { get; private set; } 
        public JsonNetResult(object data)
        {
            if (data == null) { throw new ArgumentNullException("data"); }
            Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }

			HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
			if (this.ContentEncoding != null)
			{
				response.ContentEncoding = this.ContentEncoding;
			}

			response.Write(JsonNet.Serialize(Data));
        }
    }
}