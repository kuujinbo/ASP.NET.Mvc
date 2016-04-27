using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Web.Mvc
{
    public class JsonNetSerializer
    {
        public static string Get(object o)
        {
            return JsonConvert.SerializeObject(
                o, 
                Formatting.Indented, 
                new IsoDateTimeConverter() { DateTimeFormat = "M/d/yyyy" }
            );
        }
    }
}