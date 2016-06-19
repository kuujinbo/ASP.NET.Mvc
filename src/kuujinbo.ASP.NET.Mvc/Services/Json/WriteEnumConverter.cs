/* =======================================================================
 * make enums more end-user friendly
 * =======================================================================
 */
using kuujinbo.ASP.NET.Mvc.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace kuujinbo.ASP.NET.Mvc.Services.Json
{
    public class WriteEnumConverter : StringEnumConverter
    {
        public override void WriteJson(
            JsonWriter writer, 
            object value, 
            JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(RegexUtils.PascalCaseSplit(value.ToString()));
            }
        }
    }
}