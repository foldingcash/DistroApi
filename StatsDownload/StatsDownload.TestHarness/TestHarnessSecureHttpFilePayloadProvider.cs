namespace StatsDownload.TestHarness
{
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;

    public class TestHarnessSecureHttpFilePayloadProvider : ISecureFilePayloadService
    {
        private readonly ISecureFilePayloadService secureFilePayloadService;

        private readonly ITestHarnessSettingsService testHarnessSettingsService;

        public TestHarnessSecureHttpFilePayloadProvider(ISecureFilePayloadService secureFilePayloadService,
            ITestHarnessSettingsService testHarnessSettingsService)
        {
            this.secureFilePayloadService = secureFilePayloadService;
            this.testHarnessSettingsService = testHarnessSettingsService;
        }

        public void DisableSecureFilePayload(FilePayload filePayload)
        {
            secureFilePayloadService.DisableSecureFilePayload(filePayload);
        }

        public void EnableSecureFilePayload(FilePayload filePayload)
        {
            if (testHarnessSettingsService.IsSecureFilePayloadDisabled())
            {
                return;
            }

            secureFilePayloadService.EnableSecureFilePayload(filePayload);
        }

        public bool IsSecureConnection(FilePayload filePayload)
        {
            return testHarnessSettingsService.IsSecureFilePayloadDisabled()
                   || secureFilePayloadService.IsSecureConnection(filePayload);
        }
    }
}