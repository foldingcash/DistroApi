namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Threading.Tasks;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Exceptions;

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

        public async Task UploadFile(FilePayload filePayload)
        {
            fileValidationService.PreValidateFile(filePayload);
            await dataStoreService.UploadFile(filePayload);
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