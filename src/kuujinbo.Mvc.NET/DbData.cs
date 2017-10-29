using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("kuujinbo.Mvc.NET.Tests")]
namespace kuujinbo.Mvc.NET
{
    public interface IDbData
    {
        IDataReader Retrieve(
            string commandText,
            Dictionary<string, object> parameters,
            bool isStoredProcedure
        );

        int Save(
            string commandText,
            Dictionary<string, object> parameters,
            bool isStoredProcedure
        );
    }

    /// <summary>
    /// A **quick and simple** ADO.NET wrapper to read and write data.
    /// </summary>
    public class DbData : IDbData
    {
        public const string DefaultParameterNamePrefix = "@"; // SQL Server
        public IDbConnection Connection { get; internal set; }
        public IDbCommand Command { get; internal set; }

        public string ParameterNamePrefix { get; set; }

        public DbData() { }

        public DbData(string connectionName)
        {
            if (connectionName == null) throw new ArgumentNullException("connectionName");

            var configurationManager = ConfigurationManager.ConnectionStrings[connectionName];
            if (configurationManager == null) throw new InvalidOperationException(
                "connection string not found"
            );

            ParameterNamePrefix = DefaultParameterNamePrefix;

            var dbProviderFactory = DbProviderFactories.GetFactory(configurationManager.ProviderName);
            Connection = dbProviderFactory.CreateConnection();
            Connection.ConnectionString = configurationManager.ConnectionString;
            Command = Connection.CreateCommand();
        }

        public virtual IDataReader Retrieve(
            string commandText, 
            Dictionary<string, object> parameters = null,
            bool isStoredProcedure = false)
        {
            Command.CommandText = commandText;
            if (isStoredProcedure) Command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    var dbParameter = Command.CreateParameter();
                    dbParameter.ParameterName = ParameterNamePrefix + kvp.Key;
                    dbParameter.Value = kvp.Value;
                    Command.Parameters.Add(dbParameter);
                }
            }
            Connection.Open();

            return Command.ExecuteReader();
        }

        public virtual int Save(
            string commandText, 
            Dictionary<string, object> parameters = null,
            bool isStoredProcedure = false)
        {
            Command.CommandText = commandText;
            if (isStoredProcedure) Command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    var dbParameter = Command.CreateParameter();
                    dbParameter.ParameterName = ParameterNamePrefix + kvp.Key;
                    dbParameter.Value = kvp.Value;
                    Command.Parameters.Add(dbParameter);
                }
            }
            Connection.Open();

            return Command.ExecuteNonQuery();
        }

        /// <summary>
        /// Build a dynamic string for a parameterized query.
        /// </summary>
        public virtual string Parameterize(IDictionary<string, object> parameters)
        {
            if (parameters == null) throw new ArgumentNullException("parameters");

            return string.Join(
                ",",
                parameters.Keys.Select(
                    x => string.Format("{0}{1}", ParameterNamePrefix, x)
                ).ToList()
            );
        }

        /// <summary>
        /// Dispose of ADO.NET members that implement IDisposable
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // clean up System.Data unmanaged resources - no need to check 
            // disposing, which flags managed resource clean up.
            if (Command != null)
            {
                Command.Dispose();
                Command = null;
            }
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}