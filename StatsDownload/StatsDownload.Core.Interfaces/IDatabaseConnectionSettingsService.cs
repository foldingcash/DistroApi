namespace StatsDownload.Core.Interfaces
{
    public interface IDatabaseConnectionSettingsService
    {
        string GetConnectionString();

        string GetDatabaseType();
    }
}