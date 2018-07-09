namespace StatsDownload.Core.Interfaces.Networking
{
    using System;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public interface IWebClient : IDisposable
    {
        Func<X509Certificate, X509Chain, SslPolicyErrors, bool> SslPolicyOverride { get; set; }

        TimeSpan Timeout { get; set; }

        void DownloadFile(Uri sourceUri, string saveToFilename);
    }
}