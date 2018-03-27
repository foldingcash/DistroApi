namespace StatsDownload.FileDownload.Console
{
    using StatsDownload.Logging;

    public class FileDownloadConsoleLoggingProvider : IApplicationLoggingService
    {
        public void LogError(string message)
        {
            System.Console.WriteLine(message);
        }

        public void LogVerbose(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}