using System;
using System.Web;

namespace kuujinbo.Mvc.NET.Helpers
{
    public static class HttpRequestBaseExtension
    {
        public static string AppendQueryString(
            this HttpRequestBase request, 
            string relativePath,
            string key,
            string value = null)
        {
            var baseUrl = request.Url.GetLeftPart(System.UriPartial.Authority);
            var fullUri = new Uri(new Uri(baseUrl), relativePath);
            var uriBuilder = new UriBuilder(fullUri);
            var nameValueCollection = HttpUtility.ParseQueryString(uriBuilder.Query);
            nameValueCollection[key] = value ?? "0";
            uriBuilder.Query = nameValueCollection.ToString();

            return uriBuilder.Uri.PathAndQuery;
        }
    }
}