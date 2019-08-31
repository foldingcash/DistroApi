namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public interface IDataStoreService
    {
        (bool, FailedReason) IsAvailable();

        void UploadFile(FilePayload filePayload);
    }
}