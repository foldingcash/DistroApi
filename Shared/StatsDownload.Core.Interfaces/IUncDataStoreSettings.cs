namespace StatsDownload.Core.Interfaces
{
    using System;

    public interface IUncDataStoreSettings
    {
        Uri UncUploadDirectory { get; }
    }
}