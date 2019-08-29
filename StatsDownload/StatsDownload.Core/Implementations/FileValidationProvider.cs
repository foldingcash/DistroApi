namespace StatsDownload.Core.Implementations
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class FileValidationProvider : IFileValidationService
    {
        public void ValidateFile(FilePayload filePayload)
        {
            //fileCompressionServiceMock.DecompressFile(DownloadFilePath, DecompressedDownloadFilePath);
            //fileReaderServiceMock.ReadFile(filePayload);
            throw new NotImplementedException();
        }
    }
}