namespace StatsDownload.Core.TestHarness
{
    using System.IO;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;

    public class TestHarnessFileCompressionProvider : IFileCompressionService
    {
        private readonly IFileCompressionService innerService;

        private readonly TestHarnessSettings settings;

        public TestHarnessFileCompressionProvider(IFileCompressionService innerService,
                                                  IOptions<TestHarnessSettings> settings)
        {
            this.innerService = innerService;
            this.settings = settings.Value;
        }

        public void CompressFile(string filePath, string compressedFilePath)
        {
            innerService.CompressFile(filePath, compressedFilePath);
        }

        public void DecompressFile(string downloadFilePath, string decompressedDownloadFilePath)
        {
            if (settings.FileCompressionDisabled)
            {
                File.Copy(downloadFilePath, decompressedDownloadFilePath);
                return;
            }

            innerService.DecompressFile(downloadFilePath, decompressedDownloadFilePath);
        }
    }
}