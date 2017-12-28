namespace StatsDownload.Core
{
    public interface ISecureDetectionService
    {
        bool IsSecureConnection(FilePayload filePayload);
    }
}