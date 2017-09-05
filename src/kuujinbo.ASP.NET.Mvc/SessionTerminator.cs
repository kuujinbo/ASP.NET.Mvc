using System;
using System.Web;
using System.Web.Mvc;
/* ############################################################################
 * terminate / logout user
 * ############################################################################
 */
using System.Web.Routing;

namespace kuujinbo.ASP.NET.Mvc
{
    public interface ISessionTerminator
    {
        int GetTimeout(bool isPrivilegedUser);

        void Logout(HttpRequestBase request, HttpResponseBase response, TempDataDictionary tempData);
    }

    public class SessionTerminator : ISessionTerminator
    {
        /// <summary>
        /// timeout intervals per DISA Application STIG 
        /// </summary>
        public static readonly int PrivilegedTimeout = 60 * 10;

        /// <summary>
        /// timeout intervals per DISA Application STIG 
        /// </summary>
        public static readonly int NonPrivilegedTimeout = 60 * 15;

        /// <summary>
        /// Ignore inactivity / idle timout for specific controller/action
        /// </summary>
        public const string IGNORE_SESSION_TIMEOUT = "ignore-session-timeout";

        /// <summary>
        /// Ignore inactivity / idle timout for specific controller/action
        /// </summary>
        public const string SESSION_TIMED_OUT = "session-timed-out";


        public int GetTimeout(bool isPrivilegedUser = false)
        {
            return isPrivilegedUser ? PrivilegedTimeout : NonPrivilegedTimeout;
        }

        /// <summary>
        /// TODO: database logging
        /// </summary>
        /// <remarks>
        /// **ANY** querystring key/value pair in route value object will 
        /// **NOT** display the modal inactivity logged out message.
        /// </remarks>
        /// <example>
        /// [1] User explcitly clicks 'Logout' button / link - modal **NOT** displayed:
        ///     @Url.Action("Logout", "NoticeAndConsent", new { logoutClick = true})
        /// [2] Default inactivity timeout with no querystring - modal **IS** displayed:
        ///     @Url.Action("Logout", "NoticeAndConsent")'
        /// 
        /// </example>
        public void Logout(HttpRequestBase request, 
            HttpResponseBase response,
            TempDataDictionary tempData = null)
        {
            var cookie = request.Cookies["DOD_NOTICE_CONSENT"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                response.SetCookie(cookie);
            }

            // tempData flags whether to display the modal inactivity message
            if (request.QueryString.Count < 1) tempData[SESSION_TIMED_OUT] = true;
        }
    }
}