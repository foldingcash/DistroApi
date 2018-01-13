namespace StatsDownload.Email
{
    using System.Collections.Generic;

    public interface IEmailSettingsValidatorService
    {
        int ParsePort(string unsafePort);

        IEnumerable<string> ParseReceivers(string unsafeReceivers);
    }
}