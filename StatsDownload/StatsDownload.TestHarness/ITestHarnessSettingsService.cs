namespace StatsDownload.TestHarness
{
    public interface ITestHarnessSettingsService
    {
        bool IsMinimumWaitTimeMetDisabled();

        bool IsSecureFilePayloadDisabled();
    }
}