namespace StatsDownload.Core.Implementations
{
    using System;

    using StatsDownload.Core.Exceptions;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class FilePayloadUploadProvider : IFilePayloadUploadService
    {
        private readonly IDataStoreService dataStoreService;

        private readonly IFileDownloadDatabaseService fileDownloadDatabaseService;

        private readonly IFileValidationService fileValidationService;

        public FilePayloadUploadProvider(IFileDownloadDatabaseService fileDownloadDatabaseService,
                                         IDataStoreServiceFactory dataStoreServiceFactory,
                                         IFileValidationService fileValidationService)
        {
            this.fileDownloadDatabaseService = fileDownloadDatabaseService
                                               ?? throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            this.fileValidationService =
                fileValidationService ?? throw new ArgumentNullException(nameof(fileValidationService));

            dataStoreService = dataStoreServiceFactory?.Create()
                               ?? throw new ArgumentNullException(nameof(dataStoreService));
        }

        public void UploadFile(FilePayload filePayload)
        {
            dataStoreService.UploadFile(filePayload);
            fileDownloadDatabaseService.FileDownloadFinished(filePayload);

            try
            {
                fileDownloadDatabaseService.FileValidationStarted(filePayload);
                fileValidationService.ValidateFile(filePayload);
                fileDownloadDatabaseService.FileValidated(filePayload);
            }
            catch (FileDownloadFailedDecompressionException)
            {
                throw;
            }
            catch (InvalidStatsFileException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new UnexpectedValidationException(exception);
            }
        }
    }
}