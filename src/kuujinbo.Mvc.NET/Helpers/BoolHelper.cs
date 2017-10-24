using System;
using System.Linq;

namespace kuujinbo.Mvc.NET.Helpers
{
    public static class BoolHelper
    {
        /// <summary>
        /// Return true count of any number of bools
        /// </summary>
        public static int TrueCount(params bool[] bools)
        {
            if (bools == null) throw new ArgumentNullException("bools");

            return bools.Count(b => b);
        }
    }
}