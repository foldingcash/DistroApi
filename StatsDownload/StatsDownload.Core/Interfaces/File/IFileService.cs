namespace StatsDownload.Core
{
    public interface IFileService
    {
        void Delete(string path);

        bool Exists(string path);
    }
}