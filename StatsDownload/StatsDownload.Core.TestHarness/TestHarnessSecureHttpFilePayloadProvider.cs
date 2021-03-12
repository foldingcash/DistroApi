namespace StatsDownload.Core.TestHarness
{
    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class TestHarnessSecureHttpFilePayloadProvider : ISecureFilePayloadService
    {
        private readonly ISecureFilePayloadService secureFilePayloadService;

        private readonly TestHarnessSettings settings;

        public TestHarnessSecureHttpFilePayloadProvider(ISecureFilePayloadService secureFilePayloadService,
                                                        IOptions<TestHarnessSettings> settings)
        {
            this.secureFilePayloadService = secureFilePayloadService;
            this.settings = settings.Value;
        }

        public void DisableSecureFilePayload(FilePayload filePayload)
        {
            secureFilePayloadService.DisableSecureFilePayload(filePayload);
        }

        public void EnableSecureFilePayload(FilePayload filePayload)
        {
            if (settings.DisableSecureFilePayload)
            {
                return;
            }

            secureFilePayloadService.EnableSecureFilePayload(filePayload);
        }

        public bool IsSecureConnection(FilePayload filePayload)
        {
            return settings.DisableSecureFilePayload || secureFilePayloadService.IsSecureConnection(filePayload);
        }
    }
}