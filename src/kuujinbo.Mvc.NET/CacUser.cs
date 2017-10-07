using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace kuujinbo.Mvc.NET
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Common_Access_Card
    /// </summary>
    public interface ICacUser
    {
        CacUser Create(byte[] rawData);
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Common_Access_Card
    /// </summary>
    public class CacUser : ICacUser
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Edipi { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Exception message
        /// </summary>
        public const string InvalidCreateParameter = "rawData";

        /// <summary>
        /// Exception message
        /// </summary>
        public const string InvalidEdipi = "edipi not 10 digits";
        
        /// <summary>
        /// Exception message
        /// </summary>
        public const string InvalidSimpleNameParameter = "simpleName";
        
        /// <summary>
        /// Exception message
        /// </summary>
        public const string InvalidTitleCaseParameter = "text cannot be null or whitespace";

        /// <summary>
        /// Edipi validator
        /// </summary>
        public static readonly Regex EdipiValidator = new Regex(
            @"^\d{10}$", RegexOptions.Compiled
        );

        /// <summary>
        /// Verify Edipi is a **10-digit string**:
        /// All BCL IntXX and UIntXX [Try]Parse methods **FAIL**
        /// validation because **NONE** guarantee a **10-digit string**.
        /// UIntXX and string length == 10 works, but not IntXX.
        /// </summary>
        public static bool ValidEdipi(string edipi)
        {
            return EdipiValidator.IsMatch(edipi);
        }

        /// <summary>
        /// Populate instance with LastName, FirstName, MiddleName and Edipi.
        /// Email property set **ONLY** if user selects email certificate.
        /// </summary>
        public virtual CacUser Create(byte[] rawData)
        {
            if (rawData == null) throw new ArgumentNullException(InvalidCreateParameter);

            X509Certificate2 cert = new X509Certificate2(rawData);
            var cacInfo = SetNameInfo(cert.GetNameInfo(X509NameType.SimpleName, false));
            cacInfo.Email = cert.GetNameInfo(X509NameType.EmailName, false)
                                .ToLower();

            return cacInfo;
        }

        /// <summary>
        /// Populate instance with LastName, FirstName, MiddleName, and Edipi.
        /// simpleName parameter from X509Certificate2.GetNameInfo()
        /// [X509NameType.SimpleName]
        /// </summary>
        public static CacUser SetNameInfo(string simpleName)
        {
            string[] splitValue = simpleName.Split(new char[] { '.' });
            // At minimum CAC has last.first.edipi format if no middle.
            if (splitValue.Length < 3) throw new FormatException(InvalidSimpleNameParameter);

            var edipi = splitValue[splitValue.Length - 1];
            if (!ValidEdipi(edipi)) throw new FormatException(InvalidEdipi);

            return new CacUser()
            {
                LastName = TitleCase(splitValue[0]),
                FirstName = TitleCase(splitValue[1]),
                MiddleName = splitValue.Length > 3 ? TitleCase(splitValue[2]) : string.Empty,
                Edipi = edipi
            };
        }

        /// <summary>
        /// Title case a string ignoring culture/globalization.
        /// </summary>
        public static string TitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException(InvalidTitleCaseParameter);

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }
}