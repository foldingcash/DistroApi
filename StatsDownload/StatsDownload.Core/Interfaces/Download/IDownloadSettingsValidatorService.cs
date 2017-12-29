namespace StatsDownload.Core
{
    public interface IDownloadSettingsValidatorService
    {
        bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert);

        bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds);
    }
}