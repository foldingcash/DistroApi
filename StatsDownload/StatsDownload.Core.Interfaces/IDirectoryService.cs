namespace StatsDownload.Core.Interfaces
{
    public interface IDirectoryService
    {
        bool Exists(string path);
    }
}