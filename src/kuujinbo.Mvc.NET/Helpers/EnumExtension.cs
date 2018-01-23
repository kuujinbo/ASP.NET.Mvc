using System;
using System.Linq;
using System.Reflection;

namespace kuujinbo.Mvc.NET.Helpers
{
    public static class EnumExtension
    {
        /// <summary>
        /// Get the custom attribute value associated to an enum.
        /// </summary>
        public static TAttribute GetAttributeValue<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            return value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
        }
    }
}
