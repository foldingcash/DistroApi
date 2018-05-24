namespace StatsDownload.Core.Implementations.Tested
{
    using System;
    using System.IO;
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Networking;
    using StatsDownload.Logging;

    public class DownloadProvider : IDownloadService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IFileService fileService;

        private readonly IHttpClientFactory httpClientFactory;

        private readonly ILoggingService loggingService;

        public DownloadProvider(ILoggingService loggingService, IDateTimeService dateTimeService,
                                IHttpClientFactory httpClientFactory, IFileService fileService)
        {
            this.loggingService = loggingService;
            this.dateTimeService = dateTimeService;
            this.httpClientFactory = httpClientFactory;
            this.fileService = fileService;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            loggingService.LogVerbose($"Attempting to download file: {dateTimeService.DateTimeNow()}");

            using (IHttpClient httpClient = httpClientFactory.Create())
            {
                SetUpFileDownload(filePayload, httpClient);
                DownloadFile(httpClient, filePayload.DownloadUri, filePayload.DownloadFilePath);
            }

            loggingService.LogVerbose($"File download complete: {dateTimeService.DateTimeNow()}");
        }

        private void DownloadFile(IHttpClient httpClient, Uri downloadUri, string downloadFilePath)
        {
            IHttpResponseMessage response;

            try
            {
                response = httpClient.GetAsync(downloadUri).Result;
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions.First();
            }

            if (response.IsSuccessStatusCode)
            {
                Stream fileContent = response.Content.ReadAsStreamAsync().Result;
                fileService.CreateFromStream(downloadFilePath, fileContent);
                return;
            }

            throw new Exception($"Failed to download the target file. Status Code: {response.StatusCode}");
        }

        private void SetUpFileDownload(FilePayload filePayload, IHttpClient httpClient)
        {
            httpClient.SslPolicyOverride = (certificate, chain, policyErrors) =>
            {
                loggingService.LogError(
                    $"Invalid SSL Certificate from Server.{Environment.NewLine}Details - Issuer: '{certificate.Issuer}' Subject: '{certificate.Subject}' Error Code: {policyErrors}");

                return filePayload.AcceptAnySslCert;
            };

            httpClient.Timeout = TimeSpan.FromSeconds(filePayload.TimeoutSeconds);
        }
    }
}