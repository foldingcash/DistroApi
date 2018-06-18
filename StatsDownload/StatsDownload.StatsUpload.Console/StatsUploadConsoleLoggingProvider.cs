namespace StatsDownload.StatsUpload.Console
{
    using System;
    using Core.Interfaces.Logging;

    public class StatsUploadConsoleLoggingProvider : IApplicationLoggingService
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