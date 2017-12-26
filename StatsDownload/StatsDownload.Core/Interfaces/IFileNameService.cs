namespace StatsDownload.Core
{
    public interface IFileNameService
    {
        string GetNewFilePath(string directory);
    }
}