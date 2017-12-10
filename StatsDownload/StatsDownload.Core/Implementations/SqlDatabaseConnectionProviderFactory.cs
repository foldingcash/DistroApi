namespace StatsDownload.Core
{
    public class SqlDatabaseConnectionProviderFactory : IDatabaseConnectionServiceFactory
    {
        public IDatabaseConnectionService Create(string connectionString)
        {
            return new SqlDatabaseConnectionProvider(connectionString);
        }
    }
}