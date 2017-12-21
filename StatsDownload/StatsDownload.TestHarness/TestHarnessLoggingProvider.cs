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
            Log?.Invoke(exception.StackTrace);
        }

        public void LogResult(FileDownloadResult result)
        {
            Log?.Invoke(
                $"Success: {result.Success}{Environment.NewLine}"
                + $"Failed Reason: {result.FailedReason}{Environment.NewLine}"
                + $"Download Id: {result.DownloadId}{Environment.NewLine}"
                + $"Download Url: {result.DownloadUrl}{Environment.NewLine}"
                + $"Download Timeout: {result.DownloadTimeoutSeconds}{Environment.NewLine}"
                + $"Download Directory: {result.DownloadDirectory}{Environment.NewLine}");
        }

        public void LogVerbose(string message)
        {
            Log?.Invoke(message);
        }
    }
}