namespace StatsDownload.TestHarness
{
    using System;

    using StatsDownload.Core;
    using StatsDownload.Logging;

    public class TestHarnessLoggingProvider : ILoggingService, IFileDownloadLoggingService
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
            Log?.Invoke($"Success: {result.Success}{Environment.NewLine}"
                        + $"Failed Reason: {result.FailedReason}{Environment.NewLine}"
                        + $"Download Id: {result.FilePayload?.DownloadId}{Environment.NewLine}"
                        + $"Download Uri: {result.FilePayload?.DownloadUri}{Environment.NewLine}"
                        + $"Download Timeout: {result.FilePayload?.TimeoutSeconds}{Environment.NewLine}"
                        + $"Accept Any Ssl Cert: {result.FilePayload?.AcceptAnySslCert}{Environment.NewLine}"
                        + $"Download File Directory: {result.FilePayload?.DownloadDirectory}{Environment.NewLine}{Environment.NewLine}"
                        + $"Download File Name: {result.FilePayload?.DownloadFileName}{Environment.NewLine}{Environment.NewLine}"
                        + $"Download File Extension: {result.FilePayload?.DownloadFileExtension}{Environment.NewLine}{Environment.NewLine}"
                        + $"Download File Path: {result.FilePayload?.DownloadFilePath}{Environment.NewLine}{Environment.NewLine}"
                        + $"Decompressed Download File Directory: {result.FilePayload?.DecompressedDownloadDirectory}{Environment.NewLine}{Environment.NewLine}"
                        + $"Decompressed Download File Name: {result.FilePayload?.DecompressedDownloadFileName}{Environment.NewLine}{Environment.NewLine}"
                        + $"Decompressed Download File Extension: {result.FilePayload?.DecompressedDownloadFileExtension}{Environment.NewLine}{Environment.NewLine}"
                        + $"Decompressed Download File Path: {result.FilePayload?.DecompressedDownloadFilePath}{Environment.NewLine}{Environment.NewLine}"
                        + $"Download Data (First 100): {result.FilePayload?.DecompressedDownloadFileData?.Substring(0, 99)}");
        }

        public void LogVerbose(string message)
        {
            Log?.Invoke(message);
        }
    }
}