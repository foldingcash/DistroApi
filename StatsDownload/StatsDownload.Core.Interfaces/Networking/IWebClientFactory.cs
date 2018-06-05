namespace StatsDownload.Core.Interfaces.Networking
{
    public interface IWebClientFactory
    {
        IWebClient Create();

        void Release(IWebClient webClient);
    }
}