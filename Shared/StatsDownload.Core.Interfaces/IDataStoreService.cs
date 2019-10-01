namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IDataStoreService
    {
        void DownloadFile(FilePayload filePayload, ValidatedFile validatedFileMock1);

        bool IsAvailable();

        void UploadFile(FilePayload filePayload);
    }
}