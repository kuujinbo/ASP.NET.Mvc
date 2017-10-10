using System;
using System.Web;

namespace kuujinbo.Mvc.NET
{
    public interface IClientCertificate
    {
        byte[] GetCertificate(HttpRequestBase request);
    }

    public class ClientCertificate : IClientCertificate
    {
        /// <summary>
        /// BIG-IP stores user client certificate in custom header
        /// </summary>
        public const string BigIpCertificateHeader = "ssl.client_cert";
        
        /// <summary>
        /// Get the user/client certificate for the current HTTP request
        /// </summary>
        public virtual byte[] GetCertificate(HttpRequestBase request)
        {
            return request.IsLocal
                ? request.ClientCertificate.Certificate
                : Convert.FromBase64String(request.Headers[BigIpCertificateHeader]);
        }
    }
}