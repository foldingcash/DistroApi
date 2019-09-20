namespace StatsDownload.Core
{
    public static class Constants
    {
        

        public static class Download
        {
            public const int MaximumTimeout = 3600;

            public const int MaximumWaitTimeInHours = 100;

            public const int MinimumTimeout = 100;

            public const int MinimumWaitTimeInHours = 1;
        }

        public static class FilePayload
        {
            public const string DecompressedFileExtension = "txt";

            public const string DecompressedFileName = "daily_user_summary";

            public const string FileExtension = "bz2";

            public const string FileName = "daily_user_summary.txt";
        }

        
    }
}