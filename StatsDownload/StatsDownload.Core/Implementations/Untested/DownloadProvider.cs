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

            using (var client = new FilePayloadWebClient())
            {
                client.DownloadFile(filePayload);
            }

            loggingService.LogVerbose($"File download complete: {DateTime.Now}");
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