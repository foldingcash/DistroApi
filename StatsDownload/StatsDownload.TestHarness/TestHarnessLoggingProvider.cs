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
            Log?.Invoke($"Exception Type: {exception.GetType()}");
            Log?.Invoke($"Exception Message: {exception.Message}");
            Log?.Invoke($"Exception Stack-trace:{Environment.NewLine}{exception.StackTrace}");
        }

        public void LogResult(FileDownloadResult result)
        {
            Log?.Invoke(
                $"Success: {result.Success}{Environment.NewLine}"
                + $"Failed Reason: {result.FailedReason}{Environment.NewLine}"
                + $"Download Id: {result.FilePayload?.DownloadId}{Environment.NewLine}"
                + $"Download Url: {result.FilePayload?.DownloadUrl}{Environment.NewLine}"
                + $"Download Timeout: {result.FilePayload?.TimeoutSeconds}{Environment.NewLine}"
                + $"Download File Directory: {result.FilePayload?.DownloadDirectory}{Environment.NewLine}{Environment.NewLine}"
                + $"Download File Name: {result.FilePayload?.DownloadFileName}{Environment.NewLine}{Environment.NewLine}"
                + $"Download File Extension: {result.FilePayload?.DownloadFileExtension}{Environment.NewLine}{Environment.NewLine}"
                + $"Download File Path: {result.FilePayload?.DownloadFilePath}{Environment.NewLine}{Environment.NewLine}"
                + $"Uncompressed Download File Directory: {result.FilePayload?.UncompressedDownloadDirectory}{Environment.NewLine}{Environment.NewLine}"
                + $"Uncompressed Download File Name: {result.FilePayload?.UncompressedDownloadFileName}{Environment.NewLine}{Environment.NewLine}"
                + $"Uncompressed Download File Extension: {result.FilePayload?.UncompressedDownloadFileExtension}{Environment.NewLine}{Environment.NewLine}"
                + $"Uncompressed Download File Path: {result.FilePayload?.UncompressedDownloadFilePath}{Environment.NewLine}{Environment.NewLine}"
                + $"Download Data (First 100): {result.FilePayload?.UncompressedDownloadFileData?.Substring(0, 99)}{Environment.NewLine}{Environment.NewLine}");
        }

        public void LogVerbose(string message)
        {
            Log?.Invoke(message);
        }
    }
}