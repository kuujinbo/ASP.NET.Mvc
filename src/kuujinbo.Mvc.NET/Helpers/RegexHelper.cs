using System.Text.RegularExpressions;

namespace kuujinbo.Mvc.NET.Helpers
{
    public class RegexHelper
    {
        static readonly Regex _pascalRegex = new Regex(
        @"	# lookbehind/lookahead match on **boundaries**
            # positive lookbehind
            (?<=			# start
                [A-Za-z]	# SINGLE upper OR lower
            )         		# end
            
            # positive lookahead
            (?=				# start
                [A-Z][a-z]	# upper FOLLOWED by lower
            )         		# end
            ",
             RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
        );

        /// <summary>
        /// Make convention based Pascal cased enums more readable.
        /// </summary>
        public static string PascalCaseSplit(string input)
        {
            return input != null ? _pascalRegex.Replace(input, " ") : input;
        }
    }
}