using System;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary>
/// For the love of jumping through unit test rings of fire thanks to M$!
/// </summary>
namespace kuujinbo.Mvc.NET.Tests._testHelpers
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationManagerHelper
    {
        #region ConnectionStrings
        /// <summary>
        /// ConnectionStringCollection is READONLY; add a convenience method
        /// **ONLY** for testing that allows us to add a connection string 
        /// at runtime.
        /// </summary>
        /// <param name="isReadonly"></param>
        public static void ToggleConnectionStringsWrite(bool isReadonly = false)
        {
            typeof(ConfigurationElementCollection)
                .GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(ConfigurationManager.ConnectionStrings, isReadonly);
        }
        #endregion


        #region DbProviderFactory
        public const string DbProviderFactoriesElement = "DbProviderFactories";
        public const string DataSection = "system.data";
        public const string SQLite = "System.Data.SQLite";

        public static object[] SQLiteProvider = new object[] {
            "SQLite Data Provider",                                 // name
            ".Net Framework Data Provider for SQLite",              // description
            SQLite,                                                 // invariant
            "System.Data.SQLite.SQLiteFactory, System.Data.SQLite"  // type
        }; 

        public static void AddDbProvider()
        {
            var section = GetRootSection();
            if (section.Tables.Contains(DbProviderFactoriesElement))
            {
                RemoveDbProvider();
            }
            else
            {
                section.Tables.Add(DbProviderFactoriesElement);
            }

            GetProvider(section).Rows.Add(SQLiteProvider);
        }

        public static void RemoveDbProvider()
        {
            var root = GetRootSection();
            var provider = GetProvider(root);
            var row = provider.Rows.Find(SQLite);
            if (row != null) provider.Rows.Remove(row);
        }

        static DataSet GetRootSection()
        {
            var section = ConfigurationManager.GetSection(DataSection) as DataSet;
            if (section == null) throw new Exception(string.Format(
                "'{0}' missing from system configuration file.", DataSection
            ));

            return section;
        }

        static DataTable GetProvider(DataSet root)
        {
            return root.Tables[DbProviderFactoriesElement];
        }
        #endregion
    }
}