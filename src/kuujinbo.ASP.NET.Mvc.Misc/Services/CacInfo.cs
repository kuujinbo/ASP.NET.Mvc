using System;
using System.Security.Cryptography.X509Certificates;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services
{
    public class CacInfo
    {
        public const string NULL_GET_PARAM = "rawData";

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Edipi { get; set; }
        public string Email { get; set; }

        public static CacInfo Get(byte[] rawData)
        {
            if (rawData == null) throw new ArgumentNullException(NULL_GET_PARAM);

            X509Certificate2 cert = new X509Certificate2(rawData);
            var cacInfo = ParseSimpleName(cert.GetNameInfo(X509NameType.SimpleName, false));
            cacInfo.Email = cert.GetNameInfo(X509NameType.EmailName, false);

            return cacInfo;
        }

        private static CacInfo ParseSimpleName(string simpleName)
        {
            string[] splitValue = simpleName.Split(new char[] { '.' });
            int lastIndex = splitValue.Length - 1;
            return new CacInfo()
            { 
                LastName = splitValue[0], FirstName = splitValue[1], 
                Edipi = splitValue[lastIndex] 
            };
        }
    }
}