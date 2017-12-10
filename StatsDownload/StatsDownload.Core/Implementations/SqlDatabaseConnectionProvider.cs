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

        public void Dispose()
        {
            sqlConnection.Close();
            sqlConnection.Dispose();
            sqlConnection = null;
        }

        public void ExecuteStoredProcedure()
        {
            using (SqlCommand command = sqlConnection.CreateCommand())
            {
                command.CommandText = "[FoldingCoin].[UpdateToLatest]";
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
        }

        public void Open()
        {
            sqlConnection.Open();
        }
    }
}