namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IStatsDownloadApiDatabaseService databaseService;

        private readonly IDataStoreService dataStoreService;

        private readonly IFilePayloadApiSettingsService filePayloadApiSettingsService;

        private readonly IFileValidationService fileValidationService;

        public StatsDownloadApiDataStoreProvider(IDataStoreService dataStoreService,
                                                 IStatsDownloadApiDatabaseService databaseService,
                                                 IFileValidationService fileValidationService,
                                                 IFilePayloadApiSettingsService filePayloadApiSettingsService)
        {
            this.dataStoreService = dataStoreService;
            this.databaseService = databaseService;
            this.fileValidationService = fileValidationService;
            this.filePayloadApiSettingsService = filePayloadApiSettingsService;
        }

        public FoldingUser[] GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            IList<ValidatedFile> validatedFiles = databaseService.GetValidatedFiles(startDate, endDate);

            ValidatedFile firstFile = validatedFiles.First();
            var filePayload = new FilePayload();
            filePayloadApiSettingsService.SetFilePayloadApiSettings(filePayload);
            dataStoreService.DownloadFile(filePayload);
            ParseResults firstFileResults = fileValidationService.ValidateFile(filePayload);

            ValidatedFile lastFile = validatedFiles.Last();
            filePayload = new FilePayload();
            filePayloadApiSettingsService.SetFilePayloadApiSettings(filePayload);
            dataStoreService.DownloadFile(filePayload);
            ParseResults lastFileResults = fileValidationService.ValidateFile(filePayload);

            //TODO: Return the folding users
            return null;
        }

        public Member[] GetMembers(DateTime minValue, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Team[] GetTeams()
        {
            throw new NotImplementedException();
        }

        public bool IsAvailable()
        {
            return dataStoreService.IsAvailable();
        }
    }
}