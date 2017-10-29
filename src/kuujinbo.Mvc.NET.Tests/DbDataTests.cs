using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using kuujinbo.Mvc.NET.Tests._testHelpers;
using Moq;
using Xunit;

namespace kuujinbo.Mvc.NET.Tests
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class DbDataTests : IDisposable
    {
        public const string ConnectionName = "test";
        public const string ConnectionString = "Data Source=:memory:;Version=3;New=True;";
        public const string ProviderName = "System.Data.SQLite";

        ConnectionStringSettings _settings;
        DbData _data;
        Mock<IDbConnection> _connection;
        Mock<IDbCommand> _command;

        public DbDataTests()
        {
            ConfigurationManagerHelper.AddDbProvider();

            ConfigurationManagerHelper.ToggleConnectionStringsWrite();
            _settings = new ConnectionStringSettings(
                ConnectionName, ConnectionString, ProviderName
            );
            ConfigurationManager.ConnectionStrings.Add(_settings);

            _connection = new Mock<IDbConnection>();
            _command = new Mock<IDbCommand>();
            _data = new DbData(ConnectionName);
            _data.Connection = _connection.Object;
            _data.Command = _command.Object;
        }

        public void Dispose()
        {
            ConfigurationManagerHelper.RemoveDbProvider();
            ConfigurationManager.ConnectionStrings.Remove(_settings);
            ConfigurationManagerHelper.ToggleConnectionStringsWrite(true);
        }

        [Fact]
        public void Constructor_NullParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => new DbData(null)
             );
        }

        [Fact]
        public void Constructor_InvalidConnectionNameParameter_Throws()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                 () => new DbData("no connection string with this name")
             );
        }

        [Fact]
        public void Constructor_ValidParameters_InitializesMembers()
        {
            Assert.Equal(DbData.DefaultParameterNamePrefix, _data.ParameterNamePrefix);
            Assert.NotNull(_data.Connection);
            Assert.NotNull(_data.Command);
        }

        [Fact]
        public void Retrieve_NoParameters_ReturnsIDataReader()
        {
            _command.Setup(x => x.ExecuteReader()).Returns(new Mock<IDataReader>().Object);
            var commandText = "test";

            var result = _data.Retrieve(commandText);

            Assert.IsAssignableFrom<IDataReader>(result);
            _command.VerifySet(x => x.CommandText = commandText, Times.Once());
            _command.VerifySet(x => x.CommandType = CommandType.StoredProcedure, Times.Never());
            _command.Verify(x => x.CreateParameter(), Times.Never());
            _command.Verify(x => x.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Never());
            _connection.Verify(x => x.Open(), Times.Once());
            _command.Verify(x => x.ExecuteReader(), Times.Once());
        }

        [Fact]
        public void Retrieve_IsSproc_ReturnsIDataReader()
        {
            _command.Setup(x => x.ExecuteReader()).Returns(new Mock<IDataReader>().Object);
            var commandText = "SELECT";

            var result = _data.Retrieve(commandText, isStoredProcedure: true);

            Assert.IsAssignableFrom<IDataReader>(result);
            _command.VerifySet(x => x.CommandText = commandText, Times.Once());
            _command.VerifySet(x => x.CommandType = CommandType.StoredProcedure, Times.Once());
            _command.Verify(x => x.CreateParameter(), Times.Never());
            _command.Verify(x => x.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Never());
            _connection.Verify(x => x.Open(), Times.Once());
            _command.Verify(x => x.ExecuteReader(), Times.Once());
        }

        [Fact]
        public void Retrieve_ValidParameters_ReturnsIDataReader()
        {
            var dbParameters = new Mock<IDbDataParameter>();
            _command.Setup(x => x.ExecuteReader()).Returns(new Mock<IDataReader>().Object);
            _command.Setup(x => x.CreateParameter()).Returns(dbParameters.Object);
            _command.Setup(x => x.Parameters.Add(It.IsAny<IDbDataParameter>())); 
            var commandText = "SELECT";
            var parameters = new Dictionary<string, object>() { { "p0", 0} };

            var result = _data.Retrieve(commandText, parameters);

            Assert.IsAssignableFrom<IDataReader>(result);
            _command.VerifySet(x => x.CommandText = commandText, Times.Once());
            _connection.Verify(x => x.Open(), Times.Once());
            _command.Verify(x => x.ExecuteReader(), Times.Once());
            _command.Verify(x => x.CreateParameter(), Times.Exactly(parameters.Keys.Count));
            _command.Verify(x => x.Parameters.Add(
                It.IsAny<IDbDataParameter>()), 
                Times.Exactly(parameters.Keys.Count)
            );
        }

        [Fact]
        public void Save_NoParameters_ReturnsInt()
        {
            _command.Setup(x => x.ExecuteNonQuery()).Returns(It.IsAny<int>());
            var commandText = "test";

            var result = _data.Save(commandText);

            Assert.IsType<int>(result);
            _command.VerifySet(x => x.CommandText = commandText, Times.Once());
            _command.VerifySet(x => x.CommandType = CommandType.StoredProcedure, Times.Never());
            _command.Verify(x => x.CreateParameter(), Times.Never());
            _command.Verify(x => x.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Never());
            _connection.Verify(x => x.Open(), Times.Once());
            _command.Verify(x => x.ExecuteNonQuery(), Times.Once());
        }

        [Fact]
        public void Save_IsSproc_ReturnsIDataReader()
        {
            _command.Setup(x => x.ExecuteNonQuery()).Returns(It.IsAny<int>());
            var commandText = "UPDATE";

            var result = _data.Save(commandText, isStoredProcedure: true);

            Assert.IsType<int>(result);
            _command.VerifySet(x => x.CommandText = commandText, Times.Once());
            _command.VerifySet(x => x.CommandType = CommandType.StoredProcedure, Times.Once());
            _command.Verify(x => x.CreateParameter(), Times.Never());
            _command.Verify(x => x.Parameters.Add(It.IsAny<IDbDataParameter>()), Times.Never());
            _connection.Verify(x => x.Open(), Times.Once());
            _command.Verify(x => x.ExecuteNonQuery(), Times.Once());
        }

        [Fact]
        public void Save_ValidParameters_ReturnsInt()
        {
            var dbParameters = new Mock<IDbDataParameter>();
            _command.Setup(x => x.ExecuteNonQuery()).Returns(It.IsAny<int>());
            _command.Setup(x => x.CreateParameter()).Returns(dbParameters.Object);
            _command.Setup(x => x.Parameters.Add(It.IsAny<IDbDataParameter>()));
            var commandText = "INSERT";
            var parameters = new Dictionary<string, object>() { { "p0", 0 }, { "p1", 1 } };

            var result = _data.Save(commandText, parameters);

            Assert.IsType<int>(result);
            _command.VerifySet(x => x.CommandText = commandText, Times.Once());
            _connection.Verify(x => x.Open(), Times.Once());
            _command.Verify(x => x.ExecuteNonQuery(), Times.Once());
            _command.Verify(x => x.CreateParameter(), Times.Exactly(parameters.Keys.Count));
            _command.Verify(x => x.Parameters.Add(
                It.IsAny<IDbDataParameter>()),
                Times.Exactly(parameters.Keys.Count)
            );
        }

        [Fact]
        public void Parameterize_NullParameter_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => _data.Parameterize(null)
             );
        }

        [Fact]
        public void Parameterize_ValidParameters_ReturnsParameterizedQuery()
        {
            var parameters = new Dictionary<string, object>() { { "p0", 0 }, { "p1", 1 } };

            var result = _data.Parameterize(parameters);

            Assert.Equal("@p0,@p1", result);
        }

        [Fact]
        public void Dispose_Call_CleansUp()
        {
            _data.Dispose();

            Assert.Null(_data.Connection);
            Assert.Null(_data.Command);
            _connection.Verify(x => x.Dispose(), Times.Once());
            _command.Verify(x => x.Dispose(), Times.Once());
        }
    }
}