namespace StatsDownload.TestHarness
{
    public interface ITestHarnessSettingsService
    {
        bool IsMinimumWaitTimeMetDisabled();

        bool IsOneHundredUsersFilterEnabled();

        bool IsSecureFilePayloadDisabled();
    }
}