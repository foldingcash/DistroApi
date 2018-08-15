namespace StatsDownload.Email
{
    public interface IEmailSettingsService
    {
        string GetFromAddress();

        string GetFromDisplayName();

        string GetPassword();

        string GetPort();

        string GetReceivers();

        string GetSmtpHost();
    }
}