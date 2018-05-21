namespace StatsDownload.Core.Wrappers.Networking
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using StatsDownload.Core.Interfaces.Networking;

    public class HttpContentWrapper : IHttpContent
    {
        private readonly HttpContent innerHttpContent;

        public HttpContentWrapper(HttpContent innerHttpContent)
        {
            this.innerHttpContent = innerHttpContent;
        }

        public Task<byte[]> ReadAsByteArrayAsync()
        {
            return innerHttpContent.ReadAsByteArrayAsync();
        }

        public Task<Stream> ReadAsStreamAsync()
        {
            return innerHttpContent.ReadAsStreamAsync();
        }

        public Task<string> ReadAsStringAsync()
        {
            return innerHttpContent.ReadAsStringAsync();
        }
    }
}