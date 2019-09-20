namespace StatsDownloadApi.DataStore
{
    using System;

    using StatsDownload.Core.Interfaces;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IDataStoreService dataStoreService;

        public StatsDownloadApiDataStoreProvider(IDataStoreService dataStoreService)
        {
            this.dataStoreService = dataStoreService;
        }

        public FoldingUser[] GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            //TODO: Identify the files we need to load, should just be two
            //TODO: Move those files to download directory for processing
            //TODO: Decompress the file and validate it
            // IFileValidationService

            throw new NotImplementedException();
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