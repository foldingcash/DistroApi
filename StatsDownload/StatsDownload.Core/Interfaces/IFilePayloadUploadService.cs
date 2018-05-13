namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IFilePayloadUploadService
    {
        void UploadFile(FilePayload filePayload);
    }
}