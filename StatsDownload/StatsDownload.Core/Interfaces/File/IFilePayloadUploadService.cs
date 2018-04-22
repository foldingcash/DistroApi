namespace StatsDownload.Core
{
    using Interfaces;

    public interface IFilePayloadUploadService
    {
        void UploadFile(FilePayload filePayload);
    }
}