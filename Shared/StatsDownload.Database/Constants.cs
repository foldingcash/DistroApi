namespace StatsDownload.Database
{
    public static class Constants
    {
        public static class StatsDownloadDatabase
        {
            private const string DatabaseSchema = "[FoldingCoin]";

            private const string SchemaSeparator = ".";

            public static readonly string AddUserDataProcedureName = $"{DatabaseSchema}{SchemaSeparator}[AddUserData]";

            public static readonly string AddUserRejectionProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[AddUserRejection]";

            public static readonly string FileDownloadErrorProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[FileDownloadError]";

            public static readonly string FileDownloadFinishedProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[FileDownloadFinished]";

            public static readonly string GetDownloadsReadyForUploadSql =
                $"SELECT DownloadId FROM {DatabaseSchema}{SchemaSeparator}[DownloadsReadyForUpload]";

            public static readonly string GetFileDataProcedureName = $"{DatabaseSchema}{SchemaSeparator}[GetFileData]";

            public static readonly string GetLastFileDownloadDateTimeSql =
                $"SELECT {DatabaseSchema}{SchemaSeparator}[GetLastFileDownloadDateTime]()";

            public static readonly string NewFileDownloadStartedProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[NewFileDownloadStarted]";

            public static readonly string StartStatsUploadProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[StartStatsUpload]";

            public static readonly string StatsUploadErrorProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[StatsUploadError]";

            public static readonly string StatsUploadFinishedProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[StatsUploadFinished]";

            public static readonly string UpdateToLatestStoredProcedureName =
                $"{DatabaseSchema}{SchemaSeparator}[UpdateToLatest]";

            public static string RebuildIndicesProcedureName = $"{DatabaseSchema}{SchemaSeparator}[RebuildIndices]";
        }
    }
}