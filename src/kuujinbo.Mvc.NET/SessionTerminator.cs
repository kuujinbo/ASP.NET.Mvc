using System.Web;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Attributes;
using kuujinbo.Mvc.NET.HtmlHelpers;

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
        /// Application logout may occur in one of two ways:
        /// [1] Client-side inactivity timeout logout. See 
        /// <see cref="kuujinbo.Mvc.NET.HtmlHelpers.SessionTerminatorHelper.TerminateSession" />.
        /// ControllerBase.TempData is used to flag a client-side modal
        /// message, so the 'Logout' or other named controller action MUST
        /// ALWAYS RETURN A REDIRECT ActionResult.
        /// 
        /// [2] Explicit user logout. I.e., user clicks on button/hyperlink.
        /// </summary>
        public void Logout(HttpRequestBase request, 
            HttpResponseBase response,
            TempDataDictionary tempData = null)
        {
            HttpCookieFactory.ExpireCookie(
                response, 
                NoticeAndConsentAuthorizeAttribute.NoticeAndConsent
            );

            // tempData flags whether to display the modal inactivity message,
            // so the controller action MUST ALWAYS RETURN A REDIRECT ActionResult.
            if (request.QueryString[SessionTerminatorHelper.ShowClientModalKey] != null) 
                tempData[SessionTimedOut] = true;
        }
    }
}