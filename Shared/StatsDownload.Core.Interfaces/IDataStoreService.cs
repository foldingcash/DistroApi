namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IDataStoreService
    {
        void DownloadFile(FilePayload filePayload, ValidatedFile validatedFile);

        bool IsAvailable();

        void UploadFile(FilePayload filePayload);
    }
}