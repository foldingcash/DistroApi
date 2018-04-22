namespace StatsDownload.Core
{
    using Interfaces;

    public interface IFilePayloadSettingsService
    {
        void SetFilePayloadDownloadDetails(FilePayload filePayload);
    }
}