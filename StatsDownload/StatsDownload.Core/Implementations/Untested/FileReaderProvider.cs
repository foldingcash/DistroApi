namespace StatsDownload.Core
{
    using System.IO;

    public class FileReaderProvider : IFileReaderService
    {
        public void ReadFile(FilePayload filePayload)
        {
            using (var reader = new StreamReader(filePayload.UncompressedDownloadFilePath))
            {
                filePayload.UncompressedDownloadFileData = reader.ReadToEnd();
            }
        }
    }
}