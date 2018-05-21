namespace StatsDownload.Core.Interfaces.Networking
{
    using System;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public interface IHttpClient : IDisposable
    {
        Func<X509Certificate, X509Chain, SslPolicyErrors, bool> SslPolicyOverride { get; set; }

        TimeSpan Timeout { get; set; }

        Task<IHttpResponseMessage> GetAsync(Uri requestUri);
    }
}