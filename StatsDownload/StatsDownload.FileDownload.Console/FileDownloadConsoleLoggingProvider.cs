namespace StatsDownload.FileDownload.Console
{
    using System;

    using StatsDownload.Logging;

    public class FileDownloadConsoleLoggingProvider : IApplicationLoggingService
    {
        public void LogError(string message)
        {
            WriteLine(message);
        }

        public void LogVerbose(string message)
        {
            WriteLine(message);
        }

        private void WriteLine(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
    }
}