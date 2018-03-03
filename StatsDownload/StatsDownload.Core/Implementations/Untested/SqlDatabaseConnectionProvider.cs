namespace StatsDownload.Core
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    public class SqlDatabaseConnectionProvider : IDatabaseConnectionService
    {
        private DbConnection sqlConnection;

        public SqlDatabaseConnectionProvider(string connectionString)
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        public void Close()
        {
            sqlConnection.Close();
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
            sqlConnection.Dispose();
            sqlConnection = null;
        }

        public DbDataReader ExecuteReader(string commandText)
        {
            DbCommand command = sqlConnection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteReader();
        }

        public object ExecuteScalar(string commandText)
        {
            DbCommand command = sqlConnection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteScalar();
        }

        public int ExecuteStoredProcedure(string storedProcedure, List<DbParameter> parameters)
        {
            DbCommand command = sqlConnection.CreateCommand();
            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters?.ToArray() ?? new DbParameter[0]);
            return command.ExecuteNonQuery();
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