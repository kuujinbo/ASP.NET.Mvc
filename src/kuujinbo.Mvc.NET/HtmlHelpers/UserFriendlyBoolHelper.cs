using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    public static class UserFriendlyBoolHelper
    {
        public const string UserFriendlyTrue = "Yes";
        public const string UserFriendlyFalse = "No";

        public static MvcHtmlString UserFriendlyBool(this HtmlHelper helper, bool value)
        {
            return new MvcHtmlString(value ? UserFriendlyTrue : UserFriendlyFalse);
        }

        public static MvcHtmlString UserFriendlyBool(this HtmlHelper helper, bool? value)
        {
            return new MvcHtmlString(value.HasValue && value.Value ? UserFriendlyTrue : UserFriendlyFalse);
        }
    }
}