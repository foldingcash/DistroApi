namespace StatsDownload.Core
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    public class SqlDatabaseConnectionProvider : IDatabaseConnectionService
    {
        public SqlDatabaseConnectionProvider(string connectionString)
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        private DbConnection sqlConnection { get; set; }

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

        public void Dispose()
        {
            sqlConnection.Close();
            sqlConnection.Dispose();
            sqlConnection = null;
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