namespace StatsDownload.Core
{
    using System;
    using System.Net;

    public class DownloadProvider : IDownloadService
    {
        private readonly ILoggingService loggingService;

        public DownloadProvider(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            loggingService.LogVerbose($"Attempting to download file: {DateTime.Now}");

            using (var client = new WebClientWithTimeout())
            {
                client.TimeoutInSeconds = filePayload.TimeoutSeconds;
                client.DownloadFile(filePayload.DownloadUri, filePayload.DownloadFilePath);
            }

            loggingService.LogVerbose($"File download complete: {DateTime.Now}");
        }

        private class WebClientWithTimeout : WebClient
        {
            public int TimeoutInSeconds { private get; set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request == null)
                {
                    return null;
                }
                request.Timeout = ConvertToMilliSeconds(TimeoutInSeconds);
                return request;
            }

            private int ConvertToMilliSeconds(int timeoutInSeconds)
            {
                return timeoutInSeconds * 1000;
            }
        }
    }
}