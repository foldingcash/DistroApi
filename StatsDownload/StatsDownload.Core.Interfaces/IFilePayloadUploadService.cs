namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IFilePayloadUploadService
    {
        void UploadFile(FilePayload filePayload);
    }
}