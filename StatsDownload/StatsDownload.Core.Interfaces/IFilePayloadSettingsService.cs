namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IFilePayloadSettingsService
    {
        void SetFilePayloadDownloadDetails(FilePayload filePayload);
    }
}