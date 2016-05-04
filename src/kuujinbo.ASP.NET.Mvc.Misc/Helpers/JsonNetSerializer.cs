using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Web.Mvc
{
    public class JsonNetSerializer
    {
        public static string Get(object o)
        {
            if (o == null) throw new System.ArgumentNullException("o");

            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "M/d/yyyy" });

            return JsonConvert.SerializeObject(o, Formatting.Indented, settings);
        }
    }
}