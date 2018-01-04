namespace StatsDownload.Core
{
    using System;

    public interface IDownloadSettingsValidatorService
    {
        bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert);

        bool TryParseMinimumWaitTimeSpan(string unsafeMinimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan);

        bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds);
    }
}