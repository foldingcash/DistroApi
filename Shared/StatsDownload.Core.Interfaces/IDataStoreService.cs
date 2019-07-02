namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.Enums;

    public interface IDataStoreService
    {
        (bool, FailedReason) IsAvailable();
    }
}