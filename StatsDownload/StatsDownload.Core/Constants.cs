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

        public static class StatsFile
        {
            public static readonly string[] DateTimeFormats =
            {
                "ddd MMM  d HH:mm:ss {0} yyyy", "ddd MMM dd HH:mm:ss {0} yyyy"
            };

            public static readonly string[] ExpectedHeaders =
            {
                "name\tnewcredit\tsum(total)\tteam", "name\tscore\twu\tteam"
            };

            public static readonly (string timeZone, int hourOffset)[] TimeZonesAndOffset =
            {
                ("GMT", 0), ("CDT", -5), ("CST", -6), ("PDT", -7), ("PST", -8)
            };
        }
    }
}