namespace StatsDownloadApi.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFilePayloadApiSettingsService
    {
        void SetFilePayloadApiSettings(FilePayload filePayload);
    }
}