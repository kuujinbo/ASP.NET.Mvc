/* ============================================================================
 * json that System.Web.Script.Serialization.JavaScriptSerializer can't
 * deal with - i.e. JavaScript dates.
 * ----------------------------------------------------------------------------
 * USAGE - in MVC controller action:
 *      return new JsonNetResult(OBJECT);
 * ============================================================================
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Web.Mvc
{
    public class JsonNetResult : ContentResult
    {
        public object Data { get; private set; }
        public string DateFormat { get; private set; }
        public JsonNetResult(object data, string dateFormat = null)
        {
            if (data == null) throw new ArgumentNullException("data");

            Data = data;
            DateFormat = dateFormat;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(JsonNetSerializer.Get(Data, DateFormat));
        }
    }
}