using System;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc
{
    public static class CookieFactory
    {
        public static readonly string KEY_NAME = typeof(CookieFactory).ToString();


        public const string BAD_COOKIE_NAME = "name";

        /// <summary>
        /// flag that user acknowledged application entry notice
        /// </summary>
        public static readonly string NOTICE_AND_CONSENT = KEY_NAME + "-NOTICE_AND_CONSENT";

        /// <summary>
        /// user last login date/time
        /// </summary>
        public static readonly string LAST_LOGIN_DATE = KEY_NAME + "-last-login-date";
        public const string LAST_LOGIN_DATEFORMAT = "M/d/yyyy HH:mm";

        /// <summary>
        /// user last login location (IP address)
        /// </summary>
        public static readonly string LAST_LOGIN_LOCATION = KEY_NAME + "-last-login-location";

        /// <summary>
        /// redirect to page requested instead of default route. e.g. bookmark
        /// </summary>
        public static readonly string RETURN_URL = KEY_NAME + "-returnUrl";

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