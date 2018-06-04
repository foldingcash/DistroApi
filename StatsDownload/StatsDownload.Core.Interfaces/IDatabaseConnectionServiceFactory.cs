namespace StatsDownload.Core.Interfaces
{
    public interface IDatabaseConnectionServiceFactory
    {
        IDatabaseConnectionService Create(string connectionString);
    }
}