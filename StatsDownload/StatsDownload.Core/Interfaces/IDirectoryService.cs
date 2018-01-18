namespace StatsDownload.Core
{
    public interface IDirectoryService
    {
        bool Exists(string path);
    }
}