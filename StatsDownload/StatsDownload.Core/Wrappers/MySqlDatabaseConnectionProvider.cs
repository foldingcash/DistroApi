namespace StatsDownload.Core.Wrappers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Interfaces;
    using MySql.Data.MySqlClient;

    public class MySqlDatabaseConnectionProvider : IDatabaseConnectionService
    {
        private bool disposed;

        private DbConnection sqlConnection;

        public MySqlDatabaseConnectionProvider(string connectionString)
        {
            sqlConnection = new MySqlConnection(connectionString);
        }

        public ConnectionState ConnectionState => sqlConnection.State;

        public DbCommand CreateDbCommand()
        {
            return sqlConnection.CreateCommand();
        }

        public DbTransaction CreateTransaction()
        {
            return sqlConnection.BeginTransaction();
        }

        public DbParameter CreateParameter(string parameterName, DbType dbType, ParameterDirection direction)
        {
            using (DbCommand command = sqlConnection.CreateCommand())
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
            using (DbCommand command = sqlConnection.CreateCommand())
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.DbType = dbType;
                parameter.Direction = direction;
                parameter.Size = size;
                return parameter;
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                sqlConnection.Dispose();
                sqlConnection = null;
                disposed = true;
            }
        }

        public DbDataReader ExecuteReader(string commandText)
        {
            using (DbCommand command = sqlConnection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteReader();
            }
        }

        public object ExecuteScalar(string commandText)
        {
            using (DbCommand command = sqlConnection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteScalar();
            }
        }

        public int ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters)
        {
            using (DbCommand command = sqlConnection.CreateCommand())
            {
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters?.ToArray() ?? new DbParameter[0]);
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteStoredProcedure(string storedProcedure)
        {
            return ExecuteStoredProcedure(storedProcedure, null);
        }

        public void Open()
        {
            sqlConnection.Open();
        }
    }
}