namespace StatsDownload.Core.Interfaces
{
    using System;

    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IStatsUploadService
    {
        [Obsolete("No longer maintaining this. All functionality may not be working correctly or at all.")]
        StatsUploadResults UploadStatsFiles();
    }
}