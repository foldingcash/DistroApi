namespace StatsDownload.Core
{
    public interface IResourceCleanupService
    {
        void Cleanup(FilePayload filePayload);
    }
}