using System;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc
{
    public interface IClientCertificate
    {
        byte[] Get(HttpRequestBase request);
    }

    public class ClientCertificate : IClientCertificate
    {
        /// <summary>
        /// BIG-IP stores user/client certificate in custom header
        /// </summary>
        public const string BIG_IP_CERTIFICATE_HEADER = "ssl.client_cert";

        /// <summary>
        /// Get the user/client certificate for the current HTTP request
        /// </summary>
        public virtual byte[] Get(HttpRequestBase request)
        {
            return request.IsLocal
                ? request.ClientCertificate.Certificate
                : Convert.FromBase64String(request.Headers[BIG_IP_CERTIFICATE_HEADER]);
        }
    }
}