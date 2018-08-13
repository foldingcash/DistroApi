namespace StatsDownloadApi.Database
{
    using StatsDownloadConstants = StatsDownload.Database.Constants;

    public static class Constants
    {
        public static class StatsDownloadApiDatabase
        {
            public static readonly string GetFoldingUsersProcedureName =
                $"{StatsDownloadConstants.StatsDownloadDatabase.DatabaseSchema}{StatsDownloadConstants.StatsDownloadDatabase.SchemaSeparator}[GetFoldingUsers]";

            public static readonly string GetTeamsProcedureName =
                $"{StatsDownloadConstants.StatsDownloadDatabase.DatabaseSchema}{StatsDownloadConstants.StatsDownloadDatabase.SchemaSeparator}[GetTeams]";
        }
    }
}