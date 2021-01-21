namespace StatsDownload.Core.TestHarness
{
    public class TestHarnessSettings
    {
        public bool DisableMinimumWaitTime { get; set; }

        public bool DisableSecureFilePayload { get; set; }

        public bool EnableOneHundredUsersFilter { get; set; }

        public bool EnableSqlExceptionDuringAddUsersTest { get; set; }

        public bool FileCompressionDisabled { get; set; }
    }
}