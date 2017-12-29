namespace StatsDownload.Core
{
    using System;
    using System.IO;

    public class FileReaderProvider : IFileReaderService
    {
        private readonly ILoggingService loggingService;

        public FileReaderProvider(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        public void ReadFile(FilePayload filePayload)
        {
            loggingService.LogVerbose($"Attempting to read file contents: {DateTime.Now}");

            using (var reader = new StreamReader(filePayload.UncompressedDownloadFilePath))
            {
                filePayload.UncompressedDownloadFileData = reader.ReadToEnd();
            }

            loggingService.LogVerbose($"Reading file complete: {DateTime.Now}");
        }
    }
}