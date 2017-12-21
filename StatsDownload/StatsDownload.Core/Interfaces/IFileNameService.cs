namespace StatsDownload.Core
{
    public interface IFileNameService
    {
        string GetRandomFileNamePath(string directory);
    }
}