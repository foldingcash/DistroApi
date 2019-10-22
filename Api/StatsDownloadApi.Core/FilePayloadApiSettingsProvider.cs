namespace StatsDownloadApi.Core
{
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    using StatsDownloadApi.Interfaces;

    public class FilePayloadApiSettingsProvider : IFilePayloadApiSettingsService
    {
        private readonly IFilePayloadSettingsService innerService;

        public FilePayloadApiSettingsProvider(IFilePayloadSettingsService innerService)
        {
            this.innerService = innerService;
        }

        public void SetFilePayloadApiSettings(FilePayload filePayload)
        {
            innerService.SetFilePayloadDownloadDetails(filePayload);
        }
    }
}