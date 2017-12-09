namespace StatsDownload.TestHarness
{
    using System;

    using StatsDownload.Core;

    public class TestHarnessLoggingProvider : IFileDownloadLoggingService
    {
        public TestHarnessLoggingProvider(MainForm mainForm)
        {
            Log = mainForm.Log;
        }

        private Action<string> Log { get; }

        public void LogException(Exception exception)
        {
            Log?.Invoke(exception.Message);
        }

        public void LogResult(FileDownloadResult result)
        {
            Log?.Invoke(
                $"Success: {result.Success}{Environment.NewLine}Failed Reason: {result.FailedReason}{Environment.NewLine}Exception Message: {result.Exception?.Message}");
        }

        public void LogVerbose(string message)
        {
            Log?.Invoke(message);
        }
    }
}