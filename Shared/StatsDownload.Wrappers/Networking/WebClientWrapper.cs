namespace StatsDownload.Wrappers.Networking
{
    using System;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    using StatsDownload.Core.Interfaces.Networking;

    public class WebClientWrapper : IWebClient
    {
        private readonly CustomWebClient innerWebClient;

        private bool disposed;

        public WebClientWrapper()
        {
            innerWebClient = new CustomWebClient();
        }

        public Func<X509Certificate, X509Chain, SslPolicyErrors, bool> SslPolicyOverride
        {
            get => innerWebClient.SslPolicyOverride;
            set => innerWebClient.SslPolicyOverride = value;
        }

        public TimeSpan Timeout
        {
            get => innerWebClient.Timeout;
            set => innerWebClient.Timeout = value;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                innerWebClient.Dispose();
                disposed = true;
            }
        }

        public void DownloadFile(Uri sourceUri, string saveToFilename)
        {
            innerWebClient.DownloadFile(sourceUri, saveToFilename);
        }
    }
}