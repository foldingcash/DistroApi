namespace StatsDownload.Core
{
    public class FilePayload
    {
        public string DownloadDirectory { get; set; }

        public string DownloadFileExtension { get; set; }

        public string DownloadFileName { get; set; }

        public string DownloadFilePath { get; set; }

        public int DownloadId { get; set; }

        public string DownloadUrl { get; set; }

        public int TimeoutSeconds { get; set; }

        public string UncompressedDownloadDirectory { get; set; }

        public string UncompressedDownloadFileData { get; set; }

        public string UncompressedDownloadFileExtension { get; set; }

        public string UncompressedDownloadFileName { get; set; }

        public string UncompressedDownloadFilePath { get; set; }
    }
}