namespace StatsDownload.Core
{
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
            sqlConnection.Dispose();
            sqlConnection = null;
        }

        public void Open()
        {
            sqlConnection.Open();
        }
    }
}