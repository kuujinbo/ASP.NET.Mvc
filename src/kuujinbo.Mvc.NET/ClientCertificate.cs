using System;
using System.IdentityModel.Selectors;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Web;

[assembly: InternalsVisibleTo("kuujinbo.Mvc.NET.Tests")]
namespace kuujinbo.Mvc.NET
{
    public interface IClientCertificate
    {
        CacUser GetCacUser(HttpRequestBase request, bool validateChain);
    }

    public class ClientCertificate : IClientCertificate
    {
        //public HttpRequestBase Request { get; private set; }

        //public ClientCertificate(HttpRequestBase request)
        //{
        //    Request = request;
        //}

        /// <summary>
        /// BIG-IP stores user client certificate in custom header
        /// </summary>
        public const string BigIpCertificateHeader = "ssl.client_cert";
        
        /// <summary>
        /// Get the user/client certificate for the current HTTP request
        /// </summary>
        internal byte[] GetCertificate(HttpRequestBase request)
        {
            return request.IsLocal
                ? request.ClientCertificate.Certificate
                : Convert.FromBase64String(request.Headers[BigIpCertificateHeader]);
        }

        /// <summary>
        /// Get a CacUser
        /// </summary>
        /// <usage>
        /// https://github.com/kuujinbo/Mvc.NET/blob/master/src/kuujinbo.Mvc.NET.Examples/Controllers/CacUserController.cs
        /// </usage>
        public virtual CacUser GetCacUser(HttpRequestBase request, bool validateChain = false)
        {
            X509Certificate2 cert = new X509Certificate2(GetCertificate(request));

            var subjectName = cert.GetNameInfo(X509NameType.SimpleName, false);
            var cacUser = CacUser.Create(subjectName);
            cacUser.Subject = subjectName;

            cacUser.Email = cert.GetNameInfo(X509NameType.EmailName, false)
                                .ToLower();

            if (validateChain) ValidateChain(cert, cacUser);

            return cacUser;
        }

        void ValidateChain(X509Certificate2 cert, CacUser cacUser)
        {
            // **ALL** validation flags turned on
            var validator = X509CertificateValidator.ChainTrust;
            try
            {
                validator.Validate(cert);
            }
            catch (Exception e)
            {
                cacUser.ChainError = e.Message;
            }
        }
    }
}