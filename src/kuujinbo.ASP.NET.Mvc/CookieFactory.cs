using System;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc
{
    public static class CookieFactory
    {
        public const string BAD_COOKIE_NAME = "name";

        /// <summary>
        /// flag that user acknowledged application entry notice
        /// </summary>
        public const string DOD_NOTICE_CONSENT = "DOD_NOTICE_CONSENT";

        /// <summary>
        /// user last login date/time
        /// </summary>
        public const string LAST_LOGIN_DATE = "last-login-date";
        public const string LAST_LOGIN_DATEFORMAT = "M/d/yyyy HH:mm";

        /// <summary>
        /// user last login location (IP address)
        /// </summary>
        public const string LAST_LOGIN_LOCATION = "last-login-location";

        /// <summary>
        /// redirect to page requested instead of default route. e.g. bookmark
        /// </summary>
        public const string RETURN_URL = "returnUrl";

        public static HttpCookie Create(
            string name,
            string value = null,
            bool httpOnly = true,
            bool secure = true)
        {

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(BAD_COOKIE_NAME);

            return new HttpCookie(name, value) { HttpOnly = httpOnly, Secure = secure };
        }
    }
}