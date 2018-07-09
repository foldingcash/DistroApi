namespace StatsDownload.Core.Interfaces.Logging
{
    public interface IApplicationLoggingService
    {
        void LogError(string message);

        void LogVerbose(string message);
    }
}