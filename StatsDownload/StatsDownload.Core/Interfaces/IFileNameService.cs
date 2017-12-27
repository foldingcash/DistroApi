namespace StatsDownload.Core
{
    public interface IFileNameService
    {
        void SetDownloadFileDetails(string downloadDirectory, FilePayload filePayload);
    }
}