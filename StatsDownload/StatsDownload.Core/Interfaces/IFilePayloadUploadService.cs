namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IFilePayloadUploadService
    {
        void UploadFile(FilePayload filePayload);
    }
}