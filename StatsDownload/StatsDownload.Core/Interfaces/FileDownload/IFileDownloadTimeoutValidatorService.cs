namespace StatsDownload.Core
{
    public interface IFileDownloadTimeoutValidatorService
    {
        bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds);
    }
}