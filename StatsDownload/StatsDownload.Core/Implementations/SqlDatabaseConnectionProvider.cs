namespace StatsDownload.Core
{
    using System;
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
            throw new NotImplementedException();
        }

        public void Open()
        {
            sqlConnection.Open();
        }
    }
}