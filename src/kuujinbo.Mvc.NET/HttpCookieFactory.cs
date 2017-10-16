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
        /// Number of days in past to set Expires header
        /// </summary>
        public const int ExpiresDays = -4;

        /// <summary>
        /// Set a **SESSION** HttpCookie with the given key/value name pair. 
        /// See also Create() method.
        /// </summary>
        public static void SetCookie(
            HttpResponseBase response,
            string name,
            string value = null,
            bool httpOnly = true,
            bool secure = true)
        {
            response.SetCookie(Create(name, value, httpOnly, secure));
        }

        /// <summary>
        /// Expire a HttpCookie with the given key name. 
        /// See also Create() method.
        /// </summary>
        public static void ExpireCookie(HttpResponseBase response, string name)
        {
            var cookie = Create(name);
            cookie.Expires = DateTime.UtcNow.AddDays(ExpiresDays);
            response.SetCookie(cookie);
        }

        /// <summary>
        /// Create a HTTP **SESSION** cookie that by default:
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