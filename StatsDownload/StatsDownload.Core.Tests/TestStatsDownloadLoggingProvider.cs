namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Extensions;

    [TestFixture]
    public class TestStatsDownloadLoggingProvider
    {
        [SetUp]
        public void SetUp()
        {
            loggerMock = Substitute.For<ILogger<StatsDownloadLoggingProvider>>();

            systemUnderTest = NewStatsDownloadLoggingProvider(loggerMock);
        }

        private ILogger<StatsDownloadLoggingProvider> loggerMock;

        private IStatsDownloadLoggingService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadLoggingProvider(null));
        }

        [Test]
        public void LogDebug_WhenInvoked_LogsDebug()
        {
            systemUnderTest.LogDebug("debug");

            loggerMock.Received().LogDebug("debug");
        }

        [Test]
        public void LogError_WhenInvoked_LogsError()
        {
            systemUnderTest.LogError("error");

            loggerMock.Received().LogError("error");
        }

        [Test]
        public void LogException_WhenInvoked_LogsException()
        {
            var exception = new Exception();
            systemUnderTest.LogException(exception);

            loggerMock.Received().LogError(exception, "There was an unexpected exception");
        }

        [Test]
        public void LogFailedUserData_WhenInvoked_LogsFailedUserData()
        {
            var failedUserData = new FailedUserData(100, "data", RejectionReason.UnexpectedFormat,
                new UserData(0, "name", 1, 2, 3)
                {
                    BitcoinAddress = "bitcoin address",
                    FriendlyName = "friendly name",
                    SlpAddress = "slp address",
                    BitcoinCashAddress = "bitcoin cash address"
                });

            systemUnderTest.LogFailedUserData(10, failedUserData);

            loggerMock.Received().LogError($"Download Id: 10{Environment.NewLine}"
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

        [TestCase(150)]
        [TestCase(50)]
        [TestCase(0)]
        [TestCase(100)]
        [TestCase(-1)]
        public void LogResult_WhenFileDownloadResult_LogsResult(int decompressedFileDataLength)
        {
            var filePayload = new FilePayload
                              {
                                  DownloadId = 100,
                                  DownloadUri = new Uri("http://localhost"),
                                  TimeoutSeconds = 101,
                                  AcceptAnySslCert = true,
                                  DownloadDirectory = "download directory",
                                  DownloadFileName = "download file name",
                                  DownloadFileExtension = "download file extension",
                                  DownloadFilePath = "download file path",
                                  DecompressedDownloadDirectory = "decompressed download directory",
                                  DecompressedDownloadFileName = "decompressed download file name",
                                  DecompressedDownloadFileExtension = "decompressed download file extension",
                                  DecompressedDownloadFilePath = "decompressed download file path",
                                  FailedDownloadFilePath = "failed download file path",
                                  DecompressedDownloadFileData =
                                      decompressedFileDataLength < 0 ? null
                                          : new string('A', decompressedFileDataLength)
                              };
            var result = new FileDownloadResult(FailedReason.UnexpectedException, filePayload);
            systemUnderTest.LogResult(result);

            loggerMock.Received().LogDebug($"Success: {result.Success}{Environment.NewLine}"
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

        [Test]
        public void LogResult_WhenStatsUploadResult_LogsResult()
        {
            var statsUploadResult = new StatsUploadResult(1, FailedReason.DatabaseUnavailable);

            systemUnderTest.LogResult(statsUploadResult);

            loggerMock.LogDebug($"Success: {statsUploadResult.Success}{Environment.NewLine}"
                                + $"Failed Reason: {statsUploadResult.FailedReason}{Environment.NewLine}"
                                + $"Download Id: {statsUploadResult.DownloadId}");
        }

        [Test]
        public void LogResults_WhenInvoked_LogsResults()
        {
            var statsUploadResult = new StatsUploadResult(1, FailedReason.DatabaseUnavailable);
            var statsUploadResults =
                new StatsUploadResults(new List<StatsUploadResult> { statsUploadResult, statsUploadResult });

            systemUnderTest.LogResults(statsUploadResults);

            loggerMock.Received(1).LogDebug($"Success: {statsUploadResults.Success}{Environment.NewLine}"
                                            + $"Failed Reason: {statsUploadResults.FailedReason}");
            loggerMock.Received(2).LogDebug($"Success: {statsUploadResult.Success}{Environment.NewLine}"
                                            + $"Failed Reason: {statsUploadResult.FailedReason}{Environment.NewLine}"
                                            + $"Download Id: {statsUploadResult.DownloadId}");
        }

        private IStatsDownloadLoggingService NewStatsDownloadLoggingProvider(
            ILogger<StatsDownloadLoggingProvider> logger)
        {
            return new StatsDownloadLoggingProvider(logger);
        }
    }
}