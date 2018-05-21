namespace StatsDownload.Core.Interfaces.Networking
{
    public interface IHttpClientFactory
    {
        IHttpClient Create();
    }
}