namespace StatsDownload.Core.Interfaces
{
    public interface IDatabaseConnectionSettingsService
    {
        int? GetCommandTimeout();

        string GetConnectionString();

        string GetDatabaseType();
    }
}