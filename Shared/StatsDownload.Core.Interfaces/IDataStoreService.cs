namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IDataStoreService
    {
        bool IsAvailable();

        void UploadFile(FilePayload filePayload);
    }
}