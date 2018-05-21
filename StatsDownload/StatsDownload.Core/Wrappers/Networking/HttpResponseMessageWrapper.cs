namespace StatsDownload.Core.Wrappers.Networking
{
    using System.Net;
    using System.Net.Http;

    using StatsDownload.Core.Interfaces.Networking;

    public class HttpResponseMessageWrapper : IHttpResponseMessage
    {
        private readonly HttpResponseMessage innerResponseMessage;

        private IHttpContent content;

        public HttpResponseMessageWrapper(HttpResponseMessage innerResponseMessage)
        {
            this.innerResponseMessage = innerResponseMessage;
        }

        public IHttpContent Content
        {
            get
            {
                if (content == null)
                {
                    content = new HttpContentWrapper(innerResponseMessage.Content);
                }

                return content;
            }
        }

        public bool IsSuccessStatusCode => innerResponseMessage.IsSuccessStatusCode;

        public HttpStatusCode StatusCode => innerResponseMessage.StatusCode;
    }
}