using System;
using System.Linq;

namespace kuujinbo.Mvc.NET.Helpers
{
    public static class CsvHelper
    {
        public static string RemoveEmptyDuplicates(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) throw new ArgumentException("csv");

            var removed = csv.Trim().Split(
                new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries
            )
            .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();

            return string.Join(",", removed);
        }
    }
}