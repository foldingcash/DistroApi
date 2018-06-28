namespace StatsDownload.Core
{
    public static class Constants
    {
        public static class BitcoinAddress
        {
            public const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

            public const int Size = 25;
        }

        public static class Download
        {
            public const int MaximumTimeout = 3600;

            public const int MaximumWaitTimeInHours = 100;

            public const int MinimumTimeout = 100;

            public const int MinimumWaitTimeInHours = 1;
        }

        public static class FilePayload
        {
            public const string DecompressedFileExtension = ".txt";

            public const string DecompressedFileName = "daily_user_summary";

            public const string FileExtension = ".bz2";

            public const string FileName = "daily_user_summary.txt";
        }

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

        public static class StatsFile
        {
            public static readonly (string format, int hourOffset)[] DateTimeFormatsAndOffset =
            {
                ("ddd MMM  d HH:mm:ss CDT yyyy", -5),
                ("ddd MMM dd HH:mm:ss CDT yyyy", -5),

                ("ddd MMM  d HH:mm:ss CST yyyy", -6),
                ("ddd MMM dd HH:mm:ss CST yyyy", -6),

                ("ddd MMM  d HH:mm:ss PDT yyyy", -7),
                ("ddd MMM dd HH:mm:ss PDT yyyy", -7),

                ("ddd MMM  d HH:mm:ss PST yyyy", -8),
                ("ddd MMM dd HH:mm:ss PST yyyy", -8)
            };

            public static readonly string[] ExpectedHeaders =
            {
                "name\tnewcredit\tsum(total)\tteam",
                "name\tscore\twu\tteam"
            };
        }
    }
}