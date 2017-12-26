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
                + $"Download Id: {result.StatsPayload.DownloadId}{Environment.NewLine}"
                + $"Download URL: {result.StatsPayload.DownloadUrl}{Environment.NewLine}"
                + $"Download Timeout: {result.StatsPayload.TimeoutSeconds}{Environment.NewLine}"
                + $"Download File Path: {result.StatsPayload.DownloadFilePath}{Environment.NewLine}{Environment.NewLine}"
                + $"Uncompressed Download File Path: {result.StatsPayload.UncompressedDownloadFilePath}{Environment.NewLine}{Environment.NewLine}"
                + $"Download Data (First 100): {result.StatsPayload.StatsData.Substring(0, 99)}{Environment.NewLine}{Environment.NewLine}");
        }

        public void LogVerbose(string message)
        {
            Log?.Invoke(message);
        }
    }
}