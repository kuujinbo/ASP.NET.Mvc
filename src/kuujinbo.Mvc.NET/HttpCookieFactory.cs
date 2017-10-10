using System;
using System.Web;

namespace kuujinbo.Mvc.NET
{
    public static class HttpCookieFactory
    {
        /// <summary>
        /// Exception message
        /// </summary>
        public const string InvalidCreateParameter = "name";

        /// <summary>
        /// Create a HTTP cookie that by default:
        /// [1] can only be transmitted over an encrypted connection
        /// [2] cannot be accessed through client side script
        /// </summary>
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