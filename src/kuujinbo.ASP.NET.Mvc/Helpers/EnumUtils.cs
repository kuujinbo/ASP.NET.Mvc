using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace kuujinbo.ASP.NET.Mvc.Helpers
{
    public static class EnumUtils
    {
        public static string DisplayText(object value)
        {
            var name = value.GetType()
                .GetMember(value.ToString())[0]
                .GetCustomAttribute<DisplayAttribute>();

            return name != null
                ? name.GetName()
                : RegexUtils.PascalCaseSplit(value.ToString());
        }
    }
}