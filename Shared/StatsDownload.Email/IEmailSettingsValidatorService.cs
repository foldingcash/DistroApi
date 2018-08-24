namespace StatsDownload.Email
{
    using System.Collections.Generic;

    public interface IEmailSettingsValidatorService
    {
        string ParseFromAddress(string unsafeFromAddress);

        string ParseFromDisplayName(string unsafeFromDisplayName);

        string ParsePassword(string unsafePassword);

        int ParsePort(string unsafePort);

        IEnumerable<string> ParseReceivers(string unsafeReceivers);

        string ParseSmtpHost(string unsafeSmtpHost);
    }
}