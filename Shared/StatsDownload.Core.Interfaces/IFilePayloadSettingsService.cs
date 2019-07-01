namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFilePayloadSettingsService
    {
        void SetFilePayloadDownloadDetails(FilePayload filePayload);
    }
}