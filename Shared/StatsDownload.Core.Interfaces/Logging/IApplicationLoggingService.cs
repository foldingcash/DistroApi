namespace StatsDownload.Core.Interfaces.Logging
{
    public interface IApplicationLoggingService
    {
        void LogDebug(string message);

        void LogError(string message);
    }
}