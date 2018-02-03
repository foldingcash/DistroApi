namespace StatsDownload.Core
{
    using System;
    using System.Net;

    using StatsDownload.Logging;

    public class DownloadProvider : IDownloadService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly ILoggingService loggingService;

        public DownloadProvider(ILoggingService loggingService, IDateTimeService dateTimeService)
        {
            this.loggingService = loggingService;
            this.dateTimeService = dateTimeService;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            loggingService.LogVerbose($"Attempting to download file: {dateTimeService.DateTimeNow()}");

            using (var client = new FilePayloadWebClient())
            {
                client.DownloadFile(filePayload);
            }

            loggingService.LogVerbose($"File download complete: {dateTimeService.DateTimeNow()}");
        }

        private class FilePayloadWebClient : WebClient
        {
            private bool acceptAnySslCert;

            private int timeoutInSeconds;

            public void DownloadFile(FilePayload filePayload)
            {
                timeoutInSeconds = filePayload.TimeoutSeconds;
                acceptAnySslCert = filePayload.AcceptAnySslCert;
                DownloadFile(filePayload.DownloadUri, filePayload.DownloadFilePath);
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request == null)
                {
                    return null;
                }
                request.Timeout = ConvertToMilliSeconds(timeoutInSeconds);

                if (acceptAnySslCert && request is HttpWebRequest)
                {
                    var httpWebRequest = request as HttpWebRequest;
                    httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
                }

                return request;
            }

            private int ConvertToMilliSeconds(int seconds)
            {
                return seconds * 1000;
            }
        }
    }
}