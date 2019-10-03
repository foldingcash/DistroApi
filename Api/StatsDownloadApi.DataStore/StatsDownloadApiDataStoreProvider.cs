namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IStatsDownloadApiDatabaseService databaseService;

        private readonly IDataStoreService dataStoreService;

        private readonly IFilePayloadApiSettingsService filePayloadApiSettingsService;

        private readonly IFileValidationService fileValidationService;

        private readonly ILoggingService loggingService;

        public StatsDownloadApiDataStoreProvider(IDataStoreService dataStoreService,
                                                 IStatsDownloadApiDatabaseService databaseService,
                                                 IFileValidationService fileValidationService,
                                                 IFilePayloadApiSettingsService filePayloadApiSettingsService,
                                                 ILoggingService loggingService)
        {
            this.dataStoreService = dataStoreService;
            this.databaseService = databaseService;
            this.fileValidationService = fileValidationService;
            this.filePayloadApiSettingsService = filePayloadApiSettingsService;
            this.loggingService = loggingService;
        }

        public FoldingUser[] GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            ParseResults[] parsedFiles = GetValidatedFiles(startDate, endDate);
            FoldingUser[] results = AggregateParseResults(parsedFiles.First(), parsedFiles.Last());
            loggingService.LogMethodFinished();
            return results;
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

        private FoldingUser[] AggregateParseResults(ParseResults firstFileResults, ParseResults lastFileResults)
        {
            loggingService.LogMethodInvoked();
            var length = lastFileResults.UsersData.Count();
            var foldingUsers = new List<FoldingUser>(length);
            
            foreach (UserData userData in lastFileResults.UsersData)
            {
                var previous = firstFileResults.UsersData.FirstOrDefault(user => user.Name == userData.Name);
                if (previous is UserData)
                {
                    foldingUsers.Add(new FoldingUser(userData.FriendlyName, userData.BitcoinAddress,
                        userData.TotalPoints - previous.TotalPoints,
                        userData.TotalWorkUnits - previous.TotalWorkUnits));
                }
                else
                {
                    foldingUsers.Add(new FoldingUser(userData.FriendlyName, userData.BitcoinAddress,
                        userData.TotalPoints, userData.TotalWorkUnits));
                }
            }

            loggingService.LogMethodFinished();
            return foldingUsers.ToArray();
        }

        private ParseResults GetValidatedFile(ValidatedFile validatedFile)
        {
            loggingService.LogMethodInvoked();
            var filePayload = new FilePayload();
            filePayloadApiSettingsService.SetFilePayloadApiSettings(filePayload);
            dataStoreService.DownloadFile(filePayload, validatedFile);
            ParseResults results = fileValidationService.ValidateFile(filePayload);
            loggingService.LogMethodFinished();
            return results;
        }

        private ParseResults[] GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            IList<ValidatedFile> validatedFiles = databaseService.GetValidatedFiles(startDate, endDate);
            ParseResults[] results =
            {
                GetValidatedFile(validatedFiles.First()), GetValidatedFile(validatedFiles.Last())
            };
            loggingService.LogMethodFinished();
            return results;
        }
    }
}