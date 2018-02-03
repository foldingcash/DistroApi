namespace StatsDownload.Core
{
    public interface IFileDeleteService
    {
        void Delete(string path);

        bool Exists(string path);
    }
}