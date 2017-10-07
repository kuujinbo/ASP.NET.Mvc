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
        public string MiddleName { get; set; }
        public string Edipi { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Populate DodCac instance with LastName, FirstName, MiddleName
        /// and Edipi. Email property set **ONLY** if user selects email 
        /// certificate.
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
        /// Populate DodCac instance with LastName, FirstName, MiddleName,
        /// and Edipi. simpleName parameter from X509Certificate2.GetNameInfo()
        /// [X509NameType.SimpleName]
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
                MiddleName = splitValue.Length > 3 ? TitleCase(splitValue[2]) : string.Empty,
                Edipi = edipi
            };
        }

        /// <summary>
        /// Verify Edipi is ten digits string 
        /// </summary>
        /// <remarks>
        /// All of the BCL IntXX and UIntXX [Try]Parse methods are **INVALID**
        /// for validation because **NONE** guarantee a 10-digit **string**.
        /// UIntXX and string length == 10 works, but not IntXX.
        /// </remarks>
        public static bool ValidEdipi(string edipi)
        {
            return Regex.IsMatch(edipi, @"^\d{10}$");
        }

        /// <summary>
        /// Title case a string ignoring culture/globalization.
        /// </summary>
        public static string TitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException(BAD_TITLE_CASE_TEXT);

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }
}