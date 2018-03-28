namespace StatsDownload.StatsUpload.Console
{
    using StatsDownload.Logging;

    public class StatsUploadConsoleLoggingProvider : IApplicationLoggingService
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