namespace StatsDownload.Email
{
    public interface IEmailCredentials
    {
        string Address { get; }

        string DisplayName { get; }

        string HostName { get; }

        string Password { get; }

        int Port { get; }
    }
}