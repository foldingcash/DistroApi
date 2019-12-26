namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.Logging;

    public interface IDatabaseConnectionServiceFactory
    {
        IDatabaseConnectionService Create(ILoggingService logger, string connectionString, int? commandTimeout);
    }
}