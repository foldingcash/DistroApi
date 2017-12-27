namespace StatsDownload.Core
{
    using System;
    using System.IO;

    public class FileNameProvider : IFileNameService
    {
        private const string FileExtension = ".bz2";

        private const string FileName = "daily_user_summary.txt";

        private const string UncompressedFileExtension = ".txt";

        private const string UncompressedFileName = "daily_user_summary";

        private readonly IDateTimeService dateTimeService;

        public FileNameProvider(IDateTimeService dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public void SetDownloadFileDetails(string downloadDirectory, FilePayload filePayload)
        {
            DateTime now = DateTimeNow();

            string fileName = $"{now.ToFileTime()}.{FileName}";
            string uncompressedFileName = $"{now.ToFileTime()}.{UncompressedFileName}";

            filePayload.DownloadDirectory = downloadDirectory;
            filePayload.DownloadFileName = fileName;
            filePayload.DownloadFileExtension = FileExtension;
            filePayload.DownloadFilePath = Path.Combine(downloadDirectory, $"{fileName}{FileExtension}");

            filePayload.UncompressedDownloadDirectory = downloadDirectory;
            filePayload.UncompressedDownloadFileName = uncompressedFileName;
            filePayload.UncompressedDownloadFileExtension = UncompressedFileExtension;
            filePayload.UncompressedDownloadFilePath = Path.Combine(
                downloadDirectory,
                $"{uncompressedFileName}{UncompressedFileExtension}");
        }

        private DateTime DateTimeNow()
        {
            return dateTimeService.DateTimeNow();
        }
    }
}