namespace StatsDownload.Core.Interfaces.Networking
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IHttpContent
    {
        Task<byte[]> ReadAsByteArrayAsync();

        Task<Stream> ReadAsStreamAsync();

        Task<string> ReadAsStringAsync();
    }
}