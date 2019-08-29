namespace StatsDownload.Database
{
    public static class Constants
    {
        public static class FileDownloadDatabase
        {
            public static readonly string DatabaseSchema = StatsDownloadDatabase.DatabaseSchema;

            public static readonly string SchemaSeparator = StatsDownloadDatabase.SchemaSeparator;

            public static readonly string FileDownloadErrorProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[FileDownloadError]";

            public static readonly string FileDownloadFinishedProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[FileDownloadFinished]";
            public static readonly string GetLastFileDownloadDateTimeSqlFunction =
                $"{DatabaseSchema}{SchemaSeparator}[GetLastFileDownloadDateTime]";
            public static readonly string NewFileDownloadStartedProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[NewFileDownloadStarted]";
            public static readonly string UpdateToLatestStoredProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[UpdateToLatest]";
            public static readonly string[] FileDownloadObjects =
            {
                UpdateToLatestStoredProcedureName,
                GetLastFileDownloadDateTimeSqlFunction,
                NewFileDownloadStartedProcedureName,
                FileDownloadFinishedProcedureName,
                FileDownloadErrorProcedureName
            };

            public static readonly string GetLastFileDownloadDateTimeSql =
                $"SELECT {GetLastFileDownloadDateTimeSqlFunction}()";
        }

        public static class StatsDownloadDatabase
        {
            public const string DatabaseSchema = "[FoldingCoin]";

            public const string SchemaSeparator = ".";
        }

        public static class StatsUploadDatabase
        {
            public static readonly string DatabaseSchema = StatsDownloadDatabase.DatabaseSchema;

            public static readonly string SchemaSeparator = StatsDownloadDatabase.SchemaSeparator;

            public static readonly string AddUserDataProcedureName = $"{DatabaseSchema}{SchemaSeparator}[AddUserData]";

            public static readonly string AddUserRejectionProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[AddUserRejection]";

            public static string GetDownloadsReadyForUploadSqlView =
                $"{DatabaseSchema}{SchemaSeparator}[DownloadsReadyForUpload]";
            public static readonly string GetDownloadsReadyForUploadSql =
                $"SELECT DownloadId FROM {GetDownloadsReadyForUploadSqlView}";

            public static readonly string GetFileDataProcedureName = $"{DatabaseSchema}{SchemaSeparator}[GetFileData]";

            public static string RebuildIndicesProcedureName = $"{DatabaseSchema}{SchemaSeparator}[RebuildIndices]";

            public static readonly string StartStatsUploadProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[StartStatsUpload]";

            public static readonly string StatsUploadErrorProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[StatsUploadError]";

            public static readonly string StatsUploadFinishedProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[StatsUploadFinished]";

            public static readonly string[] StatsUploadObjects =
            {
                AddUserDataProcedureName,
                AddUserRejectionProcedureName,
                GetDownloadsReadyForUploadSqlView,
                GetFileDataProcedureName,
                RebuildIndicesProcedureName,
                StartStatsUploadProcedureName,
                StatsUploadFinishedProcedureName,
                StatsUploadErrorProcedureName
            };
        }
    }
}