namespace StatsDownload.Core
{
    using System;
    using System.Net;

    public class FileDownloaderProvider : IFileDownloaderService
    {
        public void DownloadFile(string address, string fileName, int timeoutInSeconds)
        {
            using (var client = new WebClientWithTimeout())
            {
                client.TimeoutInSeconds = timeoutInSeconds;
                client.DownloadFile(address, fileName);
            }
        }

        private class WebClientWithTimeout : WebClient
        {
            public int TimeoutInSeconds { get; set; }

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