namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Extensions;

    public class StatsDownloadLoggingProvider : IStatsDownloadLoggingService
    {
        private readonly ILogger logger;

        public StatsDownloadLoggingProvider(ILogger<StatsDownloadLoggingProvider> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogFailedUserData(int downloadId, FailedUserData failedUserData)
        {
            logger.LogError($"Download Id: {downloadId}{Environment.NewLine}"
                            + $"Line Number: {failedUserData.LineNumber}{Environment.NewLine}"
                            + $"Data: {failedUserData.Data}{Environment.NewLine}"
                            + $"Rejection Reason: {failedUserData.RejectionReason}{Environment.NewLine}"
                            + $"Name: {failedUserData.UserData?.Name}{Environment.NewLine}"
                            + $"Total Points: {failedUserData.UserData?.TotalPoints}{Environment.NewLine}"
                            + $"Total Work Units: {failedUserData.UserData?.TotalWorkUnits}{Environment.NewLine}"
                            + $"Team Number: {failedUserData.UserData?.TeamNumber}{Environment.NewLine}"
                            + $"Friendly Name: {failedUserData.UserData?.FriendlyName}{Environment.NewLine}"
                            + $"Bitcoin Address: {failedUserData.UserData?.BitcoinAddress}"
                            + $"Bitcoin Cash Address: {failedUserData.UserData?.BitcoinCashAddress}"
                            + $"SLP Address: {failedUserData.UserData?.SlpAddress}");
        }

        public void LogResult(FileDownloadResult result)
        {
            LogDebug($"Success: {result.Success}{Environment.NewLine}"
                     + $"Failed Reason: {result.FailedReason}{Environment.NewLine}"
                     + $"Download Id: {result.FilePayload?.DownloadId}{Environment.NewLine}"
                     + $"Download Uri: {result.FilePayload?.DownloadUri}{Environment.NewLine}"
                     + $"Download Timeout: {result.FilePayload?.TimeoutSeconds}{Environment.NewLine}"
                     + $"Accept Any Ssl Cert: {result.FilePayload?.AcceptAnySslCert}{Environment.NewLine}"
                     + $"Download File Directory: {result.FilePayload?.DownloadDirectory}{Environment.NewLine}"
                     + $"Download File Name: {result.FilePayload?.DownloadFileName}{Environment.NewLine}"
                     + $"Download File Extension: {result.FilePayload?.DownloadFileExtension}{Environment.NewLine}"
                     + $"Download File Path: {result.FilePayload?.DownloadFilePath}{Environment.NewLine}"
                     + $"Decompressed Download File Directory: {result.FilePayload?.DecompressedDownloadDirectory}{Environment.NewLine}"
                     + $"Decompressed Download File Name: {result.FilePayload?.DecompressedDownloadFileName}{Environment.NewLine}"
                     + $"Decompressed Download File Extension: {result.FilePayload?.DecompressedDownloadFileExtension}{Environment.NewLine}"
                     + $"Decompressed Download File Path: {result.FilePayload?.DecompressedDownloadFilePath}{Environment.NewLine}"
                     + $"Failed Download File Path: {result.FilePayload?.FailedDownloadFilePath}{Environment.NewLine}"
                     + $"Download Data (First 100): {result.FilePayload?.DecompressedDownloadFileData?.SubstringSafe(0, 100)}");
        }

        public void LogResult(StatsUploadResult statsUploadResult)
        {
            LogDebug($"Success: {statsUploadResult.Success}{Environment.NewLine}"
                     + $"Failed Reason: {statsUploadResult.FailedReason}{Environment.NewLine}"
                     + $"Download Id: {statsUploadResult.DownloadId}");
        }

        public void LogResults(StatsUploadResults statsUploadResults)
        {
            LogDebug($"Success: {statsUploadResults.Success}{Environment.NewLine}"
                     + $"Failed Reason: {statsUploadResults.FailedReason}");

            foreach (StatsUploadResult statsUploadResult in statsUploadResults?.UploadResults
                                                            ?? new List<StatsUploadResult>())
            {
                LogResult(statsUploadResult);
            }
        }

        private void LogDebug(string message)
        {
            logger.LogDebug(message);
        }
    }
}