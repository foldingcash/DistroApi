namespace StatsDownload.Core
{
    using System;

    public interface IDownloadSettingsValidatorService
    {
        bool IsValidDownloadDirectory(string unsafeDownloadDirectory);

        bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert);

        bool TryParseDownloadUri(string unsafeDownloadUri, out Uri downloadUri);

        bool TryParseMinimumWaitTimeSpan(string unsafeMinimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan);

        bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds);
    }
}