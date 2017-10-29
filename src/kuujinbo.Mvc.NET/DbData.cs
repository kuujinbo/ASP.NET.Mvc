using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("kuujinbo.Mvc.NET.Tests")]
namespace kuujinbo.Mvc.NET
{
    public interface IDbData
    {
        IDataReader Retrieve(
            string commandText,
            Dictionary<string, object> parameters
        );

        int Save(
            string commandText,
            Dictionary<string, object> parameters
        );
    }

    /// <summary>
    /// A **quick and simple** ADO.NET wrapper to read and write data.
    /// </summary>
    public class DbData : IDbData
    {
        public IDbConnection Connection { get; internal set; }
        public IDbCommand Command { get; internal set; }

        public DbData() { }

        public DbData(string connectionName)
        {
            if (connectionName == null) throw new ArgumentNullException("connectionName");

            var configurationManager = ConfigurationManager.ConnectionStrings[connectionName];
            if (configurationManager == null) throw new InvalidOperationException(
                "connection string not found"
            );

            var dbProviderFactory = DbProviderFactories.GetFactory(configurationManager.ProviderName);
            Connection = dbProviderFactory.CreateConnection();
            Connection.ConnectionString = configurationManager.ConnectionString;
            Command = Connection.CreateCommand();
        }


        public virtual IDataReader Retrieve(
            string commandText, 
            Dictionary<string, object> parameters = null)
        {
            Command.CommandText = commandText;
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    var dbParameter = Command.CreateParameter();
                    dbParameter.ParameterName = "@" + kvp.Key;
                    dbParameter.Value = kvp.Value;
                    Command.Parameters.Add(dbParameter);
                }
            }
            Connection.Open();

            return Command.ExecuteReader();
        }

        public virtual int Save(
            string commandText, 
            Dictionary<string, object> parameters = null)
        {
            Command.CommandText = commandText;
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    var dbParameter = Command.CreateParameter();
                    dbParameter.ParameterName = "@" + kvp.Key;
                    dbParameter.Value = kvp.Value;
                    Command.Parameters.Add(dbParameter);
                }
            }
            Connection.Open();

            return Command.ExecuteNonQuery();
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