namespace StatsDownload.Logging
{
    public interface IApplicationLoggingService
    {
        void LogError(string message);

        void LogVerbose(string message);
    }
}