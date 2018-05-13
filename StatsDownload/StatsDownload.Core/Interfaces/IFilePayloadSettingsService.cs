namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IFilePayloadSettingsService
    {
        void SetFilePayloadDownloadDetails(FilePayload filePayload);
    }
}