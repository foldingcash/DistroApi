namespace StatsDownload.Core.Interfaces
{
    public interface IDataStoreSettings
    {
        string DataStoreType { get; }

        string UploadDirectory { get; }
    }
}