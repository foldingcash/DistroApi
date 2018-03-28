namespace StatsDownload.TestHarness
{
    using System;

    using StatsDownload.Logging;

    public class TestHarnessLoggingProvider : IApplicationLoggingService
    {
        public TestHarnessLoggingProvider(MainForm mainForm)
        {
            Log = mainForm.Log;
        }

        private Action<string> Log { get; }

        public void LogError(string message)
        {
            Log?.Invoke(message);
        }

        public void LogVerbose(string message)
        {
            Log?.Invoke(message);
        }
    }
}