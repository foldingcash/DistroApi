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
                                         IDataStoreService dataStoreService,
                                         IFileValidationService fileValidationService)
        {
            this.fileDownloadDatabaseService = fileDownloadDatabaseService
                                               ?? throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            this.dataStoreService = dataStoreService ?? throw new ArgumentNullException(nameof(dataStoreService));
            this.fileValidationService =
                fileValidationService ?? throw new ArgumentNullException(nameof(fileValidationService));
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
            catch (Exception exception)
            {
                throw new UnexpectedValidationException(exception);
            }
        }
    }
}