using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    /// <summary>
    /// Strange, but true: some developers think it's OK to display true and
    /// false in their UIs....
    /// </summary>
    public class UserFriendlyBool
    {
        public const string UserFriendlyTrue = "Yes";
        public const string UserFriendlyFalse = "No";

        public string TrueValue { get; private set; }
        public string FalseValue { get; private set; }

        /// <summary>
        /// Convert boolean to a user-friendly dsiplayable value.
        /// </summary>
        public UserFriendlyBool(
            string trueValue = UserFriendlyTrue, 
            string falseValue = UserFriendlyFalse)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        /// <summary>
        /// Convert boolean to a human-readable valiu
        /// </summary>
        public MvcHtmlString Convert(bool value)
        {
            return new MvcHtmlString(value ? TrueValue : FalseValue);
        }

        /// <summary>
        /// Convert boolean to a human-readable valiu
        /// </summary>
        public MvcHtmlString Convert(bool? value)
        {
            return new MvcHtmlString(value.HasValue && value.Value ? TrueValue : FalseValue);
        }
    }
}