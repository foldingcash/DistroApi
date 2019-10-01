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
            ParseResults[] parsedFiles = GetValidatedFiles(startDate, endDate);
            return AggregateParseResults(parsedFiles.First(), parsedFiles.Last());
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
            var foldingUsers = new List<FoldingUser>();

            foreach (UserData userData in lastFileResults.UsersData)
            {
                if (firstFileResults.UsersData.Any(user => user.Name == userData.Name))
                {
                    UserData previous = firstFileResults.UsersData.First(user => user.Name == userData.Name);
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

            return foldingUsers.ToArray();
        }

        private ParseResults GetValidatedFile(ValidatedFile validatedFile)
        {
            var filePayload = new FilePayload();
            filePayloadApiSettingsService.SetFilePayloadApiSettings(filePayload);
            dataStoreService.DownloadFile(filePayload, validatedFile);
            return fileValidationService.ValidateFile(filePayload);
        }

        private ParseResults[] GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            IList<ValidatedFile> validatedFiles = databaseService.GetValidatedFiles(startDate, endDate);

            return new[] { GetValidatedFile(validatedFiles.First()), GetValidatedFile(validatedFiles.Last()) };
        }
    }
}