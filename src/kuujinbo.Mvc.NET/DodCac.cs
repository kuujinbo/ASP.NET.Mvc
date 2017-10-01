using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System;

namespace kuujinbo.Mvc.NET
{
    public interface IDodCac
    {
        DodCac Get(byte[] rawData);
    }
    public class DodCac : IDodCac
    {
        public const string BAD_GET_PARAM = "rawData";
        public const string BAD_EDIPI = "edipi not 10 digits";
        public const string BAD_SIMPLE_NAME = "simpleName";
        public const string BAD_TITLE_CASE_TEXT = "text cannot be null or whitespace";

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Edipi { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Get DodCac object with LastName, FirstName, & EdiPi properties populated.
        /// Set Email property if user selected email certificate.
        /// </summary>
        public virtual DodCac Get(byte[] rawData)
        {
            if (rawData == null) throw new ArgumentNullException(BAD_GET_PARAM);

            X509Certificate2 cert = new X509Certificate2(rawData);
            var cacInfo = GetDodCac(cert.GetNameInfo(X509NameType.SimpleName, false));
            cacInfo.Email = cert.GetNameInfo(X509NameType.EmailName, false)
                .ToLower();

            return cacInfo;
        }

        /// <summary>
        /// Get DodCac object with LastName, FirstName, & EdiPi properties populated
        /// </summary>
        public static DodCac GetDodCac(string simpleName)
        {
            string[] splitValue = simpleName.Split(new char[] { '.' });
            // CAC should at least have last.first.edipi if no middle initial/name
            if (splitValue.Length < 3) throw new FormatException(BAD_SIMPLE_NAME);

            var edipi = splitValue[splitValue.Length - 1];
            if (!ValidEdipi(edipi)) throw new FormatException(BAD_EDIPI);

            return new DodCac()
            {
                LastName = TitleCase(splitValue[0]),
                FirstName = TitleCase(splitValue[1]),
                Edipi = edipi
            };
        }

        /// <summary>
        /// Verify Edipi is ten digits string 
        /// </summary>
        /// <remarks>
        /// All of the CLR IntXX and UIntXX [Try]Parse methods are **INVALID**
        /// for validation because **NONE** guarantee a 10-digit **string**.
        /// UIntXX and string length == 10 works, but not IntXX.
        /// </remarks>
        public static bool ValidEdipi(string edipi)
        {
            return Regex.IsMatch(edipi, @"^\d{10}$");
        }

        /// <summary>
        /// title case a string, ignoring culture/globalization
        /// </summary>
        public static string TitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException(BAD_TITLE_CASE_TEXT);

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }
}