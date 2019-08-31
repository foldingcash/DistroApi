namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFilePayloadUploadService
    {
        void UploadFile(FilePayload filePayload);
    }
}