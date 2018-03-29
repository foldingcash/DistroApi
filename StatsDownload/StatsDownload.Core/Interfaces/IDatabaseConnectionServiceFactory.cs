namespace StatsDownload.Core
{
    public interface IDatabaseConnectionServiceFactory
    {
        IDatabaseConnectionService Create(string connectionString);
    }
}