using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;

namespace kuujinbo.Mvc.NET.Tests._testHelpers
{
    [ExcludeFromCodeCoverage]
    public static class TestFiles
    {
        public const string TestDataDirectory = "_testData";
        
        public static readonly string BaseProjectDirectory;

        static TestFiles()
        {
            var replace = new Regex(@"^(.*?)\bbin\b.*");
            var binIndex = AppDomain.CurrentDomain.BaseDirectory;

            BaseProjectDirectory = Path.Combine(
                replace.Replace(AppDomain.CurrentDomain.BaseDirectory, "$1"),
                TestDataDirectory
            );
        }

        public static string GetTestDataPath(string filename)
        {
            return Path.Combine(BaseProjectDirectory, filename);
        }
    }
}
