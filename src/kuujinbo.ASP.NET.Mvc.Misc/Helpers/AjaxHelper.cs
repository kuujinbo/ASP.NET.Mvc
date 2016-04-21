namespace System.Web.Mvc
{
    public static class AjaxHelper
    {
        public const string CSRF_TOKEN = "__RequestVerificationToken";
        public const string AJAX_HEADER_NAME = "X-Requested-With";
        public const string AJAX_HEADER_VALUE = "XMLHttpRequest";

        public static string AntiForgeryHeader(HttpRequestBase request)
        {
            return request.Headers[CSRF_TOKEN];
        }
    }
}