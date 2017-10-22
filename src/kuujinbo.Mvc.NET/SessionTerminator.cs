using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Attributes;

namespace kuujinbo.Mvc.NET
{
    public interface ISessionTerminator
    {
        int GetTimeout(bool isPrivilegedUser);

        void Logout(HttpRequestBase request, HttpResponseBase response, TempDataDictionary tempData);
    }

    /// <summary>
    /// DISA Application STIG user inactivity session terminator
    /// </summary>
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
        /// Ignore inactivity / idle timout for specific controller/action.
        /// See <see cref="kuujinbo.Mvc.NET.Attributes.SessionTerminatorIgnoreAttribute" />
        /// for more information.
        /// </summary>
        public const string IgnoreSessionTimeout = "ignore-session-timeout";

        /// <summary>
        /// Flag session timeout. See <see cref="Logout" />
        /// </summary>
        public const string SessionTimedOut = "session-timed-out";


        public int GetTimeout(bool isPrivilegedUser = false)
        {
            return isPrivilegedUser ? PrivilegedTimeout : NonPrivilegedTimeout;
        }

        /// <summary>
        /// Logout - **see remarks and example XML documentation**.
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
            HttpCookieFactory.ExpireCookie(
                response, 
                NoticeAndConsentAuthorizeAttribute.NoticeAndConsent
            );

            // tempData flags whether to display the modal inactivity message
            if (request.QueryString.Count < 1) tempData[SessionTimedOut] = true;
        }
    }
}