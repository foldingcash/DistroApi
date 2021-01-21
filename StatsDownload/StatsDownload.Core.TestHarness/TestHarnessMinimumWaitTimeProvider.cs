namespace StatsDownload.Core.TestHarness
{
    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class TestHarnessMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService;

        private readonly TestHarnessSettings settings;

        public TestHarnessMinimumWaitTimeProvider(
            IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService,
            IOptions<TestHarnessSettings> settings)
        {
            this.fileDownloadMinimumWaitTimeService = fileDownloadMinimumWaitTimeService;
            this.settings = settings.Value;
        }

        public bool IsMinimumWaitTimeMet(FilePayload filePayload)
        {
            return settings.DisableMinimumWaitTime
                   || fileDownloadMinimumWaitTimeService.IsMinimumWaitTimeMet(filePayload);
        }
    }
}