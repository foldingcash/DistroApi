namespace StatsDownload.Email
{
    public interface IEmailService
    {
        EmailResult SendEmail(string subject, string body);
    }
}