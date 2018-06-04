namespace StatsDownload.Core.Implementations.Tested
{
    using System;
    using System.Net.Security;
    using Interfaces.Logging;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Networking;

    public class DownloadProvider : IDownloadService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly ILoggingService loggingService;

        private readonly IWebClientFactory webClientFactory;

        public DownloadProvider(ILoggingService loggingService, IDateTimeService dateTimeService,
                                IWebClientFactory webClientFactory)
        {
            this.loggingService = loggingService;
            this.dateTimeService = dateTimeService;
            this.webClientFactory = webClientFactory;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            loggingService.LogVerbose($"Attempting to download file: {dateTimeService.DateTimeNow()}");

            using (IWebClient webClient = webClientFactory.Create())
            {
                SetUpFileDownload(filePayload, webClient);
                DownloadFile(filePayload, webClient);
            }

            loggingService.LogVerbose($"File download complete: {dateTimeService.DateTimeNow()}");
        }

        private void DownloadFile(FilePayload filePayload, IWebClient webClient)
        {
            webClient.DownloadFile(filePayload.DownloadUri, filePayload.DownloadFilePath);
        }

        private void SetUpFileDownload(FilePayload filePayload, IWebClient webClient)
        {
            webClient.SslPolicyOverride = (certificate, chain, policyErrors) =>
            {
                loggingService.LogError(
                    $"Invalid SSL Certificate from Server.{Environment.NewLine}Details - Issuer: '{certificate.Issuer}' Subject: '{certificate.Subject}' Error Code: {policyErrors}");

                if (filePayload.AcceptAnySslCert)
                {
                    return true;
                }

                return policyErrors == SslPolicyErrors.None;
            };

            webClient.Timeout = TimeSpan.FromSeconds(filePayload.TimeoutSeconds);
        }
    }
}