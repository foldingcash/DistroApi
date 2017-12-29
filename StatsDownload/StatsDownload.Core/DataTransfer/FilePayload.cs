namespace StatsDownload.Core
{
    using System;

    public class FilePayload
    {
        public bool AcceptAnySslCert { get; set; }

        public string DownloadDirectory { get; set; }

        public string DownloadFileExtension { get; set; }

        public string DownloadFileName { get; set; }

        public string DownloadFilePath { get; set; }

        public int DownloadId { get; set; }

        public Uri DownloadUri { get; set; }

        public int TimeoutSeconds { get; set; }

        public string UncompressedDownloadDirectory { get; set; }

        public string UncompressedDownloadFileData { get; set; }

        public string UncompressedDownloadFileExtension { get; set; }

        public string UncompressedDownloadFileName { get; set; }

        public string UncompressedDownloadFilePath { get; set; }
    }
}