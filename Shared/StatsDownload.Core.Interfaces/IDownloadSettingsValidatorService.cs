namespace StatsDownload.Core.Interfaces
{
    using System;

    public interface IDownloadSettingsValidatorService
    {
        bool IsValidDownloadDirectory(string unsafeDownloadDirectory);
        
        bool TryParseDownloadUri(string unsafeDownloadUri, out Uri downloadUri);

        bool TryParseMinimumWaitTimeSpan(int minimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan);

        bool TryParseTimeout(int timeout, out int timeoutInSeconds);
    }
}