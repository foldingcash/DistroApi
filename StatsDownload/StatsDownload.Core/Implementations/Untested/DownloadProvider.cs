namespace StatsDownload.Core
{
    using System;
    using System.Net;

    public class DownloadProvider : IDownloadService
    {
        public void DownloadFile(StatsPayload statsPayload)
        {
            using (var client = new WebClientWithTimeout())
            {
                client.TimeoutInSeconds = statsPayload.TimeoutSeconds;
                client.DownloadFile(statsPayload.DownloadUrl, statsPayload.DownloadFileName);
            }
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