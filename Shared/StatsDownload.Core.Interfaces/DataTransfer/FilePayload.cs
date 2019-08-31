namespace StatsDownload.Core.Interfaces.DataTransfer
{
    using System;

    public class FilePayload
    {
        public bool AcceptAnySslCert { get; set; }

        public string DecompressedDownloadDirectory { get; set; }

        public string DecompressedDownloadFileData { get; set; }

        public string DecompressedDownloadFileExtension { get; set; }

        public string DecompressedDownloadFileName { get; set; }

        public string DecompressedDownloadFilePath { get; set; }

        public string DownloadDirectory { get; set; }

        public byte[] DownloadFileData { get; set; }

        public string DownloadFileExtension { get; set; }

        public string DownloadFileName { get; set; }

        public string DownloadFilePath { get; set; }

        public int DownloadId { get; set; }

        public Uri DownloadUri { get; set; }

        public string FailedDownloadFilePath { get; set; }

        public DateTime? FileUtcDateTime { get; set; }

        public TimeSpan MinimumWaitTimeSpan { get; set; }

        public int TimeoutSeconds { get; set; }

        public string UploadPath { get; set; }
    }
}