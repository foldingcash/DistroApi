namespace StatsDownload.TestHarness
{
    using System.IO;

    using StatsDownload.Core.Interfaces;

    public class TestHarnessFileCompressionProvider : IFileCompressionService
    {
        private readonly IFileCompressionService innerService;

        private readonly ITestHarnessSettingsService settingsService;

        public TestHarnessFileCompressionProvider(IFileCompressionService innerService,
                                                  ITestHarnessSettingsService settingsService)
        {
            this.innerService = innerService;
            this.settingsService = settingsService;
        }

        public void DecompressFile(string downloadFilePath, string decompressedDownloadFilePath)
        {
            if (settingsService.IsFileCompressionDisabled())
            {
                File.Copy(downloadFilePath, decompressedDownloadFilePath);
                return;
            }

            innerService.DecompressFile(downloadFilePath, decompressedDownloadFilePath);
        }
    }
}