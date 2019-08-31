namespace StatsDownload.TestHarness
{
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class TestHarnessMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService;

        private readonly ITestHarnessSettingsService testHarnessSettingsService;

        public TestHarnessMinimumWaitTimeProvider(
            IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService,
            ITestHarnessSettingsService testHarnessSettingsService)
        {
            this.fileDownloadMinimumWaitTimeService = fileDownloadMinimumWaitTimeService;
            this.testHarnessSettingsService = testHarnessSettingsService;
        }

        public bool IsMinimumWaitTimeMet(FilePayload filePayload)
        {
            return testHarnessSettingsService.IsMinimumWaitTimeMetDisabled()
                   || fileDownloadMinimumWaitTimeService.IsMinimumWaitTimeMet(filePayload);
        }
    }
}