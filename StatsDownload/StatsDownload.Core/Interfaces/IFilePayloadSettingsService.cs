namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IFilePayloadSettingsService
    {
        void SetFilePayloadDownloadDetails(FilePayload filePayload);
    }
}