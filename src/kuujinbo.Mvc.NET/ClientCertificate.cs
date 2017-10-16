using System;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace kuujinbo.Mvc.NET
{
    public interface IClientCertificate
    {
        byte[] GetCertificate();
        string GetSubjectName();
        CacUser GetCacUser();
    }

    public class ClientCertificate : IClientCertificate
    {
        public HttpRequestBase Request { get; private set; }

        public ClientCertificate(HttpRequestBase request)
        {
            Request = request;
        }

        /// <summary>
        /// BIG-IP stores user client certificate in custom header
        /// </summary>
        public const string BigIpCertificateHeader = "ssl.client_cert";
        
        /// <summary>
        /// Get the user/client certificate for the current HTTP request
        /// </summary>
        public virtual byte[] GetCertificate()
        {
            return Request.IsLocal
                ? Request.ClientCertificate.Certificate
                : Convert.FromBase64String(Request.Headers[BigIpCertificateHeader]);
        }

        /// <summary>
        /// Get the subject name from a certificate.
        /// </summary>
        public virtual string GetSubjectName()
        {
            return new X509Certificate2(GetCertificate())
                .GetNameInfo(X509NameType.SimpleName, false);
        }

        /// <summary>
        /// Get a CacUser
        /// </summary>
        public virtual CacUser GetCacUser()
        {
            X509Certificate2 cert = new X509Certificate2(GetCertificate());
            var cacUser = CacUser.Create(cert.GetNameInfo(X509NameType.SimpleName, false));
            cacUser.Email = cert.GetNameInfo(X509NameType.EmailName, false)
                                .ToLower();

            return cacUser;
        }
    }
}