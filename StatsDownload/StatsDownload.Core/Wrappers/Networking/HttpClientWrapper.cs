namespace StatsDownload.Core.Wrappers.Networking
{
    using System;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    using StatsDownload.Core.Interfaces.Networking;

    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient innerClient;

        private bool disposed;

        public HttpClientWrapper()
        {
            var requestHandler = new WebRequestHandler();
            innerClient = new HttpClient(requestHandler);
            requestHandler.ServerCertificateValidationCallback = CertValidationCallback;
        }

        public Func<X509Certificate, X509Chain, SslPolicyErrors, bool> SslPolicyOverride { get; set; }

        public TimeSpan Timeout
        {
            get
            {
                return innerClient.Timeout;
            }
            set
            {
                innerClient.Timeout = value;
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                innerClient.Dispose();
                disposed = true;
            }
        }

        public async Task<IHttpResponseMessage> GetAsync(Uri requestUri)
        {
            var result = await innerClient.GetAsync(requestUri);
            return new HttpResponseMessageWrapper(result);
        }

        private bool CertValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
                                            SslPolicyErrors sslPolicyErrors)
        {
            return SslPolicyOverride?.Invoke(certificate, chain, sslPolicyErrors) ?? false;
        }
    }
}