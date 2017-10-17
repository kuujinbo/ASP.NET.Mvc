using System;
using System.Text.RegularExpressions;

namespace kuujinbo.Mvc.NET
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Common_Access_Card
    /// </summary>
    public class CacUser
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Edipi { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Subject name stored for logging / debugging
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Exception message when certificate trust chain fails validation.
        /// </summary>
        public string ChainError { get; set; }

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
        /// Verify Edipi is a **10-digit string identifier**:
        /// All BCL IntXX and UIntXX [Try]Parse methods **FAIL** validation
        /// because **NONE** guarantee a **10-digit string identifier**.
        /// </summary>
        public static bool ValidEdipi(string edipi)
        {
            return EdipiValidator.IsMatch(edipi);
        }

        /// <summary>
        /// Create instance with LastName, FirstName, MiddleName, and Edipi.
        /// simpleName parameter from X509Certificate2.GetNameInfo()
        /// [X509NameType.SimpleName]
        /// </summary>
        public static CacUser Create(string subjectName)
        {
            string[] splitValue = subjectName.Split(new char[] { '.' });
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
        /// Title case ignoring culture/globalization.
        /// </summary>
        public static string TitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException(InvalidTitleCaseParameter);

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }
}