using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services
{
    public interface ICacInfo
    {
        CacInfo Get(byte[] rawData);
    }

    public class CacInfo : ICacInfo
    {
        public const string NULL_GET_PARAM = "rawData";
        public const string BAD_EDIPI = "edipi not 10 digits";
        public const string BAD_SIMPLE_NAME = "simpleName";
        public const string BAD_TITLE_CASE_TEXT = "text cannot be null or whitespace";

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Edipi { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// get CacInfo from X509Certificate2.GetNameInfo():
        /// [1] X509NameType.SimpleName
        /// [2] X509NameType.EmailName
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns>CacInfo</returns>
        /// <remarks>
        /// X509Certificate2.GetNameInfo() returns empty string if not found,
        /// **not** null
        /// </remarks>
        public CacInfo Get(byte[] rawData)
        {
            if (rawData == null) throw new ArgumentNullException(NULL_GET_PARAM);

            X509Certificate2 cert = new X509Certificate2(rawData);
            var cacInfo = GetSimpleName(cert.GetNameInfo(X509NameType.SimpleName, false));
            cacInfo.Email = cert.GetNameInfo(X509NameType.EmailName, false)
                .ToLower();

            return cacInfo;
        }

        /// <summary>
        /// get CacInfo from X509Certificate2.GetNameInfo() [X509NameType.SimpleName]
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns>CacInfo</returns>
        /// <remarks>
        /// at a minimum input parameter is expected to be last.first.edipi
        /// </remarks>
        public static CacInfo GetSimpleName(string simpleName)
        {
            string[] splitValue = simpleName.Split(new char[] { '.' });
            if (splitValue.Length < 3) throw new FormatException(BAD_SIMPLE_NAME);

            var edipi = splitValue[splitValue.Length - 1];
            if (!ValidEdipi(edipi)) throw new FormatException(BAD_EDIPI);

            return new CacInfo()
            {
                LastName = TitleCase(splitValue[0]),
                FirstName = TitleCase(splitValue[1]),
                Edipi = edipi
            };
        }

        /// <summary>
        /// verify Edipi is ten digit number
        /// </summary>
        /// <param name="edipi"></param>
        /// <returns>bool</returns>
        public static bool ValidEdipi(string edipi)
        {
            return Regex.IsMatch(edipi, @"^\d{10}$");
        }

        /// <summary>
        /// title case ignoring culture/globalization
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException(BAD_TITLE_CASE_TEXT);

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }
}