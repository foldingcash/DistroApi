namespace StatsDownloadApi.Database
{
    using StatsDownloadConstants = StatsDownload.Database.Constants;

    public static class Constants
    {
        public static class StatsDownloadApiDatabase
        {
            public static readonly string GetDistroUsersProcedureName =
                $"{StatsDownloadConstants.StatsDownloadDatabase.DatabaseSchema}{StatsDownloadConstants.StatsDownloadDatabase.SchemaSeparator}[GetDistroUsers]";
        }
    }
}