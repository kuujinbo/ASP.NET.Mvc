using System;
using System.Web;

namespace kuujinbo.Mvc.NET.Helpers
{
    public static class HttpRequestBaseExtension
    {
        /// <summary>
        /// Get the web site's absolute URL, with optional path information
        /// </summary>
        public static string GetAbsoluteUrl(
            this HttpRequestBase request, 
            string relativeUrl = null)
        {
            var baseUri = new Uri(
                new Uri(request.Url.GetLeftPart(UriPartial.Authority)), 
                request.ApplicationPath == "/"
                    ? "/"
                    : string.Format("/{0}/", request.ApplicationPath.Trim('/'))
            );

            return relativeUrl == null
                ? baseUri.ToString()
                : new Uri(baseUri, relativeUrl.Trim('/')).ToString();
        }

        /// <summary>
        /// Append key/value pair to get a properly formatted query string
        /// </summary>
        public static string AppendQueryString(
            this HttpRequestBase request, 
            string relativePath,
            string key,
            string value = null)
        {
            var baseUrl = request.Url.GetLeftPart(UriPartial.Authority);
            var fullUri = new Uri(new Uri(baseUrl), relativePath);
            var uriBuilder = new UriBuilder(fullUri);
            var nameValueCollection = HttpUtility.ParseQueryString(uriBuilder.Query);
            nameValueCollection[key] = value ?? "0";
            uriBuilder.Query = nameValueCollection.ToString();

            return uriBuilder.Uri.PathAndQuery;
        }
    }
}