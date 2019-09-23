namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IStatsDownloadApiDatabaseService databaseService;

        private readonly IDataStoreService dataStoreService;

        public StatsDownloadApiDataStoreProvider(IDataStoreService dataStoreService,
                                                 IStatsDownloadApiDatabaseService databaseService)
        {
            this.dataStoreService = dataStoreService;
            this.databaseService = databaseService;
        }

        public FoldingUser[] GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            IList<ValidatedFile> validatedFiles = databaseService.GetValidatedFiles(startDate, endDate);

            //TODO: Identify the files we need to load, should just be two
            //TODO: Move those files to download directory for processing
            //TODO: Decompress the file and validate it
            //TODO: Return the folding users
            // IFileValidationService
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