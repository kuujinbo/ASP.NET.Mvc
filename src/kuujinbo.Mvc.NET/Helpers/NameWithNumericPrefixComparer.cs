using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace kuujinbo.Mvc.NET.Helpers
{
    public interface INameWithNumericPrefix
    {
        string Name { get; set; }
    }

    public class NameWithNumericPrefixComparer : IComparer<INameWithNumericPrefix>
    {
        public static readonly Regex DigitsPrefix = new Regex(@"^(\d+)", RegexOptions.Compiled);

        public int Compare(INameWithNumericPrefix x, INameWithNumericPrefix y)
        {
            int xNumeric, yNumeric;
            var xMatch = DigitsPrefix.Match(x.Name);
            var yMatch = DigitsPrefix.Match(y.Name);

            if (xMatch.Success && yMatch.Success
                && Int32.TryParse(xMatch.Groups[1].Value, out xNumeric)
                && Int32.TryParse(yMatch.Groups[1].Value, out yNumeric))
            {
                return xNumeric != yNumeric
                    ? xNumeric.CompareTo(yNumeric)
                    : x.Name.CompareTo(y.Name);
            }

            return x.Name.CompareTo(y.Name);
        }

    }
}