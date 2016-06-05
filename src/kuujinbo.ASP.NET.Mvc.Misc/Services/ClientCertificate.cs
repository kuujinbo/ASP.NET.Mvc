/* ===========================================================================
 * IIS CRL check
 * https://blogs.msdn.microsoft.com/kaushal/2012/10/15/disable-client-certificate-revocation-crl-check-on-iis/
 * ===========================================================================
 */
using System;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services
{
    /// <summary>
    /// wrapper for tests
    /// </summary>
    public interface IClientCertificate
    {
        byte[] Get(HttpRequestBase request);
    }

    public class ClientCertificate : IClientCertificate
    {
        /// <summary>
        /// specific to current 'corporate' network environment. BIG-IP does
        /// **NOT** allow direct access to HttpRequestBase.ClientCertificate
        /// </summary>
        public const string CERT_HEADER = "ssl.client_cert";

        public virtual byte[] Get(HttpRequestBase request)
        {
            return request.IsLocal 
                ? request.ClientCertificate.Certificate
                : Convert.FromBase64String(request.Headers[CERT_HEADER]);
        }
    }
}