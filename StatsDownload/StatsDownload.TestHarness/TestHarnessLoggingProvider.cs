namespace StatsDownload.TestHarness
{
    using System;

    using StatsDownload.Logging;

    public class TestHarnessLoggingProvider : IApplicationLoggingService
    {
        private readonly Action<string> logAction;

        public TestHarnessLoggingProvider(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        public void LogError(string message)
        {
            logAction?.Invoke(message);
        }

        public void LogVerbose(string message)
        {
            logAction?.Invoke(message);
        }
    }
}