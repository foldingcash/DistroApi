namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Net;

    public class WebClientWithTimeout : WebClient
    {
        public int TimeoutInSeconds { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            request.Timeout = ConvertToMilliSeconds(TimeoutInSeconds);
            return request;
        }

        private int ConvertToMilliSeconds(int timeoutInSeconds)
        {
            return timeoutInSeconds * 1000;
        }
    }
}