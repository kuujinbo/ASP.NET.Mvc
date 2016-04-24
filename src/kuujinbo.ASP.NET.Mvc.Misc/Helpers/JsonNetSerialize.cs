using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Web.Mvc
{
    public class JsonNet
    {
        public static string Serialize(object o)
        {
            return JsonConvert.SerializeObject(
                o, 
                Formatting.Indented, 
                new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" }
            );
        }
    }
}