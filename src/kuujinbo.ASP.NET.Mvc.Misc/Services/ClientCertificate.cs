using System;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services
{
    public interface IClientCertificate
    {
        byte[] Get(HttpRequestBase request);
    }

    public class ClientCertificate : IClientCertificate
    {
        public const string CERT_HEADER = "ssl.client_cert";

        public byte[] Get(HttpRequestBase request)
        {
            return request.IsLocal 
                ? request.ClientCertificate.Certificate
                : Convert.FromBase64String(request.Headers[CERT_HEADER]);
        }
    }
}