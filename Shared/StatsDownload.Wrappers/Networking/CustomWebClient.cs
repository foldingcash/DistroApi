namespace StatsDownload.Wrappers.Networking
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public class CustomWebClient : WebClient
    {
        public Func<X509Certificate, X509Chain, SslPolicyErrors, bool> SslPolicyOverride { get; set; }

        public TimeSpan Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request == null)
            {
                return null;
            }

            request.Timeout = Convert.ToInt32(Timeout.TotalMilliseconds);

            if (request is HttpWebRequest)
            {
                var httpWebRequest = request as HttpWebRequest;
                httpWebRequest.ServerCertificateValidationCallback += ServerCertificateValidateCallback;
            }

            return request;
        }

        private bool ServerCertificateValidateCallback(object sender, X509Certificate certificate, X509Chain chain,
                                                       SslPolicyErrors errors)
        {
            return SslPolicyOverride?.Invoke(certificate, chain, errors) ?? errors == SslPolicyErrors.None;
        }
    }
}