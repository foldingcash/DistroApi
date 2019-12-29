namespace StatsDownload.Database.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;

    public class MicrosoftSqlDatabaseConnectionProvider : IDatabaseConnectionService
    {
        private readonly int? commandTimeout;

        private readonly ILoggingService logger;

        private bool disposed;

        private DbConnection sqlConnection;

        public MicrosoftSqlDatabaseConnectionProvider(ILoggingService logger, string connectionString,
                                                      int? commandTimeout = null)
        {
            // This class is instantiated by a Castle.Windsor TypedFactory. NULL is not a valid
            // argument for typed factories and is ignored. Leaving it a required parameter
            // throws an unresolved dependency exception. Marking it as optional lets be created
            // with the NULL value when that's the argument.
            this.commandTimeout = commandTimeout;
            this.logger = logger;

            sqlConnection = NewSqlConnection(connectionString);
        }

        public ConnectionState ConnectionState => sqlConnection.State;

        public void Close()
        {
            logger.LogVerbose("Attempting to close the SQL connection");
            sqlConnection.Close();
            logger.LogVerbose("SQL connection closed");
        }

        public DbCommand CreateDbCommand()
        {
            DbCommand command = sqlConnection.CreateCommand();
            SetCommandTimeout(command);
            return command;
        }

        public DbParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection direction)
        {
            using (DbCommand command = CreateDbCommand())
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.DbType = dbType;
                parameter.Direction = direction;
                return parameter;
            }
        }

        public DbParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection direction, int size)
        {
            using (DbCommand command = CreateDbCommand())
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.DbType = dbType;
                parameter.Direction = direction;
                parameter.Size = size;
                return parameter;
            }
        }

        public DbTransaction CreateTransaction()
        {
            return sqlConnection.BeginTransaction();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                logger.LogVerbose("Attempting to dispose the SQL connection");
                sqlConnection.Dispose();
                sqlConnection = null;
                disposed = true;
                logger.LogVerbose("SQL connection disposed");
            }
        }

        public DbDataReader ExecuteReader(string commandText)
        {
            using (DbCommand command = CreateDbCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteReader();
            }
        }

        public object ExecuteScalar(string commandText)
        {
            using (DbCommand command = CreateDbCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteScalar();
            }
        }

        public void ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters,
                                           DataTable dataTable)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedure))
            {
                command.Parameters.AddRange(parameters?.ToArray() ?? new DbParameter[0]);
                using (IDataReader reader = command.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }
        }

        public int ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedure))
            {
                command.Parameters.AddRange(parameters?.ToArray() ?? new DbParameter[0]);
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteStoredProcedure(DbTransaction transaction, string storedProcedure,
                                          IEnumerable<DbParameter> parameters)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedure))
            {
                command.Transaction = transaction;
                command.Parameters.AddRange(parameters?.ToArray() ?? new DbParameter[0]);
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteStoredProcedure(string storedProcedure)
        {
            return ExecuteStoredProcedure(storedProcedure, (IEnumerable<DbParameter>)null);
        }

        public void ExecuteStoredProcedure(string storedProcedure, DataTable dataTable)
        {
            using (IDbCommand command = CreateStoredProcedureCommand(storedProcedure))
            using (IDataReader reader = command.ExecuteReader())
            {
                dataTable.Load(reader);
            }
        }

        public void Open()
        {
            logger.LogVerbose("Attempting to open SQL connection");
            sqlConnection.Open();
            logger.LogVerbose("SQL connection opened");
        }

        public DbCommand CreateStoredProcedureCommand(string storedProcedure)
        {
            DbCommand command = CreateDbCommand();
            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;
            return command;
        }

        private DbConnection NewSqlConnection(string connectionString)
        {
            try
            {
                logger.LogVerbose("Attempting to create a SQL connection");
                var connection = new SqlConnection(connectionString);
                string server = connection.DataSource;
                string database = connection.Database;
                logger.LogVerbose($"SQL connection created with Server: {server} Database: {database}");
                return connection;
            }
            catch (Exception)
            {
                logger.LogVerbose($"SQL connection failed to create with ConnectionString: {connectionString}");
                throw;
            }
        }

        private void SetCommandTimeout(DbCommand command)
        {
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }
        }
    }
}