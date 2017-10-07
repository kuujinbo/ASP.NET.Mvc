using System;
using System.Web;

namespace kuujinbo.Mvc.NET
{
    public static class HttpCookieFactory
    {
        public const string KeyName = "_http-cookie";

        public const string InvalidCreateParameter = "name";

        /// <summary>
        /// redirect to page requested instead of default route. e.g. bookmark
        /// </summary>
        public static readonly string ReturnUrl = KeyName + "-returnUrl";

        public static HttpCookie Create(
            string name,
            string value = null,
            bool httpOnly = true,
            bool secure = true)
        {

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(InvalidCreateParameter);

            return new HttpCookie(name, value) { HttpOnly = httpOnly, Secure = secure };
        }
    }
}