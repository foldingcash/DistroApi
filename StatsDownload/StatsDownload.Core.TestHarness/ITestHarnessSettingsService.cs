namespace StatsDownload.Core.TestHarness
{
    public interface ITestHarnessSettingsService
    {
        bool IsFileCompressionDisabled();

        bool IsMinimumWaitTimeMetDisabled();

        bool IsOneHundredUsersFilterEnabled();

        bool IsSecureFilePayloadDisabled();
    }
}