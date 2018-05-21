namespace StatsDownload.Core.Interfaces.Networking
{
    using System.Net;

    public interface IHttpResponseMessage
    {
        IHttpContent Content { get; }

        bool IsSuccessStatusCode { get; }

        HttpStatusCode StatusCode { get; }
    }
}