namespace StatsDownload.Core
{
    using System.Data;
    using System.Data.SqlClient;

    public class SqlDatabaseConnectionProvider : IDatabaseConnectionService
    {
        public SqlDatabaseConnectionProvider(string connectionString)
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        private SqlConnection sqlConnection { get; set; }

        public void Close()
        {
            sqlConnection.Close();
        }

        public void Dispose()
        {
            sqlConnection.Close();
            sqlConnection.Dispose();
            sqlConnection = null;
        }

        public int ExecuteStoredProcedure(string storedProcedure)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;
            return command.ExecuteNonQuery();
        }

        public void Open()
        {
            sqlConnection.Open();
        }
    }
}