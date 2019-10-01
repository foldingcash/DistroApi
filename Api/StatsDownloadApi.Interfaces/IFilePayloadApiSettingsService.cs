namespace StatsDownloadApi.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public interface IFilePayloadApiSettingsService
    {
        void SetFilePayloadApiSettings(FilePayload filePayload, ValidatedFile validatedFileForUpload);
    }
}