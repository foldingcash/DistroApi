namespace StatsDownload.Core.Interfaces
{
    public interface IDataStoreServiceFactory
    {
        IDataStoreService Create();
    }
}