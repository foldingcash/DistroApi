namespace StatsDownload.Email
{
    public interface IEmailSettingsService
    {
        string GetFromAddress();

        string GetFromDisplayName();

        string GetHostName();

        string GetPassword();

        string GetPort();

        string GetReceivers();
    }
}