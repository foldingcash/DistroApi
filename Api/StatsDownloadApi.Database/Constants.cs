namespace StatsDownloadApi.Database
{
    using StatsDownloadConstants = StatsDownload.Database.Constants;

    public static class Constants
    {
        public static class StatsDownloadApiDatabase
        {
            public static readonly string GetFoldingMembersProcedureName =
                $"{StatsDownloadConstants.StatsDownloadDatabase.DatabaseSchema}{StatsDownloadConstants.StatsDownloadDatabase.SchemaSeparator}[GetFoldingMembers]";
            public static readonly string GetMembersProcedureName =
                $"{StatsDownloadConstants.StatsDownloadDatabase.DatabaseSchema}{StatsDownloadConstants.StatsDownloadDatabase.SchemaSeparator}[GetMembers]";
            public static readonly string GetTeamsProcedureName =
                $"{StatsDownloadConstants.StatsDownloadDatabase.DatabaseSchema}{StatsDownloadConstants.StatsDownloadDatabase.SchemaSeparator}[GetTeams]";
            public static readonly string[] ApiObjects =
            {
                GetFoldingMembersProcedureName, GetMembersProcedureName, GetTeamsProcedureName
            };

            public static string GetValidatedFilesProcedureName =>
                $"{StatsDownloadConstants.StatsDownloadDatabase.DatabaseSchema}{StatsDownloadConstants.StatsDownloadDatabase.SchemaSeparator}[GetValidatedFiles]";
        }
    }
}