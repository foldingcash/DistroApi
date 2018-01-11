namespace StatsDownload.Email
{
    public interface IEmailService
    {
        EmailResult SendEmail(string name, string email, string subject, string body);
    }
}